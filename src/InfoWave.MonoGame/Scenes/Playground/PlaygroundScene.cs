using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using ImGuiNET;
using InfoWave.MonoGame.Common.Utils;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.Playground;

public class Ticker
{
    private long _tick = 0;
    private double _timer = 0;

    private const double TickRate = 0.01f;

    public long Ticks => _tick;

    public Ticker()
    {
        _timer = TickRate;
    }
    
    public void Advance()
    {
        _timer = 0;
    }

    public void Advance(GameTime gameTime)
    {
        _timer -= gameTime.ElapsedGameTime.TotalSeconds;
    }

    public bool Tick()
    {
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
    private bool _paused = true;
    protected readonly World World = World.Create();

    // Systems
    private readonly RenderingSystem _renderingSystem;
    private readonly InferenceSystem _inferenceSystem;
    private readonly BehaviourSystem _behaviourSystem;
    private readonly SensorSystem _sensorSystem;
    protected readonly List<ISystem> Systems = new();

    public PlaygroundScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base("Playground", graphicsDevice, imGuiRenderer)
    {
        _renderingSystem = new RenderingSystem(World, graphicsDevice, SpriteBatch);
        _inferenceSystem = new InferenceSystem(World);
        _behaviourSystem = new BehaviourSystem(World);
        _sensorSystem = new SensorSystem(World);
    }

    protected override void OnCreate()
    {
        var arena = World.CreateArena(48, 24).Get<Grid>();
        arena[7, 2] = 1;
        arena[7, 3] = 1;
        arena[7, 4] = 1;
        arena[7, 5] = 1;

        for (var i = 0; i < 6; i++)
        {
            var agent = World.CreateAgent($"Agent {i}", 2 + i * 2, 3 + i * 4);
            var inferences = agent.Get<Inference>();
            inferences.Rules.Add((memory) =>
            {
                var messages = memory.GetOr("messages", () => new List<string>());
                if (messages.Any())
                {
                    messages.Clear();
                    memory["infected"] = true;
                }
            });
            var behaviour = agent.Get<Behaviour>();
            behaviour.Rules.Add((memory) =>
            {
                if (memory.TryGetValue("collided", out object obj))
                {
                    var collided = (Position)obj;
                    var direction = collided;

                    while (collided == direction)
                    {
                        direction = new Position(
                            Random.Shared.Next(-1, 2),
                            Random.Shared.Next(-1, 2)
                        );
                    }

                    return new[]
                    {
                        new MoveDescriptor()
                        {
                            Velocity = direction,
                            OnSuccess = (memory) => { memory.Remove("collided"); },
                            OnReject = (memory) => { memory["collided"] = direction; }
                        }
                    };
                }

                return new Descriptor[] { };
            });
            behaviour.Rules.Add((memory) =>
            {
                if (!memory.ContainsKey("infected") || memory.ContainsKey("collided"))
                {
                    return new Descriptor[] { };
                }

                var position = (Position)memory["position"];
                var positions = (Dictionary<string, Position>)memory["positions"];
                var visited = memory.GetOr("visited", () => new HashSet<string>());
                var told = memory.GetOr("told", () => new HashSet<string>());
                // find closest agent not yet visited

                var sorted = positions
                    .Where(x =>
                        !visited.Contains(x.Key)
                        && !told.Contains(x.Key))
                    .OrderBy(x => x.Value.SquaredDistance(position))
                    .FirstOrDefault();

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (sorted.Key is not null)
                {
                    if (sorted.Value.SquaredDistance(position) == 1)
                    {
                        return new[]
                        {
                            new TellDescriptor(sorted.Value - position, "message")
                            {
                                OnSuccess = (_ => { told.Add(sorted.Key); })
                            }
                        };
                    }
                    else
                    {
                        return new[]
                        {
                            new MoveDescriptor()
                            {
                                Velocity = (sorted.Value - position).Capped(1),
                                OnReject = (memory) => { memory["collided"] = (sorted.Value - position).Capped(1); }
                            }
                        };
                    }
                }

                return new Descriptor[] { };
            });

            if (i == 3)
            {
                agent.Get<WorkingMemory>().Memory["infected"] = true;
            }
        }
    }

    protected virtual bool ShouldUpdate() => true;
    
    protected override void OnUpdate(GameTime gameTime)
    {
        if (!_paused && ShouldUpdate())
        {
            _ticker.Advance(gameTime);
        }
        
        if (_ticker.Tick())
        {
            _sensorSystem.Execute();
            _inferenceSystem.Execute();
            _behaviourSystem.Execute();
            foreach (var system in Systems)
            {
                system.Execute();
            }
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
        if (ImGui.Button(_paused ? "Play" : "Pause"))
        {
            _paused = !_paused;
        }

        if (ImGui.Button("Next"))
        {
            _ticker.Advance();
        }
        ImGui.Text($"Step: {_ticker.Ticks}");
        World.Query(new QueryDescription().WithAll<WorkingMemory, Name>(), (ref WorkingMemory memory, ref Name name) =>
        {
            var infected = memory.Memory.ContainsKey("infected");
            ImGui.Text("Agent: " + name.Value + " " + (infected ? "Infected" : "Healthy"));
        });
        ImGui.End();
    }
}