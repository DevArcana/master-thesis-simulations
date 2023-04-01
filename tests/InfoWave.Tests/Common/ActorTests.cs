using InfoWave.Common;
using Moq;

namespace InfoWave.Tests.Common;

public class ActorTests
{
    [Test]
    public void StartsWithoutKnowledge()
    {
        var sut = new Actor("King");
        sut.ToString().Should().Be("King knows nothing");
    }

    [Test]
    public void LearnsPlotPoints()
    {
        var sut = new Actor("Servant");
        sut.Learn(new PlotEvent("The king is dead", Array.Empty<Tag>()));
        sut.Learn(new PlotEvent("Long live the king", Array.Empty<Tag>()));
        sut.ToString().Should().Be("Servant knows: The king is dead, Long live the king");
    }

    [Test]
    public void LearnsOnlyUniquePlotPoints()
    {
        var sut = new Actor("Servant");
        sut.Learn(new PlotEvent("The king is dead", Array.Empty<Tag>()));
        sut.Learn(new PlotEvent("The king is dead", Array.Empty<Tag>()));
        sut.ToString().Should().Be("Servant knows: The king is dead");
    }

    [Test]
    public void SharesKnowledgeWithAnotherActor()
    {
        // Arrange
        var guardA = new Actor("Guard A");
        var guardB = new Actor("Guard B");
        
        guardA.AddBehaviour(CommonBehaviours.ExchangesInformation);
        guardB.AddBehaviour(CommonBehaviours.ExchangesInformation);
        
        guardA.Learn(new PlotEvent("The weather is nice today", Array.Empty<Tag>()));
        guardB.Learn(new PlotEvent("The crops have grown very nicely", Array.Empty<Tag>()));

        // Act
        guardA.InteractWith(guardB);
        
        // Assert
        guardA.ToString().Should().Be("Guard A knows: The weather is nice today, The crops have grown very nicely");
        guardB.ToString().Should().Be("Guard B knows: The crops have grown very nicely, The weather is nice today");
    }
}