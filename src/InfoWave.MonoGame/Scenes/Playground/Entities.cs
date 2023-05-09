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

        return agent;
    }
}