namespace InfoWave.Tests;

public class KnowledgeTests
{
    [Fact]
    public void AcceptsSingleFact()
    {
        var fact = new Fact("the ball is green");
        var sut = new Knowledge();

        sut.Add(fact);

        sut.Facts.Count.Should().Be(1);
    }
}