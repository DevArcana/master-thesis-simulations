namespace InfoWave;

/// <summary>
/// Represents an abstract object existing in the information world.
/// </summary>
/// <param name="Id">Identifier of the object.</param>
public record Entity(string Id) : Identifier("entity", Id);