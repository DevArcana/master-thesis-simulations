using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using InfoWave.MonoGame.Common.Utils;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Scenes.Playground;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.DiseaseSpread;

public class DiseaseSpreadScene : PlaygroundScene
{
    public DiseaseSpreadScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base(graphicsDevice, imGuiRenderer)
    {
    }

    protected override void OnCreate()
    {
        Systems.Add(new Playground.System(() =>
        {
            World.Query(in new QueryDescription().WithAll<WorkingMemory, Tile>(),
                (ref WorkingMemory memory, ref Tile tile) =>
                {
                    if (memory.Memory.TryGetValue("infected", out object obj) && (bool)obj)
                    {
                        tile.Index = 1;
                    }
                    else
                    {
                        tile.Index = 0;
                    }
                });
        }));
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
                var visible = (Dictionary<string, bool>)memory["visible"];
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
                    if (sorted.Value.SquaredDistance(position) == 1 && visible[sorted.Key])
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
}