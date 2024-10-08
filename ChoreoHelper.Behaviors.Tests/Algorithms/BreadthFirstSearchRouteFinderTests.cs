using System.Collections.Immutable;
using ChoreoHelper.Behaviors.Algorithms;
using Shouldly;

namespace ChoreoHelper.Behaviors.Tests.Algorithms;

[TestOf(typeof(BreadthFirstSearchRouteFinder))]
public static class BreadthFirstSearchRouteFinderTests
{
    [TestOf(typeof(BreadthFirstSearchRouteFinder), nameof(BreadthFirstSearchRouteFinder.FindAllRoutesAsync))]
    public sealed class FindAllRoutesAsyncTests
    {
        [Fact]
        public async Task ShouldReturnEmptyListWhenNoRequiredNodes()
        {
            // Arrange
            var distanceMatrix = new int[0, 0];
            var requiredNodes = ImmutableArray<int>.Empty;
            const int maxDistance = 10;

            // Act
            var routeFinder = new BreadthFirstSearchRouteFinder();
            var result = await routeFinder.FindAllRoutesAsync(
                distanceMatrix,
                requiredNodes,
                null,
                maxDistance);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ShouldReturnEmptyListWhenNoPathsExist()
        {
            // Arrange
            int[,] distanceMatrix = {
                { -1, -1 },
                { -1, -1 }
            };
            var requiredNodes = ImmutableArray.Create(0, 1);
            const int maxDistance = 10;

            // Act
            var routeFinder = new BreadthFirstSearchRouteFinder();
            var result = await routeFinder.FindAllRoutesAsync(
                distanceMatrix,
                requiredNodes,
                null,
                maxDistance);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ShouldFindRoutesInSimpleGraph()
        {
            // Arrange
            int[,] distanceMatrix = {
                { -1, 1, -1 },
                { 1, -1, 1 },
                { -1, 1, -1 }
            };
            var requiredNodes = ImmutableArray.Create(0, 2);
            const int maxDistance = 10;

            // Act
            var routeFinder = new BreadthFirstSearchRouteFinder();
            var result = await routeFinder.FindAllRoutesAsync(
                distanceMatrix,
                requiredNodes,
                null,
                maxDistance);

            // Assert
            result.ShouldSatisfyAllConditions(
                r => r.ShouldNotBeEmpty(),
                r => r.ShouldAllBe(route => route.HasVisitedAllRequiredNodes(requiredNodes))
            );
        }

        [Fact]
        public async Task ShouldNotReturnRoutesExceedingMaxDistance()
        {
            // Arrange
            int[,] distanceMatrix = {
                { -1, 5 },
                { 5, -1 }
            };
            var requiredNodes = ImmutableArray.Create(0, 1);
            const int maxDistance = 4;

            // Act
            var routeFinder = new BreadthFirstSearchRouteFinder();
            var result = await routeFinder.FindAllRoutesAsync(
                distanceMatrix,
                requiredNodes,
                null,
                maxDistance);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ShouldRespectCancellationToken()
        {
            // Arrange
            int[,] distanceMatrix = {
                { -1, 1 },
                { 1, -1 }
            };
            var requiredNodes = ImmutableArray.Create(0, 1);
            const int maxDistance = 10;
            var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act
            var routeFinder = new BreadthFirstSearchRouteFinder();
            var result = await routeFinder.FindAllRoutesAsync(
                distanceMatrix,
                requiredNodes,
                null,
                maxDistance,
                cts.Token);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ShouldHandlePenaltiesForRevisitingNodes()
        {
            // Arrange
            int[,] distanceMatrix = {
                { -1, 1, -1 },
                { 1, -1, 1 },
                { -1, 1, -1 }
            };
            var requiredNodes = ImmutableArray.Create(0, 1, 2);
            const int maxDistance = 10;

            // Act
            var routeFinder = new BreadthFirstSearchRouteFinder();
            var result = await routeFinder.FindAllRoutesAsync(
                distanceMatrix,
                requiredNodes,
                null,
                maxDistance);

            // Assert
            List<Action> conditions = new();
            foreach (var route in result)
            {
                conditions.Add(() => route.VisitedNodes
                    .GroupBy(n => n)
                    .Select(g => g.Count() - 1)
                    .Sum()
                    .ShouldBeLessThan(4));

                conditions.Add(() => route.Distance
                    .ShouldBeLessThanOrEqualTo(maxDistance));
            }

            result.ShouldSatisfyAllConditions(conditions.ToArray());
        }

        [Fact]
        public async Task ShouldSkipRoutesWithTooManyRepetitions()
        {
            // Arrange
            int[,] distanceMatrix = {
                { -1, 1 },
                { 1, -1 }
            };
            var requiredNodes = ImmutableArray.Create(0, 1);
            const int maxDistance = 50;

            // Act
            var routeFinder = new BreadthFirstSearchRouteFinder();
            var result = await routeFinder.FindAllRoutesAsync(
                distanceMatrix,
                requiredNodes,
                null,
                maxDistance);

            // Assert
            List<Action> conditions = new();
            foreach (var route in result)
            {
                conditions.Add(() => route.VisitedNodes
                    .GroupBy(n => n)
                    .Select(g => g.Count() - 1)
                    .Sum()
                    .ShouldBeLessThan(4));

                conditions.Add(() => route.Distance
                    .ShouldBeLessThanOrEqualTo(maxDistance));
            }

            result.ShouldSatisfyAllConditions(conditions.ToArray());
        }
    }
}