using InfoWave.Common;
using InfoWave.Guard;

namespace InfoWave.Tests.Scenarios;

public class GuardTests
{
    [Test]
    public void GuardsExchangeSecrets()
    {
        var tag = GuardBehaviours.GuardTag;

        var john = new Actor("John");
        var greg = new Actor("Greg");
        var bob = new Actor("Bob");

        john.AddTag(tag);
        greg.AddTag(tag);

        john.AddBehaviour(GuardBehaviours.ExchangesInformation);
        greg.AddBehaviour(GuardBehaviours.ExchangesInformation);
        bob.AddBehaviour(CommonBehaviours.ExchangesInformation);

        var secret = new PlotEvent("Guard Secret", new[] { tag });
        var gossip = new PlotEvent("Gossip", Array.Empty<Tag>());
        
        john.Learn(secret);
        greg.Learn(gossip);
        
        john.InteractWith(greg);
        john.InteractWith(bob);
        greg.InteractWith(bob);
        
        john.ToString().Should().Be("John knows: Guard Secret, Gossip");
        greg.ToString().Should().Be("Greg knows: Gossip, Guard Secret");
        bob.ToString().Should().Be("Bob knows: Gossip");
    }
    
    [Test]
    public void GuardsCatchCriminals()
    {
        var tag = GuardBehaviours.GuardTag;

        var john = new Actor("John");
        var bob = new Actor("Bob");

        john.AddTag(tag);

        john.AddBehaviour(GuardBehaviours.CatchesCriminals);
        john.AddBehaviour(GuardBehaviours.ExchangesInformation);
        
        bob.AddBehaviour(CommonBehaviours.ExchangesInformation);

        var secret = new PlotEvent("Guard Secret", new[] { tag });
        var gossip = new PlotEvent("Gossip", Array.Empty<Tag>());

        var crime = new PlotAction(bob, "{0} stole an apple", new[] { GuardBehaviours.CrimeTag });
        
        john.Learn(secret);
        john.Learn(crime);
        bob.Learn(gossip);
        
        john.InteractWith(bob);
        
        john.ToString().Should().Be("John knows: Guard Secret, Bob stole an apple, John arrested Bob for their crime: Bob stole an apple");
        bob.ToString().Should().Be("Bob knows: Gossip, John arrested Bob for their crime: Bob stole an apple");
    }
}