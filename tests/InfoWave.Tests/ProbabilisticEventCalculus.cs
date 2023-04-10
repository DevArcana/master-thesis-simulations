namespace InfoWave.Tests.ProbabilisticEventCalculus;

public record Fluent(string Description);

public record Predicate;

public record FluentPredicate(float Certainty, Fluent Fluent) : Predicate;

public record Initiated(float Certainty, Fluent Fluent) : FluentPredicate(Certainty, Fluent);

public record Terminated(float Certainty, Fluent Fluent) : FluentPredicate(Certainty, Fluent);

public record EventPredicate(Func<FluentState, IEnumerable<Predicate>> Action) : Predicate;

internal sealed class ReverseIntComparer : IComparer<int>
{
    public int Compare(int x, int y) => y.CompareTo(x);
}

public class FluentState
{
    private readonly SortedList<int, List<Predicate>> _state = new(new ReverseIntComparer());

    private int _time = int.MaxValue;

    private IEnumerable<FluentPredicate> ExecuteAction(int at, Func<FluentState, IEnumerable<Predicate>> action)
    {
        var time = _time;
        _time = at - 1;
        var predicates = action(this);
        _time = time;

        foreach (var predicate in predicates)
        {
            switch (predicate)
            {
                case EventPredicate eventPredicate:
                    foreach (var fluent in ExecuteAction(at, eventPredicate.Action))
                    {
                        yield return fluent;
                    }

                    break;
                case FluentPredicate fluentPredicate:
                    yield return fluentPredicate;
                    break;
            }
        }
    }

    private IEnumerable<FluentPredicate> GetPredicatesAt(int at)
    {
        if (_state.TryGetValue(at, out var set))
        {
            foreach (var predicate in set)
            {
                switch (predicate)
                {
                    case EventPredicate eventPredicate:
                        foreach (var action in ExecuteAction(at, eventPredicate.Action))
                        {
                            yield return action;
                        }

                        break;
                    case FluentPredicate fluentPredicate:
                        yield return fluentPredicate;
                        break;
                }
            }
        }
    }

    public void Initiate(Fluent fluent, int at, float certainty = 1.0f)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new List<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new Initiated(certainty, fluent);
        set.Add(predicate);
    }

    public void Terminate(Fluent fluent, int at, float certainty = 1.0f)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new List<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new Terminated(certainty, fluent);
        set.Add(predicate);
    }

    public void AddEvent(Func<FluentState, IEnumerable<Predicate>> action, int at)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new List<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new EventPredicate(action);
        set.Add(predicate);
    }

    private static float CalculateCertainty(IReadOnlyCollection<FluentPredicate> predicates)
    {
        var certainty = 0.0f;

        foreach (var predicate in predicates)
        {
            switch (predicate)
            {
                case Initiated initiated:
                    certainty += initiated.Certainty;
                    break;
                case Terminated terminated:
                    certainty += 1.0f - terminated.Certainty;
                    break;
            }
        }
        
        return certainty / predicates.Count;
    }

    public float HoldsAt(Fluent fluent, int at)
    {
        foreach (var time in _state.Keys.Where(t => t <= at))
        {
            var predicates = GetPredicatesAt(time).Where(predicate => predicate.Fluent == fluent).ToArray();
            if (predicates.Any())
            {
                return CalculateCertainty(predicates);
            }
        }

        return 0.0f;
    }

    public float Holds(Fluent fluent) => HoldsAt(fluent, _time);
}

public class ProbabilisticEventCalculus
{
    [Test]
    public void Certainty()
    {
        var state = new FluentState();
        var fluent = new Fluent("Fluent");
        state.Initiate(fluent, 0);
        state.HoldsAt(fluent, 0).Should().Be(1.0f);
        state.Terminate(fluent, 0);
        state.HoldsAt(fluent, 0).Should().Be(0.5f);
        state.Terminate(fluent, 0);
        state.HoldsAt(fluent, 0).Should().BeApproximately(0.33f, 0.01f);
    }
    
    [Test]
    public void CompoundCertainty()
    {
        var state = new FluentState();
        var fluent = new Fluent("Fluent");
        state.Initiate(fluent, 0);
        state.HoldsAt(fluent, 0).Should().Be(1.0f);
        state.Terminate(fluent, 0);
        state.HoldsAt(fluent, 0).Should().Be(0.5f);
        state.Terminate(fluent, 1);
        state.HoldsAt(fluent, 0).Should().BeApproximately(0.33f, 0.01f);
    }
}