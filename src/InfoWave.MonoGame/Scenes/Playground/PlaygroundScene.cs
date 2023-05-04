using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace InfoWave.MonoGame.Scenes.Playground;

// agent - components such as position, health, brain
// brain - memory, rules, operators
// agent - feed memory, decide, get operators

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
    public readonly List<Func<IReadOnlyDictionary<string, object>, IEnumerable<ActionDescriptor>>> BehaviourRules = new();
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

public class Agent
{
    public string Id;
    public Position Position;
    public Brain Brain;

    public Agent(string id)
    {
        Id = id;
        Brain = new Brain();
        
        Brain.InferenceRules.Add(memory =>
        {
            var position = (Position)memory["position"];
            if (position.X == 3)
            {
                memory["reached"] = false;
            }
            else if (position.X == 6)
            {
                memory["reached"] = true;
            }
        });

        Brain.BehaviourRules.Add(memory =>
        {
            var position = (Position)memory["position"];
            var reached = memory.TryGetValue("reached", out var reachedValue) && (bool)reachedValue;

            return new[] { new MoveDescriptor() { Velocity = new Position() { X = reached ? -1 : 1, Y = 0 } } };
        });
    }
}

public class PlaygroundScene : Scene
{
    private const double TickRate = 1.0f;
    private const int TileSize = 16;

    private long _tick = 0;
    private double _timer = 0;

    private readonly List<Agent> _agents;
    private readonly Texture2D[] _tiles;

    private readonly SpriteFont _font;

    public PlaygroundScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
        : base("Playground", graphicsDevice, imGuiRenderer)
    {
        _timer = TickRate;
        _agents = new List<Agent>();
        _tiles = new[]
        {
            Texture2D.FromFile(graphicsDevice, @"Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0004.png")
        };

        var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(@"Assets/Fonts/Inter.ttf"),
            (float)TileSize,
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
    }

    protected override void OnCreate()
    {
        _agents.Add(new Agent("bob") { Position = new Position() { X = 3, Y = 3 } });
        _agents.Add(new Agent("alice") { Position = new Position() { X = 6, Y = 5 } });
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
        foreach (var agent in _agents)
        {
            agent.Brain.Memory["position"] = agent.Position;
            foreach (var descriptor in agent.Brain.Braining())
            {
                switch (descriptor)
                {
                    case MoveDescriptor moveDescriptor:
                        agent.Position.X += moveDescriptor.Velocity.X;
                        agent.Position.Y += moveDescriptor.Velocity.Y;
                        moveDescriptor.OnSuccess(agent.Brain.Memory);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(descriptor));
                }
            }
        }
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
        foreach (var agent in _agents)
        {
            SpriteBatch.Draw(
                _tiles[0],
                new Rectangle(
                    TileSize * agent.Position.X,
                    TileSize * agent.Position.Y,
                    TileSize,
                    TileSize),
                Color.White);

            var size = _font.MeasureString(agent.Id);
            SpriteBatch.DrawString(
                _font,
                agent.Id,
                new Vector2(
                    TileSize * agent.Position.X + (TileSize - size.X) / 2.0f,
                    TileSize * agent.Position.Y - TileSize / 2.0f - size.Y),
                Color.White);
        }
    }

    protected override void OnGui()
    {
        ImGui.Begin("Playground", ImGuiWindowFlags.AlwaysAutoResize);

        ImGui.Text($"Heartbeat: {_timer:F2}s Tick: {_tick}");

        ImGui.End();
    }
}