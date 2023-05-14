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

    private const double TickRate = 0.1f;

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
    private readonly SensorSystem _sensorSystem;

    public PlaygroundScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base("Playground", graphicsDevice, imGuiRenderer)
    {
        _renderingSystem = new RenderingSystem(_world, graphicsDevice, SpriteBatch);
        _inferenceSystem = new InferenceSystem(_world);
        _behaviourSystem = new BehaviourSystem(_world);
        _sensorSystem = new SensorSystem(_world);
    }

    protected override void OnCreate()
    {
        var arena = _world.CreateArena(48, 24).Get<Grid>();
        arena[7, 2] = 1;
        arena[7, 3] = 1;
        arena[7, 4] = 1;
        arena[7, 5] = 1;

        for (var i = 0; i < 6; i++)
        {
            var agent = _world.CreateAgent($"Agent {i}", 2 + i * 2, 3 + i * 4);
            var inferences = agent.Get<Inference>();
            inferences.Rules.Add((memory) =>
            {
                var messages = memory.GetOr("messages", () => new List<string>());
                if (messages.Any())
                {
                    messages.Clear();
                    memory["infected"] = true;
                    Globals.Infected.Add((string) memory["name"]);
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

    protected override void OnUpdate(GameTime gameTime)
    {
        if (_ticker.Tick(gameTime))
        {
            _sensorSystem.Execute();
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
        foreach (var agent in Globals.Agents)
        {
            ImGui.Text("Agent: " + agent + " " + (Globals.Infected.Contains(agent) ? "Infected" : "Healthy"));
        }
        ImGui.End();
    }
}