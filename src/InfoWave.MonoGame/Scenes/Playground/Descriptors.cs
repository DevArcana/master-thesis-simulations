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
        var grid = world.GetFirst<Grid>();
        if (grid[x, y] == 0)
        {
            position.X = x;
            position.Y = y;
            return true;
        }
        
        return false;
    }
}