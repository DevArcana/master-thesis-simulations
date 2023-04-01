namespace InfoWave.Common;

public class Actor
{
    public string Name { get; }
    
    public IEnumerable<PlotEvent> Knowledge => _knowledge;
    public IEnumerable<Tag> Tags => _tags;

    private readonly HashSet<PlotEvent> _knowledge = new();
    private readonly List<Func<Actor, Actor, bool>> _behaviours = new();
    private readonly List<Tag> _tags = new();

    public Actor(string name)
    {
        Name = name;
    }
    
    public void Learn(PlotEvent plotEvent)
    {
        _knowledge.Add(plotEvent);
    }

    public void AddBehaviour(Func<Actor, Actor, bool> behaviour)
    {
        _behaviours.Add(behaviour);
    }
    
    public void AddTag(Tag tag)
    {
        _tags.Add(tag);
    }

    public void InteractWith(Actor actor)
    {
        foreach (var behaviour in _behaviours)
        {
            if (behaviour(this, actor))
            {
                return;
            }
        }
        
        foreach (var behaviour in actor._behaviours)
        {
            if (behaviour(actor, this))
            {
                return;
            }
        }
    }
    
    public override string ToString()
    {
        return _knowledge.Count == 0 
            ? $"{Name} knows nothing" 
            : $"{Name} knows: {string.Join(", ", _knowledge)}";
    }
}