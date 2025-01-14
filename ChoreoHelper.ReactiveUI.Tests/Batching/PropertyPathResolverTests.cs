using System.Linq.Expressions;
using ReactiveUI.TestObjects;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ReactiveUI.Batching;

[TestOf(typeof(PropertyPathResolver))]
public static class PropertyPathResolverTests
{
    [TestOf(typeof(PropertyPathResolver), nameof(PropertyPathResolver.GetPropertyChain))]
    public sealed class GetPropertyChainTests
    {
        public GetPropertyChainTests(ITestOutputHelper testOutputHelper)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddXUnit(testOutputHelper);
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            services.UseMicrosoftDependencyResolver();
            _ = services.BuildServiceProvider();
        }

        [Fact]
        public void SimpleProperty_ReturnsCorrectChain()
        {
            // Arrange & Act
            string chain = PropertyPathResolver.GetPropertyChain<TestClass>(e => e.SimpleProperty);

            // Assert
            chain.ShouldBe("SimpleProperty");
        }

        [Fact]
        public void NestedProperty_ReturnsCorrectChain()
        {
            // Arrange & Act
            string chain = PropertyPathResolver.GetPropertyChain<TestClass>(e => e.NestedProperty.ValueProperty);

            // Assert
            chain.ShouldBe("NestedProperty.ValueProperty");
        }

        [Fact]
        public void DeepNestedProperty_ReturnsCorrectChain()
        {
            // Arrange & Act
            string chain = PropertyPathResolver.GetPropertyChain<TestClass>(e => e.NestedProperty.DeepNestedProperty.DeepValueProperty);

            // Assert
            chain.ShouldBe("NestedProperty.DeepNestedProperty.DeepValueProperty");
        }

        [Fact]
        public void PropertyWithCast_ReturnsCorrectChain()
        {
            // Arrange & Act
            string chain = PropertyPathResolver.GetPropertyChain<TestClass>(e => (object)e.SimpleProperty);

            // Assert
            chain.ShouldBe("SimpleProperty");
        }

        [Fact]
        public void PropertyWithMethodCall_ThrowsException()
        {
            // Arrange & Act & Assert
            Should.Throw<InvalidOperationException>(() =>
                PropertyPathResolver.GetPropertyChain<TestClass>(e => e.SimpleProperty.ToString()));
        }

        [Fact]
        public void UnaryExpression_ReturnsCorrectChain()
        {
            // Arrange
            Action action = () => PropertyPathResolver.GetPropertyChain<TestClass>(e => e.NestedProperty.ValueProperty * 2);

            // Act & Assert
            Should.Throw<InvalidOperationException>(action);
        }

        [Fact]
        public void NullableProperty_ReturnsCorrectChain()
        {
            // Arrange
            Expression<Func<TestClass, object?>> expression = e => e.NestedProperty.DeepNestedProperty.DeepValueProperty;

            // Act
            string chain = PropertyPathResolver.GetPropertyChain(expression);

            // Assert
            chain.ShouldBe("NestedProperty.DeepNestedProperty.DeepValueProperty");
        }
    }

    [TestOf(typeof(PropertyPathResolver), nameof(PropertyPathResolver.GetPropertyChains))]
    public sealed class GetPropertyChainsTests
    {
        [Fact]
        public void GetPropertyChains_MultipleProperties_ReturnsCorrectChains()
        {
            // Arrange
            Expression<Func<TestClass, object?>>[] expressions = new Expression<Func<TestClass, object?>>[]
            {
                e => e.SimpleProperty,
                e => e.NestedProperty.ValueProperty,
                e => e.NestedProperty.DeepNestedProperty.DeepValueProperty
            };

            // Act
            var chains = PropertyPathResolver.GetPropertyChains(expressions).ToList();

            // Assert
            chains.ShouldBe(new List<string>
            {
                "SimpleProperty",
                "NestedProperty.ValueProperty",
                "NestedProperty.DeepNestedProperty.DeepValueProperty"
            });
        }

        [Fact]
        public void GetPropertyChains_EmptyArray_ReturnsEmptyList()
        {
            // Arrange
            Expression<Func<TestClass, object?>>[] expressions = Array.Empty<Expression<Func<TestClass, object?>>>();

            // Act
            var chains = PropertyPathResolver.GetPropertyChains(expressions).ToList();

            // Assert
            chains.ShouldBeEmpty();
        }

        [Fact]
        public void GetPropertyChains_WithCastExpressions_ReturnsCorrectChains()
        {
            // Arrange
            Expression<Func<TestClass, object?>>[] expressions = new Expression<Func<TestClass, object?>>[]
            {
                e => (object)e.SimpleProperty,
                e => (object)e.NestedProperty.ValueProperty
            };

            // Act
            var chains = PropertyPathResolver.GetPropertyChains(expressions).ToList();

            // Assert
            chains.ShouldBe(new List<string>
            {
                "SimpleProperty",
                "NestedProperty.ValueProperty"
            });
        }

        [Fact]
        public void GetPropertyChains_WithUnsupportedExpressions_ThrowsException()
        {
            // Arrange
            Expression<Func<TestClass, object?>>[] expressions = new Expression<Func<TestClass, object?>>[]
            {
                e => e.SimpleProperty.ToString(),
                e => e.NestedProperty.ValueProperty * 2
            };

            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
                PropertyPathResolver.GetPropertyChains(expressions).ToList());
        }
    }

    [TestOf(typeof(PropertyPathResolver), nameof(PropertyPathResolver.GetPropertyValueFromPropertyChain))]
    public sealed class GetPropertyValueFromPropertyChainTests
    {
        [Fact]
        public void SimpleProperty_ReturnsValue()
        {
            // Arrange
            var testObj = new TestClass { SimpleProperty = "TestValue" };

            // Act
            var value = testObj.GetPropertyValueFromPropertyChain("SimpleProperty");

            // Assert
            value.ShouldBe("TestValue");
        }

        [Fact]
        public void NestedProperty_ReturnsValue()
        {
            // Arrange
            var testObj = new TestClass
            {
                NestedProperty = new NestedClass { ValueProperty = 42 }
            };

            // Act
            var value = testObj.GetPropertyValueFromPropertyChain("NestedProperty.ValueProperty");

            // Assert
            value.ShouldBe(42);
        }

        [Fact]
        public void NullNestedProperty_ReturnsNull()
        {
            // Arrange
            var testObj = new TestClass { NestedProperty = null };

            // Act
            var value = testObj.GetPropertyValueFromPropertyChain("NestedProperty.ValueProperty");

            // Assert
            value.ShouldBeNull();
        }

        [Fact]
        public void InvalidProperty_ReturnsNull()
        {
            // Arrange
            var testObj = new TestClass();

            // Act
            var value = testObj.GetPropertyValueFromPropertyChain("NonExistentProperty");

            // Assert
            value.ShouldBeNull();
        }
    }
}
