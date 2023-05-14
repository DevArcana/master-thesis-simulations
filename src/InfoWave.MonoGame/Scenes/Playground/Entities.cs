using Arch.Core;

namespace InfoWave.MonoGame.Scenes.Playground;

public static class Entities
{
    public static Entity CreateAgent(this World world, string name, int x, int y)
    {
        var agent = world.Create(
            new Name(name),
            new Position(x, y),
            new WorkingMemory(),
            new Inference(),
            new Behaviour());

        Globals.Agents.Add(name);
        
        return agent;
    }

    public static Entity CreateArena(this World world, int width, int height)
    {
        var arena = world.Create(new Grid(width, height));
        return arena;
    }
}