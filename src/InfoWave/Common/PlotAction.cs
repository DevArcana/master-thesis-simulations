namespace InfoWave.Common;

public record PlotAction(Actor Actor, string Description, EventTag[] Tags, params object[] Values) : PlotEvent(Description, Tags, Values)
{
    public override string ToString()
    {
        return string.Format(Description, Values.Prepend(Actor.Name).ToArray());
    }
}