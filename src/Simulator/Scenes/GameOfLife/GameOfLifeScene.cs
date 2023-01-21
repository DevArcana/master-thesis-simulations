using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulator.Scenes.GameOfLife;

public class GameOfLifeScene : Scene
{
    private readonly Texture2D _pixel;
    public GameOfLifeScene(GraphicsDevice graphicsDevice) : base("Game of Life", graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new [] {Color.White});
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
        SpriteBatch.Draw(_pixel, new Rectangle(100, 100, 100, 100), Color.White);
    }
}