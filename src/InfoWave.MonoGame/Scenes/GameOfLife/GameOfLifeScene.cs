using ImGuiNET;
using InfoWave.MonoGame.Common.Layers;
using InfoWave.MonoGame.Common.Utils;
using InfoWave.MonoGame.Core.Gui;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.GameOfLife;

public class GameOfLifeScene : Scene
{
    private int _width;
    private int _height;
    private int _cellSize;
    private int _cellGap;
    private bool _simulating;
    private int _step;

    private Grid<bool> _grid;
    private BooleanGridSceneLayer _gridSceneLayer;

    public GameOfLifeScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer) : base("Game of Life", graphicsDevice, imGuiRenderer)
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
        _gridSceneLayer = new BooleanGridSceneLayer(GraphicsDevice, _grid, Color.White, Color.Gray, _cellSize, _cellGap);
        _step = 0;
    }

    private void Simulate()
    {
        _step++;
        var clone = _grid.Clone();
        
        for (var x = 0; x < _grid.Width; x++)
        {
            for (var y = 0; y < _grid.Height; y++)
            {
                var neighbours = 0;
                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        if (clone[x + dx, y + dy])
                        {
                            neighbours++;
                        }
                    }
                }

                if (clone[x, y])
                {
                    switch (neighbours)
                    {
                        case < 2:
                        case > 3:
                            _grid[x, y] = false;
                            break;
                    }
                }
                else if (neighbours == 3)
                {
                    _grid[x, y] = true;
                }
            }
        }
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
        
        ImGui.Text($"Step: {_step}");

        if (ImGui.Button("Next Step"))
        {
            Simulate();
        }

        if (ImGui.Button(_simulating ? "Stop" : "Start"))
        {
            _simulating = !_simulating;
        }

        ImGui.End();
    }

    protected override void OnCreate()
    {
        RebuildGrid();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (_gridSceneLayer.Clicked())
        {
            if (_gridSceneLayer.ClickX >= 0 &&
                _gridSceneLayer.ClickX < _grid.Width &&
                _gridSceneLayer.ClickY >= 0 &&
                _gridSceneLayer.ClickY < _grid.Height)
            {
                _grid[_gridSceneLayer.ClickX, _gridSceneLayer.ClickY] = !_grid[_gridSceneLayer.ClickX, _gridSceneLayer.ClickY];
            }
        }

        if (_simulating)
        {
            Simulate();
        }
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
        _gridSceneLayer?.Draw(SpriteBatch);
    }
}