namespace InfoWave.EventCalculus;

public record Predicate;

public record FluentPredicate(Fluent Fluent) : Predicate;

public record Initiated(Fluent Fluent) : FluentPredicate(Fluent);

public record Terminated(Fluent Fluent) : FluentPredicate(Fluent);

public record EventPredicate(Func<FluentState, IEnumerable<Predicate>> Action) : Predicate;