namespace InfoWave.MonoGame.Scenes.Playground;

public class Agent : ITickable
{
    public int x, y;
    public void Tick()
    {
        x += 1;
    }
}