namespace InfoWave.Tests;

public record PlotPoint(string Description)
{
    public override string ToString()
    {
        return Description;
    }
}

public record ActorPlotPoint(Actor Actor, string Description) : PlotPoint(Description)
{
    public override string ToString()
    {
        return string.Format(Description, Actor.Name);
    }
}

public class PlotPointTests
{
    [Test]
    public void ToStringReturnsDescription()
    {
        var sut = new PlotPoint("The king is dead");
        sut.ToString().Should().Be("The king is dead");
    }
    
    [Test]
    public void ToStringReturnsDescriptionWithActor()
    {
        var guardA = new Actor("Guard A");
        var guardB = new Actor("Guard B");
        
        guardA.Learn(new ActorPlotPoint(guardB, "{0} is a friend"));
        guardA.ToString().Should().Be("Guard A knows: Guard B is a friend");
    }
}

public class Actor
{
    public string Name { get; }
    public IEnumerable<PlotPoint> Knowledge => _knowledge;

    private readonly HashSet<PlotPoint> _knowledge = new();

    public Actor(string name)
    {
        Name = name;
    }

    public void Learn(PlotPoint point)
    {
        _knowledge.Add(point);
    }

    public void TalkTo(Actor other)
    {
        foreach (var point in Knowledge)
        {
            other.Learn(point);
        }

        foreach (var point in other.Knowledge)
        {
            Learn(point);
        }
    }

    public override string ToString()
    {
        return _knowledge.Count == 0 
            ? $"{Name} knows nothing" 
            : $"{Name} knows: {string.Join(", ", Knowledge)}";
    }
}

public class ActorTests
{
    [Test]
    public void StartsWithoutKnowledge()
    {
        var sut = new Actor("King");
        sut.Knowledge.Should().BeEmpty();
        sut.ToString().Should().Be("King knows nothing");
    }
    
    [Test]
    public void LearnsPlotPoints()
    {
        var sut = new Actor("Servant");
        sut.Learn(new PlotPoint("The king is dead"));
        sut.Learn(new PlotPoint("Long live the king"));
        sut.Knowledge.Should().HaveCount(2);
        sut.ToString().Should().Be("Servant knows: The king is dead, Long live the king");
    }
    
    [Test]
    public void LearnsOnlyUniquePlotPoints()
    {
        var sut = new Actor("Servant");
        sut.Learn(new PlotPoint("The king is dead"));
        sut.Learn(new PlotPoint("The king is dead"));
        sut.Knowledge.Should().HaveCount(1);
        sut.ToString().Should().Be("Servant knows: The king is dead");
    }
    
    [Test]
    public void LearnsPlotPointsFromOtherActors()
    {
        // Arrange
        var guardA = new Actor("Guard A");
        var guardB = new Actor("Guard B");
        var criminal = new Actor("Criminal");
        
        var crime = new ActorPlotPoint(criminal, "{0} stole an apple");
        
        // Act
        guardA.Learn(crime);
        guardA.TalkTo(guardB);
        
        // Assert
        guardB.ToString().Should().Be("Guard B knows: Guard A knows: Criminal stole an apple");
    }
}