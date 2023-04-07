namespace InfoWave.Tests;

public record Fluent;
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
    private readonly SortedList<int, Predicate> _state = new(new ReverseIntComparer());
    
    private int _time = int.MaxValue;

    public void Initiate(Fluent fluent, int at)
    {
        
        _state.Add(at, new Initiated(fluent));
    }
    
    public void Terminate(Fluent fluent, int at)
    {
        _state.Add(at, new Terminated(fluent));
    }
    
    public void AddEvent(EventPredicate eventPredicate, int at)
    {
        _state.Add(at, eventPredicate);
    }

    public bool HoldsAt(Fluent fluent, int at)
    {
        foreach (var (_, predicate) in _state.Where(x => x.Key <= at))
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

        return false;
    }

    public bool Holds(Fluent fluent) => HoldsAt(fluent, _time);
}

public class EventCalculus
{
    [Test]
    public void FluentsDoNotHoldBeforeInitiated()
    {
        var state = new FluentState();
        var fluent = new Fluent();
        
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
        var fluent = new Fluent();
        
        state.Initiate(fluent, 3);
        state.HoldsAt(fluent, 3).Should().BeTrue();
    }
    
    [Test]
    public void FluentDoesNotHoldAtTermination()
    {
        var state = new FluentState();
        var fluent = new Fluent();
        
        state.Initiate(fluent, 3);
        state.Terminate(fluent, 4);
        state.HoldsAt(fluent, 4).Should().BeFalse();
    }
    
    [Test]
    public void ActionsInfluenceFluents()
    {
        var state = new FluentState();
        var fluent = new Fluent();
        var action = new EventPredicate(_ => new[] {new Initiated(fluent)});
        
        state.AddEvent(action, 3);
        state.HoldsAt(fluent, 3).Should().BeTrue();
    }
    
    [Test]
    public void ComplexActionsInfluenceEvents()
    {
        var state = new FluentState();
        var fluentPre = new Fluent();
        var fluentPost = new Fluent();
        var action = new EventPredicate(state => state.Holds(fluentPre) ? new[] {new Initiated(fluentPost)} : Array.Empty<Predicate>());
        
        state.AddEvent(action, 3);
        state.HoldsAt(fluentPost, 3).Should().BeFalse();
        state.Initiate(fluentPre, 2);
        state.HoldsAt(fluentPost, 3).Should().BeTrue();
    }
    
    [Test]
    public void ChainActionsInfluenceEvents()
    {
        var state = new FluentState();
        var fluent1 = new Fluent();
        var fluent2 = new Fluent();
        var fluent3 = new Fluent();
        var fluent4 = new Fluent();
        var action1 = new EventPredicate(state => state.Holds(fluent1) ? new[] {new Initiated(fluent2)} : Array.Empty<Predicate>());
        var action2 = new EventPredicate(state => state.Holds(fluent2) ? new[] {new Initiated(fluent3)} : Array.Empty<Predicate>());
        var action3 = new EventPredicate(state => state.Holds(fluent3) ? new[] {new Initiated(fluent4)} : Array.Empty<Predicate>());
        
        state.AddEvent(action1, 3);
        state.AddEvent(action2, 4);
        state.AddEvent(action3, 5);
        state.HoldsAt(fluent4, 5).Should().BeFalse();
        state.Initiate(fluent1, 2);
        state.HoldsAt(fluent4, 5).Should().BeTrue();
    }
}