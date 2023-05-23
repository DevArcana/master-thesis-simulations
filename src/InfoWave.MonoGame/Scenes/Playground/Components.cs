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
    public readonly List<Func<Dictionary<string, object>, IEnumerable<Descriptor>>> Rules;

    public Behaviour()
    {
        Rules = new List<Func<Dictionary<string, object>, IEnumerable<Descriptor>>>();
    }
}

public struct Tile
{
    public int Index;
    
    public Tile(int index)
    {
        Index = index;
    }
}

public struct Sight
{
    public int Range;
    
    public Sight(int range)
    {
        Range = range;
    }
}

public enum InfectionStatus
{
    Susceptible,
    Infected,
    Removed
}

public struct Infection
{
    public InfectionStatus Status;
    public int Life;
}

public struct Position : IEquatable<Position>
{
    public int X;
    public int Y;

    public int PrevX;
    public int PrevY;

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Position(int x, int y, int prevX, int prevY)
    {
        X = x;
        Y = y;
        PrevX = prevX;
        PrevY = prevY;
    }
    
    public static Position operator +(Position a, Position b)
    {
        return new Position(a.X + b.X, a.Y + b.Y, a.X, a.Y);
    }
    
    public static Position operator -(Position a, Position b)
    {
        return new Position(a.X - b.X, a.Y - b.Y, a.X, a.Y);
    }
    
    public int SquaredDistance(Position other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        return dx * dx + dy * dy;
    }
    
    public Position Capped(int max)
    {
        return new Position(Math.Clamp(X, -max, max), Math.Clamp(Y, -max, max));
    }

    public bool Equals(Position other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !left.Equals(right);
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