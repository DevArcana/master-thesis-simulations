using InfoWave.MonoGame.Common.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.MapOverview.Widgets;

public class ColorMapSceneLayer : GridSceneLayer
{
    private readonly Texture2D _color;
    
    public ColorMapSceneLayer(Texture2D color, int scale) : base("Color Map", scale, 0)
    {
        _color = color;
    }

    protected override void OnDraw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Draw(_color, new Rectangle(0, 0, _color.Width * CellSize, _color.Height * CellSize), Color.White);
        base.OnDraw(graphicsDevice, spriteBatch, gameTime);
    }
}