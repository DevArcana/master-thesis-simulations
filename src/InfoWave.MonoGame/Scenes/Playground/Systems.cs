using System;
using System.Collections.Generic;
using System.IO;
using Arch.Core;
using Arch.Core.Extensions;
using InfoWave.MonoGame.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace InfoWave.MonoGame.Scenes.Playground;

public interface ISystem
{
    void Execute();
}

public class System : ISystem
{
    private readonly Action _action;

    public System(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}

public class RenderingSystem
{
    private const int TileSize = 12;

    private readonly World _world;
    private readonly SpriteBatch _spriteBatch;

    private readonly Texture2D[] _tiles;
    private readonly Texture2D[] _walls;
    private readonly SpriteFont _font;

    public RenderingSystem(World world, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _world = world;
        _spriteBatch = spriteBatch;

        _tiles = new[]
        {
            Texture2D.FromFile(graphicsDevice, @"Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0004.png"),
            Texture2D.FromFile(graphicsDevice, @"Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0005.png"),
            Texture2D.FromFile(graphicsDevice, @"Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0006.png"),
        };

        _walls = new[]
        {
            Texture2D.FromFile(graphicsDevice, @"Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0017.png"),
            Texture2D.FromFile(graphicsDevice, @"Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0145.png"),
        };

        var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(@"Assets/Fonts/Inter.ttf"),
            TileSize,
            1024,
            1024,
            new[]
            {
                CharacterRange.BasicLatin,
                CharacterRange.Latin1Supplement,
                CharacterRange.LatinExtendedA,
                CharacterRange.Cyrillic
            }
        );

        _font = fontBakeResult.CreateSpriteFont(graphicsDevice);
    }

    private void RenderAgents()
    {
        var query = new QueryDescription().WithAll<Position, Name, Tile>();
        _world.Query(in query, (ref Position pos, ref Name name, ref Tile tile) =>
        {
            _spriteBatch.Draw(
                _tiles[tile.Index],
                new Rectangle(
                    TileSize * pos.X,
                    TileSize * pos.Y,
                    TileSize,
                    TileSize),
                Color.White);

            var size = _font.MeasureString(name.Value);
            _spriteBatch.DrawString(
                _font,
                name.Value,
                new Vector2(
                    TileSize * pos.X + (TileSize - size.X) / 2.0f,
                    TileSize * pos.Y - TileSize / 2.0f - size.Y),
                Color.White);
        });
    }

    private void RenderArena()
    {
        var query = new QueryDescription().WithAll<Grid>();
        _world.Query(in query, (ref Grid grid) =>
        {
            for (var x = 0; x < grid.Width; x++)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    var tile = grid[x, y];
                    _spriteBatch.Draw(
                        _walls[tile],
                        new Rectangle(
                            TileSize * x,
                            TileSize * y,
                            TileSize,
                            TileSize),
                        Color.White);
                }
            }
        });
    }

    public void Execute()
    {
        RenderArena();
        RenderAgents();
    }
}

public class InferenceSystem
{
    private readonly World _world;

    public InferenceSystem(World world)
    {
        _world = world;
    }

    public void Execute()
    {
        var query = new QueryDescription().WithAll<WorkingMemory, Inference>();
        _world.Query(in query, (ref WorkingMemory workingMemory, ref Inference inference) =>
        {
            foreach (var rule in inference.Rules)
            {
                rule(workingMemory.Memory);
            }
        });
    }
}

public class BehaviourSystem
{
    private readonly World _world;

    public BehaviourSystem(World world)
    {
        _world = world;
    }

    public void Execute()
    {
        var query = new QueryDescription().WithAll<WorkingMemory, Behaviour>();
        _world.Query(in query, (in Entity entity) =>
        {
            ref var memory = ref entity.Get<WorkingMemory>();
            ref var behaviour = ref entity.Get<Behaviour>();

            var operators = new List<Descriptor>();
            foreach (var rule in behaviour.Rules)
            {
                operators.AddRange(rule(memory.Memory));
            }

            foreach (var op in operators)
            {
                if (op.Execute(entity, _world))
                {
                    op.OnSuccess(memory.Memory);
                    break;
                }
                else
                {
                    op.OnReject(memory.Memory);
                }
            }
        });
    }
}

public class SensorSystem
{
    private readonly World _world;

    public SensorSystem(World world)
    {
        _world = world;
    }

    public void Execute()
    {
        _world.Query(in new QueryDescription()
                .WithAll<WorkingMemory, Name>(),
            (ref WorkingMemory workingMemory, ref Name name) => { workingMemory.Memory["name"] = name.Value; });

        _world.Query(in new QueryDescription()
                .WithAll<WorkingMemory, Position>(),
            (ref WorkingMemory workingMemory, ref Position position) => { workingMemory.Memory["position"] = position; });
        
        _world.Query(in new QueryDescription()
                .WithAll<WorkingMemory, Name, Sight, Position>(),
            (in Entity entity) =>
            {
                var memory = entity.Get<WorkingMemory>().Memory;
                var name = entity.Get<Name>();
                var position = entity.Get<Position>();
                var sight = entity.Get<Sight>();

                var positions = memory.GetOr("positions", () => new Dictionary<string, Position>());
                var visible = memory.GetOr("visible", () => new Dictionary<string, bool>());
                var infected = memory.GetOr("infected", () => new Dictionary<string, string>());
                var range = sight.Range * sight.Range; 
                _world.Query(in new QueryDescription()
                        .WithAll<Name, Position, Infection>(),
                    (ref Name otherName, ref Position otherPosition, ref Infection infection) =>
                    {
                        if (otherName.Value == name.Value)
                        {
                            infected[otherName.Value] = infection.Status.ToString().ToLower();
                            return;
                        }
                        
                        if (otherPosition.SquaredDistance(position) < range)
                        {
                            positions[otherName.Value] = otherPosition;
                            visible[otherName.Value] = true;
                            infected[otherName.Value] = infection.Status.ToString().ToLower();
                        }
                        else
                        {
                            visible[otherName.Value] = false;
                        }
                    });
            });
        
        // _world.Query(in new QueryDescription()
        //     .WithAll<WorkingMemory, Name, Sight>(),
        //     (ref WorkingMemory workingMemory, ref Name self, ref Sight sight) =>
        // {
        //     var memory = workingMemory.Memory;
        //     memory["name"] = self.Value;
        //     
        //     var positions = memory.GetOr("positions", () => new Dictionary<string, Position>());
        //     memory["positions"] = positions;
        //     
        //     var selfName = self.Value;
        //     _world.Query(in new QueryDescription()
        //         .WithAll<Name, Position>(),
        //         (ref Name name, ref Position position) =>
        //     {
        //         if (name.Value == selfName)
        //         {
        //             memory["position"] = position;
        //         }
        //         else if ()
        //         {
        //             positions[name.Value] = position;
        //         }
        //     });
        // });
    }
}