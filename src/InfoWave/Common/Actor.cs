namespace InfoWave.Common;

public class Actor
{
    public string Name { get; }
    
    private readonly HashSet<PlotEvent> _knowledge = new();
    private readonly List<(Func<PlotEvent, bool>, Action<PlotEvent>)> _behaviours = new();

    public Actor(string name)
    {
        Name = name;
    }
    
    public void Learn(PlotEvent plotEvent)
    {
        _knowledge.Add(plotEvent);
        
        foreach (var (precondition, action) in _behaviours)
        {
            if (precondition(plotEvent))
            {
                action(plotEvent);
            }
        }
    }

    public void ShareWith(Actor actor)
    {
        foreach (var plotEvent in _knowledge)
        {
            actor.Learn(plotEvent);
        }
    }

    public void AddBehaviour(Func<PlotEvent, bool> precondition, Action<PlotEvent> action)
    {
        _behaviours.Add((precondition, action));
    }
    
    public override string ToString()
    {
        return _knowledge.Count == 0 
            ? $"{Name} knows nothing" 
            : $"{Name} knows: {string.Join(", ", _knowledge)}";
    }
}