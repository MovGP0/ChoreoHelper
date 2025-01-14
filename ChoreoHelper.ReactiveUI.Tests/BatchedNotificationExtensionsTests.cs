using ReactiveUI.TestObjects;

namespace ReactiveUI;

[TestOf(typeof(BatchedNotificationExtensions))]
public static class BatchedNotificationExtensionsTests
{
    [TestOf(typeof(BatchedNotificationExtensions), nameof(BatchedNotificationExtensions.WhenAnyBatched))]
    public sealed class WhenAnyBatchedTests
    {
        [Fact]
        public void ShouldBatchChangeNotifications()
        {
            // Arrange
            var testSubject = new TestBatchingReactiveObject
            {
                NestedObject1 = new TestNestedObject { NestedProperty = 5 },
                NestedObject2 = new TestNestedObject { NestedProperty = 5 }
            };

            List<TestBatchingReactiveObject> values = new();

            // Subscribe to property changes
            testSubject.WhenAnyBatched(
                e => e.SomeProperty,
                e => e.AnotherProperty,
                e => e.NestedObject1.NestedProperty,
                e => e.NestedObject2.NestedProperty
                )
                .Subscribe(ts => values.Add(ts));

            // Act
            using (testSubject.BatchChangeNotifications())
            {
                // Make changes inside the batch
                testSubject.SomeProperty = 5;
                testSubject.NestedObject1.NestedProperty = 10;
                testSubject.NestedObject2 = new TestNestedObject { NestedProperty = 15 };
            }

            // Assert
            values.ShouldSatisfyAllConditions(
                cp => cp.ShouldNotBeEmpty(),
                cp => cp.Count.ShouldBe(1),
                cp => cp[0].ShouldBe(testSubject)
            );
        }

        [Fact]
        public void ShouldTriggerImmediateNotificationWhenNoBatch()
        {
            // Arrange
            var testSubject = new TestBatchingReactiveObject
            {
                NestedObject1 = new TestNestedObject { NestedProperty = 5 }
            };

            List<TestBatchingReactiveObject> values = new();

            // Subscribe to property changes
            testSubject.WhenAnyBatched(
                e => e.NestedObject1.NestedProperty
            )
            .Subscribe(ts => values.Add(ts));

            // Act
            testSubject.NestedObject1.NestedProperty = 10;

            // Assert
            values.ShouldSatisfyAllConditions(
                cp => cp.ShouldNotBeEmpty(),
                cp => cp.Count.ShouldBe(1),
                cp => cp[0].ShouldBe(testSubject)
            );
        }

        [Fact]
        public void ShouldDisposeOldNestedObjectSubscriptionWhenReplacing()
        {
            // Arrange
            var testSubject = new TestBatchingReactiveObject
            {
                NestedObject1 = new TestNestedObject { NestedProperty = 5 }
            };

            List<TestBatchingReactiveObject> values = new();

            // Subscribe to property changes
            testSubject.WhenAnyBatched(
                e => e.NestedObject1.NestedProperty
            )
            .Subscribe(ts => values.Add(ts));

            // Act: replace the nested object
            testSubject.NestedObject1 = new TestNestedObject { NestedProperty = 15 };

            // Act: Modify the new nested object
            testSubject.NestedObject1.NestedProperty = 20;

            // Assert
            values.ShouldSatisfyAllConditions(
                cp => cp.ShouldNotBeEmpty(),
                cp => cp.Count.ShouldBe(2),
                cp => cp[1].ShouldBe(testSubject)
            );
        }
    }
}
