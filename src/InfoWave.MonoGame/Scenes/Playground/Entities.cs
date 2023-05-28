using Arch.Core;

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
}