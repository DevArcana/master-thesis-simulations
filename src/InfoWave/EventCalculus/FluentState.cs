namespace InfoWave.EventCalculus;

public class FluentState
{
    private sealed class OrderNewestEventsFirst : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
    }
    
    private readonly SortedList<int, HashSet<Predicate>> _state = new(new OrderNewestEventsFirst());

    private int _time = int.MaxValue;

    private bool Conflicts(Predicate predicate, int at)
    {
        switch (predicate)
        {
            case EventPredicate eventPredicate:
                var time = _time;
                _time = at - 1;
                var predicates = eventPredicate.Action(this);
                _time = time;
                return predicates.Any(fluent => Conflicts(fluent, at));
            case Initiated initiated:
                if (_state.TryGetValue(at, out var set) && set.Any(x => x is Terminated terminated && terminated.Fluent == initiated.Fluent))
                {
                    return true;
                }
                break;
            case Terminated terminated:
                if (_state.TryGetValue(at, out set) && set.Any(x => x is Initiated initiated && initiated.Fluent == terminated.Fluent))
                {
                    return true;
                }
                break;
        }

        return false;
    }

    public bool Initiate(Fluent fluent, int at)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new HashSet<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new Initiated(fluent);
        if (Conflicts(predicate, at)) return false;
        set.Add(predicate);
        return true;
    }

    public bool Terminate(Fluent fluent, int at)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new HashSet<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new Terminated(fluent);
        if (Conflicts(predicate, at)) return false;
        set.Add(predicate);
        return true;
    }

    public bool AddEvent(Func<FluentState, IEnumerable<Predicate>> action, int at)
    {
        if (!_state.TryGetValue(at, out var set))
        {
            set = new HashSet<Predicate>();
            _state.Add(at, set);
        }

        var predicate = new EventPredicate(action);
        if (Conflicts(predicate, at)) return false;
        set.Add(predicate);
        return true;
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