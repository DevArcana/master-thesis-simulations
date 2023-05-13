using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;

namespace InfoWave.MonoGame.Common.Utils;

public static class WorldExtensions
{
    public static IEnumerable<Entity> Matching(this World world, QueryDescription query)
    {
        var entities = new List<Entity>();
        world.GetEntities(query, entities);
        return entities;
    }

    public static T GetFirst<T>(this World world)
    {
        var query = new QueryDescription().WithAll<T>();
        return world.Matching(query).First().Get<T>();
    }
}