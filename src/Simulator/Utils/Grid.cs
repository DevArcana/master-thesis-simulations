namespace Simulator.Utils;

public class Grid<T>
{
    public readonly int Width;
    public readonly int Height;

    private readonly T[,] _data;

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;

        _data = new T[width, height];
        Clear();
    }

    public void Clear()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                _data[x, y] = default;
            }
        }
    }

    public T this[int x, int y]
    {
        get => _data[x, y];
        set => _data[x, y] = value;
    }
}