using ChoreoHelper.Behaviors.Algorithms;

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
            Assert.Equal(sortedExpected.Count, sortedResult.Count);

            for (int i = 0; i < sortedExpected.Count; i++)
            {
                Assert.Equal(sortedExpected[i], sortedResult[i]);
            }
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
            Assert.Empty(result);
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

            var finder = new DepthFirstSearchUnreachableIslandsFinder();

            // Act
            var result = finder.FindUnreachableIslands(matrix);

            // Assert
            Assert.Single(result);

            var sortedResultIsland = result[0].OrderBy(node => node).ToList();
            var sortedExpectedIsland = expectedIslands[0].OrderBy(node => node).ToList();

            Assert.Equal(sortedExpectedIsland, sortedResultIsland);
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
            Assert.Single(result);
            Assert.Equal(expectedIslands[0], result[0]);
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
            Assert.Equal(expectedIslands.Count, sortedResult.Count);

            for (int i = 0; i < expectedIslands.Count; i++)
            {
                Assert.Equal(expectedIslands[i], sortedResult[i]);
            }
        }
    }
}