namespace InfoWave.Common;

public record PlotEvent(string Description, Tag[] Tags, params object[] Values)
{
    public Tag? GetTag(string name)
    {
        return Tags.FirstOrDefault(t => t.Name == name);
    }

    public override string ToString()
    {
        return string.Format(Description, Values);
    }
}