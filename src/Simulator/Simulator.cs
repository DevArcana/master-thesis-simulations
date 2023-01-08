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

    protected virtual void ImGuiLayout()
    {
        ImGui.Begin("Simulator");
        ImGui.Text("This is still work in progress.");
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