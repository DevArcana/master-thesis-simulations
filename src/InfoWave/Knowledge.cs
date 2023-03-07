namespace InfoWave;

public class Knowledge
{
    private readonly HashSet<Fact> _facts = new();
    public IReadOnlySet<Fact> Facts => _facts;

    public void Add(Fact fact)
    {
        foreach (var known in _facts)
        {
            foreach (var knownConflict in known.Conflicts)
            {
                foreach (var conflict in fact.Conflicts)
                {
                    if (knownConflict == conflict)
                    {
                        throw new ConflictingFactException(fact, known);
                    }
                }
            }
        }
        
        _facts.Add(fact);
    }
}