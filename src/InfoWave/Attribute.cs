namespace InfoWave;

/// <summary>
/// Represents a specific attribute such as color, shape, etc.
/// </summary>
/// <param name="Id">Unique name of the attribute.</param>
public record Attribute(string Id) : Identifier("attribute", Id);