using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.Playground;

public class PlaygroundScene : Scene
{
    private readonly PlaygroundSettings _settings = new();

    private long _tick = 0;
    private double _timer = 0;

    public PlaygroundScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base("Playground", graphicsDevice, imGuiRenderer)
    {
    }

    protected override void OnCreate()
    {
        AddLayer(new AgentSceneLayer(_settings, GraphicsDevice));
        _timer = _settings.Heartbeat;
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _timer -= gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer <= 0.0f)
        {
            _timer = _settings.Heartbeat;
            _tick++;
            Heartbeat();
        }
    }

    public void Heartbeat()
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
        ImGui.Begin("Playground", ImGuiWindowFlags.AlwaysAutoResize);

        ImGui.Text($"Heartbeat: {_timer:F2}s Tick: {_tick}");

        ImGui.End();
    }
}