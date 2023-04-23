using System;
using System.Collections.Generic;

namespace InfoWave.MonoGame.Scenes.Playground;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public record Operator;
public record OpMove(Direction Direction) : Operator;

public class Brain
{
    // working memory
    private readonly Dictionary<string, object> _workingMemory = new();
    
    public void Feed(string key, object value)
    {
        // you see a wall in front of you
        _workingMemory[key] = value;
    }

    public Operator Decide()
    {
        if (_workingMemory.TryGetValue("position", out var value))
        {
            var position = ((int, int)) value;
            return new OpMove(Direction.Right);
        }
        
        return new Operator();
    }
}

public class Agent : ITickable
{
    public int x, y;

    private readonly Brain _brain = new();

    public void FeedBrain()
    {
        _brain.Feed("position", (x, y));
    }

    public void ProcessBrain()
    {
        
    }

    public void UseBrain()
    {
        
    }

    public void Tick()
    {
        FeedBrain();
        ProcessBrain();
        UseBrain();
        // you decide to go right
        var op = _brain.Decide();
        
        // you send the operator to the server
        switch (op)
        {
            case OpMove move:
                switch (move.Direction)
                {
                    case Direction.Up:
                        y--;
                        break;
                    case Direction.Down:
                        y++;
                        break;
                    case Direction.Left:
                        x--;
                        break;
                    case Direction.Right:
                        x++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                Console.WriteLine("Doing nothing");
                break;
        }
    }
}