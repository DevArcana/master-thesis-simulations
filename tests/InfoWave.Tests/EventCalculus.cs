namespace InfoWave.Tests;

public record Fluent(string Description);

public record Predicate;

public record FluentPredicate(Fluent Fluent) : Predicate;

public record Initiated(Fluent Fluent) : FluentPredicate(Fluent);

public record Terminated(Fluent Fluent) : FluentPredicate(Fluent);

public record EventPredicate(Func<FluentState, IEnumerable<Predicate>> Action) : Predicate;

internal sealed class ReverseIntComparer : IComparer<int>
{
    public int Compare(int x, int y) => y.CompareTo(x);
}

public class FluentState
{
    private readonly SortedList<int, HashSet<Predicate>> _state = new(new ReverseIntComparer());

    private int _time = int.MaxValue;

    private void ThrowOnConflict(Predicate predicate, int at)
    {
        switch (predicate)
        {
            case EventPredicate eventPredicate:
                var time = _time;
                _time = at - 1;
                var predicates = eventPredicate.Action(this);
                _time = time;
                foreach (var fluent in predicates)
                {
                    ThrowOnConflict(fluent, at);
                }
                break;
            case Initiated initiated:
                if (_state.TryGetValue(at, out var set) && set.Any(x => x is Terminated terminated && terminated.Fluent == initiated.Fluent))
                {
                    throw new InvalidOperationException($"Fluent {initiated.Fluent} is already terminated at {at}");
                }
                break;
            case Terminated terminated:
                if (_state.TryGetValue(at, out set) && set.Any(x => x is Initiated initiated && initiated.Fluent == terminated.Fluent))
                {
                    throw new InvalidOperationException($"Fluent {terminated.Fluent} is already initiated at {at}");
                }
                break;
        }
    }

    public void Initiate(Fluent fluent, int at)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new HashSet<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new Initiated(fluent);
        ThrowOnConflict(predicate, at);
        set.Add(predicate);
    }

    public void Terminate(Fluent fluent, int at)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new HashSet<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new Terminated(fluent);
        ThrowOnConflict(predicate, at);
        set.Add(predicate);
    }

    public void AddEvent(Func<FluentState, IEnumerable<Predicate>> action, int at)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new HashSet<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new EventPredicate(action);
        ThrowOnConflict(predicate, at);
        set.Add(predicate);
    }

    public bool HoldsAt(Fluent fluent, int at)
    {
        foreach (var (_, set) in _state.Where(x => x.Key <= at))
        {
            foreach (var predicate in set)
            {
                switch (predicate)
                {
                    case Initiated initiated when initiated.Fluent == fluent:
                        return true;
                    case Terminated terminated when terminated.Fluent == fluent:
                        return false;
                    case EventPredicate eventPredicate:
                        var time = _time;
                        _time = at - 1;
                        var predicates = eventPredicate.Action(this);
                        _time = time;
                        foreach (var action in predicates)
                        {
                            switch (action)
                            {
                                case Initiated initiated when initiated.Fluent == fluent:
                                    return true;
                                case Terminated terminated when terminated.Fluent == fluent:
                                    return false;
                            }
                        }

                        break;
                }
            }
        }

        return false;
    }

    public bool Holds(Fluent fluent) => HoldsAt(fluent, _time);
}

public class EventCalculus
{
    [Test]
    public void FluentsAreUnique()
    {
        var fluent1 = new Fluent("Fluent 1");
        var fluent2 = new Fluent("Fluent 2");

        fluent1.Should().NotBe(fluent2);
    }

    [Test]
    public void FluentsDoNotHoldBeforeInitiated()
    {
        var state = new FluentState();
        var fluent = new Fluent("Fluent 1");

        state.HoldsAt(fluent, 3).Should().BeFalse();
        state.Initiate(fluent, 3);
        state.HoldsAt(fluent, 4).Should().BeTrue();
        state.HoldsAt(fluent, 3).Should().BeTrue();
        state.HoldsAt(fluent, 2).Should().BeFalse();
        state.Holds(fluent).Should().BeTrue();
        state.Terminate(fluent, 4);
        state.HoldsAt(fluent, 5).Should().BeFalse();
        state.HoldsAt(fluent, 4).Should().BeFalse();
        state.HoldsAt(fluent, 3).Should().BeTrue();
        state.Holds(fluent).Should().BeFalse();
    }

    [Test]
    public void FluentHoldsAtInitiation()
    {
        var state = new FluentState();
        var fluent = new Fluent("Fluent 1");

        state.Initiate(fluent, 3);
        state.HoldsAt(fluent, 3).Should().BeTrue();
    }

    [Test]
    public void FluentDoesNotHoldAtTermination()
    {
        var state = new FluentState();
        var fluent = new Fluent("Fluent 1");

        state.Initiate(fluent, 3);
        state.Terminate(fluent, 4);
        state.HoldsAt(fluent, 4).Should().BeFalse();
    }

    [Test]
    public void ActionsInfluenceFluents()
    {
        var state = new FluentState();
        var fluent = new Fluent("Fluent 1");
        IEnumerable<Predicate> Action(FluentState _) => new[] { new Initiated(fluent) };

        state.AddEvent(Action, 3);
        state.HoldsAt(fluent, 3).Should().BeTrue();
    }

    [Test]
    public void ComplexActionsInfluenceEvents()
    {
        var state = new FluentState();
        var fluentPre = new Fluent("Fluent 1");
        var fluentPost = new Fluent("Fluent 2");

        IEnumerable<Predicate> Action(FluentState fs) =>
            fs.Holds(fluentPre!) ? new[] { new Initiated(fluentPost!) } : Array.Empty<Predicate>();

        state.AddEvent(Action, 3);
        state.HoldsAt(fluentPost, 3).Should().BeFalse();
        state.Initiate(fluentPre, 2);
        state.HoldsAt(fluentPost, 3).Should().BeTrue();
    }

    [Test]
    public void ChainActionsInfluenceEvents()
    {
        var state = new FluentState();
        var fluent1 = new Fluent("Fluent 1");
        var fluent2 = new Fluent("Fluent 2");
        var fluent3 = new Fluent("Fluent 3");
        var fluent4 = new Fluent("Fluent 4");

        IEnumerable<Predicate> Action1(FluentState fs) =>
            fs.Holds(fluent1!) ? new[] { new Initiated(fluent2!) } : Array.Empty<Predicate>();

        IEnumerable<Predicate> Action2(FluentState fs) =>
            fs.Holds(fluent2!) ? new[] { new Initiated(fluent3!) } : Array.Empty<Predicate>();

        IEnumerable<Predicate> Action3(FluentState fs) =>
            fs.Holds(fluent3!) ? new[] { new Initiated(fluent4!) } : Array.Empty<Predicate>();

        state.AddEvent(Action1, 3);
        state.AddEvent(Action2, 4);
        state.AddEvent(Action3, 5);
        state.HoldsAt(fluent4, 5).Should().BeFalse();
        state.Initiate(fluent1, 2);
        state.HoldsAt(fluent4, 5).Should().BeTrue();
    }

    [Test]
    public void FluentsMustNotConflict()
    {
        var state = new FluentState();
        var fluent = new Fluent("Fluent 1");
        state.Initiate(fluent, 3);

        Assert.Throws<InvalidOperationException>(() => state.Terminate(fluent, 3));
        Assert.Throws<InvalidOperationException>(() => state.AddEvent(_ => new[] { new Terminated(fluent) }, 3));
    }
}