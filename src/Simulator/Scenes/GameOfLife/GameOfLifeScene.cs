using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulator.Utils;
using Simulator.Widgets;

namespace Simulator.Scenes.GameOfLife;

public class GameOfLifeScene : Scene
{
    private int _width;
    private int _height;
    private int _cellSize;
    private int _cellGap;
    
    private Grid<bool> _grid;
    private BooleanGridWidget _gridWidget;
    
    public GameOfLifeScene(GraphicsDevice graphicsDevice) : base("Game of Life", graphicsDevice)
    {
        // default value
        _width = 16;
        _height = 16;
        _cellSize = 20;
        _cellGap = 4;
    }

    private void RebuildGrid()
    {
        _grid = new Grid<bool>(_width, _height);
        _gridWidget = new BooleanGridWidget(GraphicsDevice, _grid, Color.White, Color.Gray, _cellSize, _cellGap);
    }

    protected override void OnGui()
    {
        ImGui.Begin("Grid Settings", ImGuiWindowFlags.AlwaysAutoResize);
        
        if (ImGui.InputInt("width", ref _width))
        {
            RebuildGrid();
        }
        
        if (ImGui.InputInt("height", ref _height))
        {
            RebuildGrid();
        }
        
        if (ImGui.InputInt("cell size", ref _cellSize))
        {
            RebuildGrid();
        }
        
        if (ImGui.InputInt("cell gap", ref _cellGap))
        {
            RebuildGrid();
        }

        ImGui.End();
    }

    protected override void OnCreate()
    {
        RebuildGrid();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        
    }

    protected override void OnDestroy()
    {
        
    }

    protected override void OnDraw(GameTime gameTime)
    {
        _gridWidget?.Draw(SpriteBatch);
    }
}