using ReactiveUI.TestObjects;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ReactiveUI.Batching;

[TestOf(typeof(PropertyPathSubscription))]
public sealed class PropertyPathSubscriptionTests
{
    public PropertyPathSubscriptionTests(ITestOutputHelper testOutputHelper)
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
    public void SimplePropertyChange_NotifiesObserver()
    {
        // Arrange
        var rootObject = new TestObservableObject();
        var manager = new BatchedNotificationManager();
        bool notificationReceived = false;

        using var subscription = new PropertyPathSubscription(
            rootObject,
            "SimpleProperty",
            () => { notificationReceived = true; },
            manager);

        // Act
        rootObject.SimpleProperty = "New Value";

        // Assert
        notificationReceived.ShouldBeTrue();
    }

    [Fact]
    public void NestedPropertyChange_NotifiesObserver()
    {
        // Arrange
        var rootObject = new TestObservableObject
        {
            NestedProperty = new NestedTestObservableObject()
        };
        var manager = new BatchedNotificationManager();
        bool notificationReceived = false;

        using var subscription = new PropertyPathSubscription(
            rootObject,
            "NestedProperty.ValueProperty",
            () => { notificationReceived = true; },
            manager);

        // Act
        rootObject.NestedProperty.ValueProperty = 42;

        // Assert
        notificationReceived.ShouldBeTrue();
    }

    [Fact]
    public void NestedObjectReplacement_NotifiesObserver()
    {
        // Arrange
        var rootObject = new TestObservableObject
        {
            NestedProperty = new NestedTestObservableObject()
        };
        var manager = new BatchedNotificationManager();
        int notificationCount = 0;

        using var subscription = new PropertyPathSubscription(
            rootObject,
            "NestedProperty.ValueProperty",
            () => { notificationCount++; },
            manager);

        // Act
        rootObject.NestedProperty = new NestedTestObservableObject();
        rootObject.NestedProperty.ValueProperty = 100;

        // Assert
        // Should notify once for the NestedProperty change, and once for ValueProperty change
        notificationCount.ShouldBe(2);
    }

    [Fact]
    public void Disposal_StopsNotifications()
    {
        // Arrange
        var rootObject = new TestObservableObject
        {
            NestedProperty = new NestedTestObservableObject()
        };
        var manager = new BatchedNotificationManager();
        bool notificationReceived = false;

        using (var _ = new PropertyPathSubscription(
                   rootObject,
                   "NestedProperty.ValueProperty",
                   () => { notificationReceived = true; },
                   manager))
        {
            // dispose
        }

        rootObject.NestedProperty.ValueProperty = 50;

        // Assert
        notificationReceived.ShouldBeFalse();
    }

    [Fact]
    public void NullIntermediateProperty_DoesNotThrow()
    {
        // Arrange
        var rootObject = new TestObservableObject
        {
            NestedProperty = null // NestedProperty is null
        };
        var manager = new BatchedNotificationManager();
        bool notificationReceived = false;

        using var subscription = new PropertyPathSubscription(
            rootObject,
            "NestedProperty.ValueProperty",
            () => { notificationReceived = true; },
            manager);

        // Act
        rootObject.NestedProperty = new NestedTestObservableObject();
        rootObject.NestedProperty.ValueProperty = 60;

        // Assert
        // Should receive notification after setting NestedProperty and changing ValueProperty
        notificationReceived.ShouldBeTrue();
    }
}
