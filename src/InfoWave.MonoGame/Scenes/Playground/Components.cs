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