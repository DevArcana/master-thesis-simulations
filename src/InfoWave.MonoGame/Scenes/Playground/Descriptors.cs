using System;
using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using InfoWave.MonoGame.Common.Utils;

namespace InfoWave.MonoGame.Scenes.Playground;

public class Descriptor
{
    public Action<Dictionary<string, object>> OnSuccess = _ => { };
    public Action<Dictionary<string, object>> OnReject = _ => { };
    public virtual bool Execute(Entity entity, World world) => true;
}

public class MoveDescriptor : Descriptor
{
    public Position Velocity;

    public override bool Execute(Entity entity, World world)
    {
        ref var position = ref entity.Get<Position>();
        var x = position.X + Velocity.X;
        var y = position.Y + Velocity.Y;
        position.PrevX = position.X;
        position.PrevY = position.Y;
        var collided = false;
        world.Query(in new QueryDescription().WithAll<Position>(), (ref Position pos) =>
        {
            if (pos.X == x && pos.Y == y)
            {
                collided = true;
            }
        });
        var grid = world.GetFirst<Grid>();
        if (grid[x, y] == 0 && !collided)
        {
            position.X = x;
            position.Y = y;
            return true;
        }
        
        return false;
    }
}

public class InfectDescriptor : Descriptor
{
    public Position Direction;

    public InfectDescriptor(Position direction)
    {
        Direction = direction;
    }

    public override bool Execute(Entity entity, World world)
    {
        var position = entity.Get<Position>() + Direction;
        world.Query(new QueryDescription().WithAll<Position, Infection>(), (ref Position pos, ref Infection infection) =>
        {
            if ((pos.X == position.X && pos.Y == position.Y) || (pos.PrevX == position.X && pos.PrevY == position.Y))
            {
                if (infection.Status == InfectionStatus.Susceptible)
                {
                    infection.Status = InfectionStatus.Infected;
                }
            }
        });
        return true;
    }
}

public class TellDescriptor : Descriptor
{
    public Position Direction;
    public string Message;

    public TellDescriptor(Position direction, string message)
    {
        Direction = direction;
        Message = message;
    }

    public override bool Execute(Entity entity, World world)
    {
        var position = entity.Get<Position>() + Direction;
        world.Query(new QueryDescription().WithAll<Position, WorkingMemory>(), (ref Position pos, ref WorkingMemory memory) =>
        {
            var messages = memory.Memory.GetOr("messages", () => new List<string>());
            if (pos.X == position.X && pos.Y == position.Y)
            {
                messages.Add(Message);
            }
        });
        return true;
    }
}