using InfoWave.Common;

namespace InfoWave.Guard;

public static class GuardBehaviours
{
    public const string GuardTagName = "Guard";
    public const string CrimeTagName = "Crime";

    public static Tag GuardTag => new(GuardTagName);
    public static Tag CrimeTag => new(CrimeTagName);
    
    public static bool ExchangesInformation(Actor self, Actor other)
    {
        var hasRestrictedAccess = other.Tags.Any(x => x.Name == GuardTagName);
        
        foreach (var @event in self.Knowledge)
        {
            if (@event.Tags.Any(x => x.Name == GuardTagName) && !hasRestrictedAccess)
            {
                continue;
            }
            
            other.Learn(@event);
        }
        
        return true;
    }
    
    public static bool CatchesCriminals(Actor self, Actor other)
    {
        var crime = self.Knowledge.FirstOrDefault(x =>
        {
            var isCrime = x.Tags.Any(y => y.Name == CrimeTagName);
            var criminal = (x as PlotAction)?.Actor;
            
            if (isCrime && criminal != null)
            {
                return criminal == other;
            }

            return false;
        });
        
        if (crime is not null)
        {
            var arrest = new PlotAction(self, "{0} arrested {1} for their crime: {2}", Array.Empty<Tag>(), other.Name, crime.ToString());
            
            self.Learn(arrest);
            other.Learn(arrest);

            return true;
        }
        
        return false;
    }
}