using ChoreoHelper.Behaviors.Algorithms;
using Shouldly;

namespace ChoreoHelper.Behaviors.Tests.Algorithms;

[TestOf(typeof(DepthFirstSearchUnreachableIslandsFinder))]
public static class DepthFirstSearchUnreachableIslandsFinderTests
{
    [TestOf(
        typeof(DepthFirstSearchUnreachableIslandsFinder),
        nameof(DepthFirstSearchUnreachableIslandsFinder.FindUnreachableIslands))]
    public sealed class FindUnreachableIslandsTests
    {
        [Fact]
        public void ShouldFindUnreachableIslands()
        {
            // Arrange
            var matrix = new[,]
            {
                { 0, 1, 0, 0 },
                { 1, 0, 0, 0 },
                { 0, 0, 0, 1 },
                { 0, 0, 1, 0 }
            };

            var expectedIslands = new List<List<int>>
            {
                new() { 0, 1 },
                new() { 2, 3 }
            };

            var finder = new DepthFirstSearchUnreachableIslandsFinder();

            // Act
            var result = finder.FindUnreachableIslands(matrix);

            // Sort the islands and nodes within islands for comparison
            var sortedResult = result
                .Select(island => island.OrderBy(node => node).ToList())
                .OrderBy(island => island.First())
                .ToList();

            var sortedExpected = expectedIslands
                .Select(island => island.OrderBy(node => node).ToList())
                .OrderBy(island => island.First())
                .ToList();

            // Assert
            sortedResult.ShouldBe(sortedExpected);
        }

        [Fact]
        public void ShouldReturnEmptyListForEmptyMatrix()
        {
            // Arrange
            var matrix = new int[0, 0];
            var finder = new DepthFirstSearchUnreachableIslandsFinder();

            // Act
            var result = finder.FindUnreachableIslands(matrix);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldFindSingleIsland()
        {
            // Arrange
            var matrix = new[,]
            {
                { 0, 1, 0 },
                { 1, 0, 1 },
                { 0, 1, 0 }
            };

            var expectedIslands = new List<List<int>>
            {
                new() { 0, 1, 2 }
            };
            var sortedExpectedIsland = expectedIslands[0].OrderBy(node => node).ToList();

            var finder = new DepthFirstSearchUnreachableIslandsFinder();

            // Act
            var result = finder.FindUnreachableIslands(matrix);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldHaveSingleItem(),
                () => result.FirstOrDefault()?.OrderBy(node => node).ShouldBe(sortedExpectedIsland)
            );
        }

        [Fact]
        public void ShouldHandleSingleNode()
        {
            // Arrange
            var matrix = new[,]
            {
                { 0 }
            };

            var expectedIslands = new List<List<int>>
            {
                new() { 0 }
            };

            var finder = new DepthFirstSearchUnreachableIslandsFinder();

            // Act
            var result = finder.FindUnreachableIslands(matrix);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldHaveSingleItem(),
                () => result.ShouldBe(expectedIslands)
            );
        }

        [Fact]
        public void ShouldHandleDisconnectedNodes()
        {
            // Arrange
            var matrix = new[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };

            var expectedIslands = new List<List<int>>
            {
                new() { 0 },
                new() { 1 },
                new() { 2 }
            };

            var finder = new DepthFirstSearchUnreachableIslandsFinder();

            // Act
            var result = finder.FindUnreachableIslands(matrix);

            // Sort the result for comparison
            var sortedResult = result
                .Select(island => island.OrderBy(node => node).ToList())
                .OrderBy(island => island.First())
                .ToList();

            // Assert
            sortedResult.ShouldBe(expectedIslands);
        }
    }
}