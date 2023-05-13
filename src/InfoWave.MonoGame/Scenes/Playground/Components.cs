using System;
using System.Collections.Generic;

namespace InfoWave.MonoGame.Scenes.Playground;

public class WorkingMemory
{
    public readonly Dictionary<string, object> Memory;

    public WorkingMemory()
    {
        Memory = new Dictionary<string, object>();
    }
}

public class Inference
{
    public readonly List<Action<Dictionary<string, object>>> Rules;

    public Inference()
    {
        Rules = new List<Action<Dictionary<string, object>>>();
    }
}

public class Behaviour
{
    public readonly List<Func<IReadOnlyDictionary<string, object>, IEnumerable<Descriptor>>> Rules;

    public Behaviour()
    {
        Rules = new List<Func<IReadOnlyDictionary<string, object>, IEnumerable<Descriptor>>>();
    }
}

public struct Position
{
    public int X;
    public int Y;

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public readonly struct Name
{
    public readonly string Value;

    public Name(string value)
    {
        Value = value;
    }
}

public class Grid
{
    public readonly int Width, Height;
    private readonly int[,] _tiles;

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new int[width, height];
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsInBounds(Position pos)
    {
        return IsInBounds(pos.X, pos.Y);
    }

    public int this[int x, int y]
    {
        get => IsInBounds(x, y) ? _tiles[x, y] : 1;
        set
        {
            if (IsInBounds(x, y))
            {
                _tiles[x, y] = value;
            }
        }
    }
}