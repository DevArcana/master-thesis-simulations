using System;
using System.Collections.Generic;
using System.IO;
using Arch.Core;
using Arch.Core.Extensions;
using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace InfoWave.MonoGame.Scenes.Playground;

public class ActionDescriptor
{
    public Action<Dictionary<string, object>> OnSuccess = _ => { };
    public Action<Dictionary<string, object>> OnReject = _ => { };
}

public class MoveDescriptor : ActionDescriptor
{
    public Position Velocity;
}

public class Brain
{
    public readonly Dictionary<string, object> Memory = new();

    public readonly List<Func<IReadOnlyDictionary<string, object>, IEnumerable<ActionDescriptor>>> BehaviourRules =
        new();

    public readonly List<Action<Dictionary<string, object>>> InferenceRules = new();

    public IEnumerable<ActionDescriptor> Braining()
    {
        foreach (var rule in InferenceRules)
        {
            rule(Memory);
        }

        var actions = new List<ActionDescriptor>();

        foreach (var rule in BehaviourRules)
        {
            actions.AddRange(rule(Memory));
        }

        return actions;
    }
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Name
{
    public string Value;
}

public class PlaygroundScene : Scene
{
    private const double TickRate = 1.0f;
    private const int TileSize = 16;

    private long _tick = 0;
    private double _timer = 0;

    private readonly World _world;

    private readonly Texture2D[] _tiles;

    private readonly SpriteFont _font;

    public PlaygroundScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base("Playground", graphicsDevice, imGuiRenderer)
    {
        _timer = TickRate;
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

        _font = fontBakeResult.CreateSpriteFont(GraphicsDevice);

        _world = World.Create();
    }

    protected override void OnCreate()
    {
        var brain = new Brain();
        brain.BehaviourRules.Add((memory) =>
        {
            return new []{new MoveDescriptor() { Velocity = new Position() { X = 1, Y = 0 } }};
        });
        _world.Create(
            new Position { X = 1, Y = 2 },
            new Name { Value = "Agent 1" },
            brain
        );
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _timer -= gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer <= 0.0f)
        {
            _timer = TickRate;
            _tick++;
            OnTick();
        }
    }

    private void OnTick()
    {
        var query = new QueryDescription().WithAll<Position, Brain>();
        _world.Query(in query, (in Entity entity) =>
        {
            ref var brain = ref entity.Get<Brain>();
            var actions = brain.Braining();
            foreach (var action in actions)
            {
                if (action is MoveDescriptor move)
                {
                    ref var position = ref entity.Get<Position>();
                    position.X += move.Velocity.X;
                    position.Y += move.Velocity.Y;
                }
            }
        });
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
        var query = new QueryDescription().WithAll<Position, Name>();
        _world.Query(in query, (ref Position pos, ref Name name) =>
        {
            SpriteBatch.Draw(
                _tiles[0],
                new Rectangle(
                    TileSize * pos.X,
                    TileSize * pos.Y,
                    TileSize,
                    TileSize),
                Color.White);

            var size = _font.MeasureString(name.Value);
            SpriteBatch.DrawString(
                _font,
                name.Value,
                new Vector2(
                    TileSize * pos.X + (TileSize - size.X) / 2.0f,
                    TileSize * pos.Y - TileSize / 2.0f - size.Y),
                Color.White);
        });
    }

    protected override void OnGui()
    {
        ImGui.Begin("Playground", ImGuiWindowFlags.AlwaysAutoResize);

        ImGui.Text($"Heartbeat: {_timer:F2}s Tick: {_tick}");

        ImGui.End();
    }
}