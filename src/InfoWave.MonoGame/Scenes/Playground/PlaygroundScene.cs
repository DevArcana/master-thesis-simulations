using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace InfoWave.MonoGame.Scenes.Playground;

public struct Position
{
    public int X;
    public int Y;
}

public class Agent
{
    public string Id;
    public Position Position;

    public Agent(string id)
    {
        Id = id;
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
            (float) TileSize,
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
        _agents.Add(new Agent("bob") { Position = new Position() {X = 1, Y = 1}});
        _agents.Add(new Agent("alice") { Position = new Position() {X = 3, Y = 1}});
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
            agent.Position.X++;
            agent.Position.Y++;
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