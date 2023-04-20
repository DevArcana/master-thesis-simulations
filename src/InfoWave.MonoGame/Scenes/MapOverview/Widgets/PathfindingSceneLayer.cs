using InfoWave.MonoGame.Common.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InfoWave.MonoGame.Scenes.MapOverview.Widgets;

public class PathfindingSceneLayer : GridSceneLayer
{
    private readonly Texture2D _pixel;

    private readonly Color _fromColor = Color.Red;
    private readonly Color _toColor = Color.Blue;

    private Vector2? _from;
    private Vector2? _to;

    private bool _selectingFrom;
    private bool _selectingTo;

    public PathfindingSceneLayer(GraphicsDevice graphicsDevice, int scale) : base("Pathfinding", scale, 0)
    {
        _pixel = new Texture2D(graphicsDevice, scale, scale);
        var data = new Color[scale * scale];
        for (var i = 0; i < scale * scale; i++)
        {
            data[i] = Color.White;
        }
        _pixel.SetData(data);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);
        
        var mouse = Mouse.GetState();

        switch (mouse.LeftButton)
        {
            case ButtonState.Pressed:
                _selectingFrom = true;
                break;
            case ButtonState.Released when _selectingFrom:
            {
                _selectingFrom = false;
                var pos = new Vector2(ToCellCoord(mouse.X), ToCellCoord(mouse.Y));
                if (_from == pos)
                {
                    _from = null;
                }
                else
                {
                    _from = pos;
                }

                break;
            }
        }
        
        switch (mouse.RightButton)
        {
            case ButtonState.Pressed:
                _selectingTo = true;
                break;
            case ButtonState.Released when _selectingTo:
            {
                _selectingTo = false;
                var pos = new Vector2(ToCellCoord(mouse.X), ToCellCoord(mouse.Y));
                if (_to == pos)
                {
                    _to = null;
                }
                else
                {
                    _to = pos;
                }

                break;
            }
        }
    }

    protected override void OnDraw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.OnDraw(graphicsDevice, spriteBatch, gameTime);

        var mouse = Mouse.GetState();
        var highlightColor = mouse.LeftButton == ButtonState.Pressed
            ? _fromColor
            : mouse.RightButton == ButtonState.Pressed
                ? _toColor
                : Color.White;

        spriteBatch.Draw(_pixel, HoverCellScreenPos, highlightColor);

        if (_from.HasValue)
        {
            spriteBatch.Draw(_pixel, ToScreenPos(_from.Value), _fromColor);
        }
        
        if (_to.HasValue)
        {
            spriteBatch.Draw(_pixel, ToScreenPos(_to.Value), _toColor);
        }
    }
}