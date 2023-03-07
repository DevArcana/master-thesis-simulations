namespace InfoWave.Tests;

public class KnowledgeTests
{
    [Fact]
    public void AcceptsSingleFact()
    {
        var fact = new Fact("the ball is green", new []{"ball/color"});
        var sut = new Knowledge();

        sut.Add(fact);

        sut.Facts.Count.Should().Be(1);
    }
    
    [Fact]
    public void ThrowsWhenConflictingFactReceived()
    {
        var fact = new Fact("the ball is green", new []{"ball/color"});
        var conflict = new Fact("the ball is red", new []{"ball/color"});
        var sut = new Knowledge();

        sut.Add(fact);
        Assert.Throws<ConflictingFactException>(() => sut.Add(conflict));

        sut.Facts.Count.Should().Be(1);
    }
    
    [Fact]
    public void AcceptsMultipleFacts()
    {
        var first = new Fact("the ball is green", new []{"ball/color"});
        var second = new Fact("the ball is round", new []{"ball/shape"});
        var sut = new Knowledge();

        sut.Add(first);
        sut.Add(second);

        sut.Facts.Count.Should().Be(2);
    }
}