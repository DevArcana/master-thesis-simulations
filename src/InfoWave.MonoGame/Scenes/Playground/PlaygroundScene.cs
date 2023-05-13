using Arch.Core;
using Arch.Core.Extensions;
using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.Playground;

public class Ticker
{
    private long _tick = 0;
    private double _timer = 0;

    private const double TickRate = 1.0f;

    public long Ticks => _tick;

    public bool Tick(GameTime gameTime)
    {
        _timer -= gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer <= 0.0f)
        {
            _timer = TickRate;
            _tick++;
            return true;
        }

        return false;
    }
}

public class PlaygroundScene : Scene
{
    private readonly Ticker _ticker = new();
    private readonly World _world = World.Create();
    
    // Systems
    private readonly RenderingSystem _renderingSystem;
    private readonly InferenceSystem _inferenceSystem;
    private readonly BehaviourSystem _behaviourSystem;

    public PlaygroundScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base("Playground", graphicsDevice, imGuiRenderer)
    {
        _renderingSystem = new RenderingSystem(_world, graphicsDevice, SpriteBatch);
        _inferenceSystem = new InferenceSystem(_world);
        _behaviourSystem = new BehaviourSystem(_world);
    }

    protected override void OnCreate()
    {
        var arena = _world.CreateArena(12, 24).Get<Grid>();
        arena[7, 2] = 1;
        arena[7, 3] = 1;
        arena[7, 4] = 1;
        arena[7, 5] = 1;
        var agent = _world.CreateAgent("Agent 1", 2, 3);
        var inferences = agent.Get<Inference>();
        var behaviour = agent.Get<Behaviour>();
        behaviour.Rules.Add((memory) => new []{new MoveDescriptor(){Velocity = new Position(1, 0)}});
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (_ticker.Tick(gameTime))
        {
            _inferenceSystem.Execute();
            _behaviourSystem.Execute();
        } 
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
        _renderingSystem.Execute();
    }

    protected override void OnGui()
    {
        ImGui.Begin("Playground", ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.Text($"Step: {_ticker.Ticks}");
        ImGui.End();
    }
}