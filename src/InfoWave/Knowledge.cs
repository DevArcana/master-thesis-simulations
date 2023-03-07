namespace InfoWave;

public class Knowledge
{
    private readonly HashSet<Fact> _facts = new();
    public IReadOnlySet<Fact> Facts => _facts;

    public void Add(Fact fact)
    {
        _facts.Add(fact);
    }
}