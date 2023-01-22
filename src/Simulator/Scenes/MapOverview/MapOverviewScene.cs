using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulator.Gui;
using Vector2 = System.Numerics.Vector2;

namespace Simulator.Scenes.MapOverview;

public class MapOverviewScene : Scene
{
    private readonly string[] _maps;
    private int _selectedMap;
    private Texture2D _color;
    private IntPtr _colorIntPtr;
    
    public MapOverviewScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer) : base("Map Overview", graphicsDevice, imGuiRenderer)
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
        _color = Texture2D.FromFile(GraphicsDevice, Path.Join("Images", $"{map}_color.png"));
        _colorIntPtr = ImGuiRenderer.BindTexture(_color);
    }

    protected override void OnCreate()
    {
        
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        
    }

    protected override void OnDestroy()
    {
        
    }

    protected override void OnDraw(GameTime gameTime)
    {
        if (_color is not null)
        {
            SpriteBatch.Draw(_color, new Rectangle(0, 0, _color.Width * 6, _color.Height * 6), Color.White);
        }
    }

    protected override void OnGui()
    {
        ImGui.Begin("Select map", ImGuiWindowFlags.AlwaysAutoResize);
        if (ImGui.Combo("maps", ref _selectedMap, _maps, _maps.Length))
        {
            ReloadMapFromDisk();
        }

        if (_color is not null)
        {
            ImGui.Image(_colorIntPtr, new Vector2(200, 200));
        }
        ImGui.End();
    }
}