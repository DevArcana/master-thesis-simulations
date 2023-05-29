using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.Playground;

public static class Entities
{
    public static Entity CreateAgent(this World world, string name, int x, int y)
    {
        var agent = world.Create(
            new Name(name),
            new Tile(0),
            new Position(x, y),
            new Sight(Simulator.Sight),
            new Infection()
            {
                Status = InfectionStatus.Susceptible, Life = Simulator.Lifetime
            },
            new WorkingMemory(),
            new Inference(),
            new Behaviour());

        return agent;
    }

    public static Entity CreateArena(this World world, int width, int height)
    {
        var arena = world.Create(new Grid(width, height));
        return arena;
    }

    public static Entity CreateArena(this World world, GraphicsDevice graphicsDevice, string filename)
    {
        var texture = Texture2D.FromFile(graphicsDevice, filename);
        var width = texture.Width;
        var height = texture.Height;
        var pixelData = new Color[width * height];
        
        texture.GetData(pixelData);

        var arena = new Grid(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixelColor = pixelData[x + y * width];
                if (pixelColor == Color.White)
                {
                    arena[x, y] = 1;
                }
                else
                {
                    arena[x, y] = 0;
                }
            }
        }
        
        return world.Create(arena);
    }
}