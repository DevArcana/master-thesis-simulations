using System;
using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using InfoWave.MonoGame.Scenes.DiseaseSpread;
using InfoWave.MonoGame.Scenes.Playground;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InfoWave.MonoGame;

public sealed class Simulator : Game
{
    private ImGuiRenderer _imGuiRenderer = null!;
    private Scene? _scene;

    public Simulator()
    {
        var graphics = new GraphicsDeviceManager(this);

        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        graphics.PreferMultiSampling = true;
        graphics.SynchronizeWithVerticalRetrace = false;

        IsMouseVisible = true;

        Window.IsBorderless = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();

        _scene = new PlaygroundScene(GraphicsDevice, _imGuiRenderer);

        base.Initialize();
        
        var run = 0;
        while (run < 100)
        {
            run++;
            
            var scene = new DiseaseSpreadScene(GraphicsDevice, _imGuiRenderer, run, "experiment1");
            scene.Update(new GameTime());
            for (int i = 0; i < 128; i++)
            {
                scene.ForceTick();
            }
            scene.Destroy();
        }
        Exit();
    }

    private void ImGuiFramerate()
    {
        var framerate = ImGui.GetIO().Framerate;
        ImGui.Text($"frame time: {1000f / framerate:F3} ms/frame ({framerate:F1} FPS)");

        var fps = (int)Math.Round(1000d / TargetElapsedTime.TotalMilliseconds) / 30 - 1;
        if (ImGui.Combo("FPS", ref fps, "30\060\090\0120\0"))
        {
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000d / ((fps + 1) * 30));
        }
    }

    private void ImGuiScenes()
    {
        
    }

    private void ImGuiLayout()
    {
        ImGui.Begin("Simulator", ImGuiWindowFlags.AlwaysAutoResize);
        ImGuiFramerate();
        ImGuiScenes();
        ImGui.End();

        _scene?.DrawGui();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _scene?.Update(gameTime);
        if (_scene is { Status: SceneStatus.Destroyed })
        {
            _scene = null;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        // GraphicsDevice.Clear(new Color(34, 35, 35));
        GraphicsDevice.Clear(Color.Black);

        _scene?.Draw(gameTime);

        _imGuiRenderer.BeforeLayout(gameTime);
        ImGuiLayout();
        _imGuiRenderer.AfterLayout();

        base.Draw(gameTime);
    }
}