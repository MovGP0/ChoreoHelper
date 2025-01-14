using ReactiveUI.TestObjects;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ReactiveUI.Batching;

[TestOf(typeof(Subscription))]
public sealed class SubscriptionTests
{
    public SubscriptionTests(ITestOutputHelper testOutputHelper)
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
    public void SinglePropertyChange_NotifiesObserver()
    {
        // Arrange
        var rootObject = new TestObservableObject();
        var manager = new BatchedNotificationManager();
        var observer = new TestObserver<object>();

        using var subscription = new Subscription(
            rootObject,
            ["SimpleProperty"],
            observer,
            manager);

        // Act
        rootObject.SimpleProperty = "Changed Value";

        // Assert
        observer.ShouldSatisfyAllConditions(
            () => observer.ReceivedValues.Count.ShouldBe(1),
            () => observer.ReceivedValues[0].ShouldBe(rootObject));
    }

    [Fact]
    public void MultiplePropertyChanges_NotifiesObserverForEach()
    {
        // Arrange
        var rootObject = new TestObservableObject
        {
            NestedProperty = new NestedTestObservableObject()
        };
        var manager = new BatchedNotificationManager();
        var observer = new TestObserver<object>();

        using var subscription = new Subscription(
            rootObject,
            ["SimpleProperty", "NestedProperty.ValueProperty"],
            observer,
            manager);

        // Act
        rootObject.SimpleProperty = "Changed Value";
        rootObject.NestedProperty.ValueProperty = 99;

        // Assert
        observer.ShouldSatisfyAllConditions(
            o => o.ReceivedValues.Count.ShouldBe(2),
            o => o.ReceivedValues[0].ShouldBe(rootObject),
            o => o.ReceivedValues[1].ShouldBe(rootObject));
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
        var observer = new TestObserver<object>();

        using (var _ = new Subscription(
                   rootObject,
                   ["NestedProperty.ValueProperty"],
                   observer,
                   manager))
        {
            // dispose
        }

        // Act
        rootObject.NestedProperty.ValueProperty = 123;

        // Assert
        observer.ReceivedValues.Count.ShouldBe(0);
    }

    [Fact]
    public void BatchNotifications_AreAggregated()
    {
        // Arrange
        var rootObject = new TestObservableObject
        {
            NestedProperty = new NestedTestObservableObject()
        };
        var manager = new BatchedNotificationManager();
        var observer = new TestObserver<object>();

        using var subscription = new Subscription(
            rootObject,
            ["SimpleProperty", "NestedProperty.ValueProperty"],
            observer,
            manager);

        int receivedValuesCountInBatch;
        using (rootObject.BatchChangeNotifications())
        {
            // Act
            rootObject.SimpleProperty = "Changed Value";
            rootObject.NestedProperty.ValueProperty = 99;

            receivedValuesCountInBatch = observer.ReceivedValues.Count;
        }

        // Assert
        observer.ShouldSatisfyAllConditions(
            o => receivedValuesCountInBatch.ShouldBe(2),
            o => o.ReceivedValues.Count.ShouldBe(2),
            o => o.ReceivedValues[0].ShouldBe(rootObject));
    }

    [Fact]
    public void NestedBatching_BatchesAreHandledCorrectly()
    {
        // Arrange
        var rootObject = new TestObservableObject
        {
            NestedProperty = new NestedTestObservableObject()
        };
        var manager = new BatchedNotificationManager();
        var observer = new TestObserver<object>();

        using var subscription = new Subscription(
            rootObject,
            ["SimpleProperty", "NestedProperty.ValueProperty"],
            observer,
            manager);

        // Start outer batch
        int outerBatchReceivedValuesCount;
        int innerBatchReceivedValuesCount;
        using (rootObject.BatchChangeNotifications())
        {
            using (rootObject.BatchChangeNotifications())
            {
                rootObject.SimpleProperty = "Changed Value";
                rootObject.NestedProperty.ValueProperty = 99;
                innerBatchReceivedValuesCount = observer.ReceivedValues.Count;
            }

            outerBatchReceivedValuesCount = observer.ReceivedValues.Count;
        }

        // Assert after all batches have ended
        observer.ShouldSatisfyAllConditions(
            () => innerBatchReceivedValuesCount.ShouldBe(2),
            () => outerBatchReceivedValuesCount.ShouldBe(2),
            () => observer.ReceivedValues.Count.ShouldBe(2),
            () => observer.ReceivedValues[0].ShouldBe(rootObject));
    }
}
