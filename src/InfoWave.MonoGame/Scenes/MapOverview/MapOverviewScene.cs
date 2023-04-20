using System.IO;
using System.Linq;
using ImGuiNET;
using InfoWave.MonoGame.Gui;
using InfoWave.MonoGame.Scenes.MapOverview.Widgets;
using InfoWave.MonoGame.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.MapOverview;

public class MapOverviewScene : Scene
{
    private readonly string[] _maps;
    private int _selectedMap;
    private const int Scale = 10;

    private Widget _colorMapWidget;
    private Widget _costMapWidget;
    private Widget _pathfindingWidget;

    public MapOverviewScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer) : base("Map Overview",
        graphicsDevice, imGuiRenderer)
    {
        _maps = Directory
            .EnumerateFiles("Images")
            .Where(x => x.StartsWith(Path.Join("Images", "map")) && x.EndsWith("color.png"))
            .Select(x => x[7..12])
            .ToArray();

        _selectedMap = 0;
        ReloadMapFromDisk();
    }

    private void ReloadMapFromDisk()
    {
        var map = _maps[_selectedMap];
        var color = Texture2D.FromFile(GraphicsDevice, Path.Join("Images", $"{map}_color.png"));
        _colorMapWidget = new ColorMapWidget(color, Scale);
        _costMapWidget = new CostMapWidget(Scale);
        _pathfindingWidget = new PathfindingWidget(GraphicsDevice, Scale);
    }

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _colorMapWidget?.Update(gameTime);
        _costMapWidget?.Update(gameTime);
        _pathfindingWidget?.Update(gameTime);
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
        _colorMapWidget?.Draw(GraphicsDevice, SpriteBatch, gameTime);
        _costMapWidget?.Draw(GraphicsDevice, SpriteBatch, gameTime);
        _pathfindingWidget?.Draw(GraphicsDevice, SpriteBatch, gameTime);
    }

    protected override void OnGui()
    {
        ImGui.Begin("Select map", ImGuiWindowFlags.AlwaysAutoResize);
        
        if (ImGui.Combo("maps", ref _selectedMap, _maps, _maps.Length))
        {
            ReloadMapFromDisk();
        }

        _colorMapWidget?.UpdateGui();
        _costMapWidget?.UpdateGui();
        _pathfindingWidget?.UpdateGui();

        ImGui.End();
    }
}