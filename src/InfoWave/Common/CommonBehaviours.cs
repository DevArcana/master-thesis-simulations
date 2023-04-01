namespace InfoWave.Common;

public static class CommonBehaviours
{
    public static bool ExchangesInformation(Actor self, Actor other)
    {
        foreach (var @event in self.Knowledge)
        {
            other.Learn(@event);
        }
        
        return true;
    }
}