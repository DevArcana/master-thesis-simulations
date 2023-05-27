using System;
using System.Collections.Generic;
using System.IO;
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
    private int t, s, i, r = 0;
    
    public int RunIndex = 1;
    private string _directory;
    private string FileName => $"{_directory}/{RunIndex}.csv";
    
    public DiseaseSpreadScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer, int runIndex, string directory)
        : base(graphicsDevice, imGuiRenderer)
    {
        RunIndex = runIndex;
        _directory = directory;
        Directory.CreateDirectory(_directory);
    }

    public void GatherDataLine()
    {
        s = 0;
        i = 0;
        r = 0;
            
        World.Query(in new QueryDescription().WithAll<Infection, Tile, WorkingMemory>(),
            (ref Infection infection, ref Tile tile, ref WorkingMemory memory) =>
            {
                switch (infection.Status)
                {
                    case InfectionStatus.Susceptible:
                        memory.Memory["infection"] = "susceptible";
                        tile.Index = 0;
                        s++;
                        break;
                    case InfectionStatus.Infected:
                        infection.Life--;
                        if (infection.Life == 0)
                        {
                            infection.Status = InfectionStatus.Removed;
                            memory.Memory["infection"] = "removed";
                            tile.Index = 2;
                            r++;
                        }
                        else
                        {
                            memory.Memory["infection"] = "infected";
                            tile.Index = 1;
                            i++;
                        }
                        break;
                    case InfectionStatus.Removed:
                        memory.Memory["infection"] = "removed";
                        tile.Index = 2;
                        r++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
            
        using var file = File.Open(FileName, FileMode.Append);
        using var writer = new StreamWriter(file);
        writer.WriteLine($"{t},{s},{i},{r}");

        t++;
    }

    protected override void OnCreate()
    {
        File.WriteAllText(FileName, $"t,s,i,r{Environment.NewLine}");
        
        Systems.Add(new Playground.System(GatherDataLine));
        
        var random = Random.Shared;
        var arena = World.CreateArena(48, 24).Get<Grid>();

        var agents = new HashSet<string>();
        while (agents.Count < 40)
        {
            var i = agents.Count;
            var x = random.Next(1, 47);
            var y = random.Next(1, 23);
            var z = $"{x}:{y}";
            
            if (agents.Contains(z))
            {
                continue;
            }

            agents.Add(z);
            
            var agent = World.CreateAgent($"Agent {i}", x, y);
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
                if (memory.ContainsKey("collided"))
                {
                    return new Descriptor[] { };
                }

                var position = (Position)memory["position"];
                var positions = (Dictionary<string, Position>)memory["positions"];
                var visible = (Dictionary<string, bool>)memory["visible"];
                var infected = (Dictionary<string, string>)memory["infected"];
                var visited = memory.GetOr("visited", () => new HashSet<string>());
                // find closest agent not yet visited
                Enum.TryParse<InfectionStatus>(infected[(string)memory["name"]], true, out var status);

                if (status == InfectionStatus.Removed)
                {
                    return new Descriptor[] { };
                }

                if (status == InfectionStatus.Susceptible)
                {
                    return new[]
                    {
                        new MoveDescriptor() {Velocity = new Position(random.Next(-1, 2), random.Next(-1, 2))}
                    };
                }
                
                var sorted = positions
                    .Where(x =>
                        !visited.Contains(x.Key)
                        && infected.TryGetValue(x.Key, out var inf) && inf == "susceptible")
                    .OrderBy(x => x.Value.SquaredDistance(position))
                    .FirstOrDefault();

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (sorted.Key is not null)
                {
                    if (sorted.Value.SquaredDistance(position) == 1 && visible[sorted.Key])
                    {
                        return new[]
                        {
                            new InfectDescriptor(sorted.Value - position)
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
                agent.Get<Infection>().Status = InfectionStatus.Infected;
            }
        }
    }

    protected override bool ShouldUpdate()
    {
        var survivors = false;
        World.Query(in new QueryDescription()
            .WithAll<Infection>(),
            (ref Infection infection) =>
            {
                if (infection.Status == InfectionStatus.Susceptible)
                {
                    survivors = true;
                }
            });
        return survivors;
    }
}