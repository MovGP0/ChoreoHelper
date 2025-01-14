namespace ReactiveUI.TestObjects;

public sealed class TestClass
{
    public string SimpleProperty { get; set; } = string.Empty;
    public NestedClass? NestedProperty { get; set; }
}

public sealed class NestedClass
{
    public int ValueProperty { get; set; }
    public DeepNestedClass? DeepNestedProperty { get; set; }
}

public class DeepNestedClass
{
    public double DeepValueProperty { get; set; }
}