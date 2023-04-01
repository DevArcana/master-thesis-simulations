namespace InfoWave.Common;

public class Actor
{
    public string Name { get; }
    
    private readonly HashSet<PlotEvent> _knowledge = new();

    public Actor(string name)
    {
        Name = name;
    }
    
    public void Learn(PlotEvent plotEvent)
    {
        _knowledge.Add(plotEvent);
    }

    public void ShareWith(Actor actor)
    {
        foreach (var plotEvent in _knowledge)
        {
            actor.Learn(plotEvent);
        }
    }
    
    public override string ToString()
    {
        return _knowledge.Count == 0 
            ? $"{Name} knows nothing" 
            : $"{Name} knows: {string.Join(", ", _knowledge)}";
    }
}