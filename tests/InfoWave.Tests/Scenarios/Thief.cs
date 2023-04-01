using InfoWave.Common;
using Moq;

namespace InfoWave.Tests.Scenarios;

public class Thief
{
    [Test]
    public void ThiefStealsAnApple()
    {
        // Arrange
        var guard = new Actor("Guard");
        var criminal = new Actor("Criminal");

        var crime = new PlotAction(criminal, "{0} stole an apple", new[] { new EventTag("Crime") });

        guard.AddBehaviour(
            plotEvent => plotEvent.GetTag("Crime") != null,
            plotEvent =>
            {
                Console.WriteLine("Guard discovered a crime! Arresting the criminal...");
                Console.WriteLine();
                var arrest = new PlotAction(guard, "{0} arrested {1}", Array.Empty<EventTag>(), criminal.Name);

                guard.Learn(arrest);
                (plotEvent as PlotAction)?.Actor.Learn(arrest);
            });

        // Act
        Console.WriteLine("Step 1");
        Console.WriteLine(criminal);
        Console.WriteLine(guard);
        Console.WriteLine();
        
        criminal.Learn(crime);
        
        Console.WriteLine("Step 2");
        Console.WriteLine(criminal);
        Console.WriteLine(guard);
        Console.WriteLine();
        
        guard.Learn(crime);

        Console.WriteLine("Step 3");
        Console.WriteLine(criminal);
        Console.WriteLine(guard);
        Console.WriteLine();
        
        // Assert
        guard.ToString().Should().Be("Guard knows: Criminal stole an apple, Guard arrested Criminal");
        criminal.ToString().Should().Be("Criminal knows: Criminal stole an apple, Guard arrested Criminal");
    }
}