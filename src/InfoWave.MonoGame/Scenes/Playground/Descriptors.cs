using System;
using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;

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
        return true;
    }
}