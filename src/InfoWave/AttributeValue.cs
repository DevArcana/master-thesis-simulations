namespace InfoWave;

/// <summary>
/// Represents a value given to a specific attribute.
/// </summary>
/// <param name="Attribute">Attribute the value corresponds to.</param>
/// <param name="Value">The value of the specified attribute.</param>
public record AttributeValue(Attribute Attribute, string Value);