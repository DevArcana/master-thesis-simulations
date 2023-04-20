using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.Playground;

public class PlaygroundScene : Scene
{
    private readonly PlaygroundSettings _settings = new();
    
    public PlaygroundScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base("Playground", graphicsDevice, imGuiRenderer)
    {
    }

    protected override void OnCreate()
    {
        AddLayer(new AgentSceneLayer(_settings, GraphicsDevice));
    }

    protected override void OnUpdate(GameTime gameTime)
    {
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
    }

    protected override void OnGui()
    {
    }
}