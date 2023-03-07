namespace InfoWave;

public class ConflictingFactException : Exception
{
    public ConflictingFactException(Fact factA, Fact factB) : base($"Fact \"{factA.Statement}\" conflicts with \"{factB.Statement}\"")
    {
    }
}