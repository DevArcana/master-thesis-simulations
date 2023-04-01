using InfoWave.Common;

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
        sut.Learn(new PlotEvent("The king is dead", Array.Empty<EventTag>()));
        sut.Learn(new PlotEvent("Long live the king", Array.Empty<EventTag>()));
        sut.ToString().Should().Be("Servant knows: The king is dead, Long live the king");
    }
    
    [Test]
    public void LearnsOnlyUniquePlotPoints()
    {
        var sut = new Actor("Servant");
        sut.Learn(new PlotEvent("The king is dead", Array.Empty<EventTag>()));
        sut.Learn(new PlotEvent("The king is dead", Array.Empty<EventTag>()));
        sut.ToString().Should().Be("Servant knows: The king is dead");
    }
    
    [Test]
    public void SharesKnowledgeWithAnotherActor()
    {
        // Arrange
        var guardA = new Actor("Guard A");
        var guardB = new Actor("Guard B");
        var criminal = new Actor("Criminal");
        
        var crime = new PlotAction(criminal, "{0} stole an apple", new []{new EventTag("Crime")});
        
        // Act
        guardA.Learn(crime);
        guardA.ShareWith(guardB);
        
        // Assert
        guardB.ToString().Should().Be("Guard B knows: Criminal stole an apple");
    }
}