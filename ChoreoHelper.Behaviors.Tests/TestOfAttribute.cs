namespace ChoreoHelper.Behaviors.Tests;

public sealed class TestOfAttribute : Attribute
{
    public TestOfAttribute(Type type)
    {
    }

    public TestOfAttribute(Type type, string methodName)
    {
    }
}