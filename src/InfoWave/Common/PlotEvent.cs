namespace InfoWave.Common;

public record PlotEvent(string Description, EventTag[] Tags, params object[] Values)
{
    public EventTag? GetTag(string name)
    {
        return Tags.FirstOrDefault(t => t.Name == name);
    }

    public override string ToString()
    {
        return string.Format(Description, Values);
    }
}