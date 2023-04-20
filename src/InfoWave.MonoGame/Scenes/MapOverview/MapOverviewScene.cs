using System.IO;
using System.Linq;
using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using InfoWave.MonoGame.Scenes.MapOverview.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.MapOverview;

public class MapOverviewScene : Scene
{
    private readonly string[] _maps;
    private int _selectedMap;
    private const int Scale = 10;

    private SceneLayer _colorMapSceneLayer;
    private SceneLayer _costMapSceneLayer;
    private SceneLayer _pathfindingSceneLayer;

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
        _colorMapSceneLayer = new ColorMapSceneLayer(color, Scale);
        _costMapSceneLayer = new CostMapSceneLayer(Scale);
        _pathfindingSceneLayer = new PathfindingSceneLayer(GraphicsDevice, Scale);
    }

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _colorMapSceneLayer?.Update(gameTime);
        _costMapSceneLayer?.Update(gameTime);
        _pathfindingSceneLayer?.Update(gameTime);
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
        _colorMapSceneLayer?.Draw(GraphicsDevice, SpriteBatch, gameTime);
        _costMapSceneLayer?.Draw(GraphicsDevice, SpriteBatch, gameTime);
        _pathfindingSceneLayer?.Draw(GraphicsDevice, SpriteBatch, gameTime);
    }

    protected override void OnGui()
    {
        ImGui.Begin("Select map", ImGuiWindowFlags.AlwaysAutoResize);
        
        if (ImGui.Combo("maps", ref _selectedMap, _maps, _maps.Length))
        {
            ReloadMapFromDisk();
        }

        _colorMapSceneLayer?.UpdateGui();
        _costMapSceneLayer?.UpdateGui();
        _pathfindingSceneLayer?.UpdateGui();

        ImGui.End();
    }
}