namespace InfoWave;

/// <summary>
/// Represents any identifiable part of the information world.
/// </summary>
/// <param name="Type">Type of the identifiable object, ie entity, attribute, etc.</param>
/// <param name="Id">Identifier of the object.</param>
public record Identifier(string Type, string Id)
{
    public override string ToString() => $"{Type}:{Id}";
}