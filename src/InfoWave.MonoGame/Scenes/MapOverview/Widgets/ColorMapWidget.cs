using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulator.Widgets;

namespace Simulator.Scenes.MapOverview.Widgets;

public class ColorMapWidget : GridWidget
{
    private readonly Texture2D _color;
    
    public ColorMapWidget(Texture2D color, int scale) : base("Color Map", scale, 0)
    {
        _color = color;
    }

    protected override void OnDraw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Draw(_color, new Rectangle(0, 0, _color.Width * CellSize, _color.Height * CellSize), Color.White);
        base.OnDraw(graphicsDevice, spriteBatch, gameTime);
    }
}