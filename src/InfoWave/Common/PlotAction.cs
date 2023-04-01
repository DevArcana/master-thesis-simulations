namespace InfoWave.Common;

public record PlotAction(Actor Actor, string Description, EventTag[] Tags) : PlotEvent(Description, Tags)
{
    public override string ToString()
    {
        return string.Format(Description, Actor.Name);
    }
}