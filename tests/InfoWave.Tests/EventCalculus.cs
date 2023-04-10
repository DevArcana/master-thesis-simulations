using InfoWave.EventCalculus;

namespace InfoWave.Tests;

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
        state.Initiate(fluent, 3).Should().BeTrue();
        state.Terminate(fluent, 3).Should().BeFalse();
        state.AddEvent(_ => new[] { new Terminated(fluent) }, 3).Should().BeFalse();
        state.HoldsAt(fluent, 3).Should().BeTrue();
    }
}