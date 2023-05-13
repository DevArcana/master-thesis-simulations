using System;
using System.Collections.Generic;
using System.IO;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace InfoWave.MonoGame.Scenes.Playground;

public class RenderingSystem
{
    private const int TileSize = 16;

    private readonly World _world;
    private readonly SpriteBatch _spriteBatch;

    private readonly Texture2D[] _tiles;
    private readonly SpriteFont _font;

    public RenderingSystem(World world, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _world = world;
        _spriteBatch = spriteBatch;

        _tiles = new[]
        {
            Texture2D.FromFile(graphicsDevice, @"Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0004.png")
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

    public void RenderAgents()
    {
        var query = new QueryDescription().WithAll<Position, Name>();
        _world.Query(in query, (ref Position pos, ref Name name) =>
        {
            _spriteBatch.Draw(
                _tiles[0],
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

    public void RenderArena()
    {
        var query = new QueryDescription().WithAll<Grid>();
        _world.Query(in query, (ref Grid grid) =>
        {
            for (var x = 0; x < grid.Width; x++)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    var tile = grid[x, y];
                    if (tile > 0)
                    {
                        _spriteBatch.Draw(
                            _tiles[0],
                            new Rectangle(
                                TileSize * x,
                                TileSize * y,
                                TileSize,
                                TileSize),
                            Color.White);
                    }
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