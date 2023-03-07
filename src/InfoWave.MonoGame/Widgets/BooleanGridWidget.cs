using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulator.Utils;

namespace Simulator.Widgets;

public class BooleanGridWidget
{
    private readonly Texture2D _pixel;
    private readonly Grid<bool> _grid;
    private readonly Color _trueColor;
    private readonly Color _falseColor;
    private readonly int _cellSize;
    private readonly int _cellGap;

    public BooleanGridWidget(
        GraphicsDevice gfx,
        Grid<bool> grid,
        Color trueColor,
        Color falseColor,
        int cellSize,
        int cellGap)
    {
        _grid = grid;
        _trueColor = trueColor;
        _falseColor = falseColor;
        _cellSize = cellSize;
        _cellGap = cellGap;
        _pixel = new Texture2D(gfx, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    private bool _clicking;
    public int ClickX;
    public int ClickY;
    public bool Clicked()
    {
        var state = Mouse.GetState();

        if (state.LeftButton == ButtonState.Pressed)
        {
            _clicking = true;
        }
        else if (state.LeftButton == ButtonState.Released && _clicking)
        {
            _clicking = false;
            var unit = _cellGap + _cellSize;
            ClickX = state.X / unit;
            ClickY = state.Y / unit;
            return true;
        }

        return false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var unit = _cellSize + _cellGap;
        for (var x = 0; x < _grid.Width; x++)
        {
            for (var y = 0; y < _grid.Height; y++)
            {
                spriteBatch.Draw(_pixel,
                    new Rectangle(x * unit, y * unit, _cellSize, _cellSize),
                    _grid[x, y] ? _trueColor : _falseColor);
            }
        }
    }
}