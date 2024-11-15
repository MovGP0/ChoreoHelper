namespace ChoreoHelper.Entities;

public sealed class Distance(OneOf<float, None, Unknown> input) : OneOfBase<float, None, Unknown>(input)
{
    public static implicit operator Distance(float value) => new Distance(value);
    public static implicit operator Distance(None value) => new Distance(value);
    public static implicit operator Distance(Unknown value) => new Distance(value);
}