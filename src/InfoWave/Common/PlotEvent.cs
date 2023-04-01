namespace InfoWave.Common;

public record PlotEvent(string Description, EventTag[] Tags)
{
    public EventTag? GetTag(string name)
    {
        return Tags.FirstOrDefault(t => t.Name == name);
    }

    public override string ToString()
    {
        return Description;
    }
}