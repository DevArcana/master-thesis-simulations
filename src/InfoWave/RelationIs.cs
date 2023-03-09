namespace InfoWave;

/// <summary>
/// A binary relation of "X is Y".
/// </summary>
/// <param name="Entity">Entity existing in the information world.</param>
/// <param name="AttributeValue">Value of specified attribute given to the entity.</param>
public record RelationIs(Entity Entity, AttributeValue AttributeValue)
{
    public bool ConflictsWith(RelationIs other) =>
        Entity == other.Entity &&
        AttributeValue.Attribute == other.AttributeValue.Attribute &&
        AttributeValue.Value != other.AttributeValue.Value;
}