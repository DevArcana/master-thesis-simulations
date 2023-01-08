using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulator.Gui;

namespace Simulator;

public class Simulator : Game
{
    private ImGuiRenderer _imGuiRenderer;

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

        base.Initialize();
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

    protected virtual void ImGuiLayout()
    {
        ImGui.Begin("Simulator", ImGuiWindowFlags.AlwaysAutoResize);
        ImGuiFramerate();
        ImGui.End();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _imGuiRenderer.BeforeLayout(gameTime);
        ImGuiLayout();
        _imGuiRenderer.AfterLayout();

        base.Draw(gameTime);
    }
}