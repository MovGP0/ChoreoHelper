using ChoreoHelper.Behaviors.Algorithms;
using Shouldly;

namespace ChoreoHelper.Behaviors.Tests.Algorithms;

[TestOf(typeof(Route))]
public static class RouteTests
{
    [TestOf(typeof(Route), ILObjectNames.Constructor)]
    public sealed class ConstructorTests
    {
        [Fact]
        public void ShouldCreateRoute()
        {
            var route = new Route(0);
            route.ShouldSatisfyAllConditions(
                r => r.Distance.ShouldBe(0),
                r => r.VisitedNodes.Count().ShouldBe(1),
                r => r.LastVisitedNode.ShouldBe(0),
                r => r.VisitedNodes.ToArray().ShouldBe([0]));
        }
    }

    [TestOf(typeof(Route), nameof(Route.Append))]
    public sealed class AppendTests
    {
        [Fact]
        public void ShouldAppendNode()
        {
            var initialRoute = new Route(0);
            var newRoute = initialRoute.Append(1, 10);

            newRoute.ShouldSatisfyAllConditions(
                r => r.Distance.ShouldBe(10),
                r => r.LastVisitedNode.ShouldBe(1),
                r => r.VisitedNodes.Count().ShouldBe(2),
                r => r.VisitedNodes.Reverse().ShouldBe([0, 1]));
        }

        [Fact]
        public void ShouldCreateNewRouteWithoutModifyingOriginal()
        {
            var initialRoute = new Route(0);
            var newRoute = initialRoute.Append(1, 10);

            initialRoute.ShouldSatisfyAllConditions(
                r => r.Distance.ShouldBe(0),
                r => r.LastVisitedNode.ShouldBe(0),
                r => r.VisitedNodes.Count().ShouldBe(1),
                r => r.VisitedNodes.Reverse().ShouldBe([0]));

            newRoute.ShouldSatisfyAllConditions(
                r => r.Distance.ShouldBe(10),
                r => r.LastVisitedNode.ShouldBe(1),
                r => r.VisitedNodes.Count().ShouldBe(2),
                r => r.VisitedNodes.Reverse().ShouldBe([0, 1]));
        }
    }

    [TestOf(typeof(Route), nameof(Route.HasVisitedAllRequiredNodes))]
    public sealed class HasVisitedAllRequiredNodesTests
    {
        [Fact]
        public void ShouldReturnTrueWhenAllRequiredNodesVisited()
        {
            var route = new Route(0).Append(1, 10).Append(2, 20);
            var requiredNodes = new[] { 0, 1, 2 };

            route.HasVisitedAllRequiredNodes(requiredNodes).ShouldBeTrue();
        }

        [Fact]
        public void ShouldReturnFalseWhenNotAllRequiredNodesVisited()
        {
            var route = new Route(0).Append(1, 10);
            var requiredNodes = new[] { 0, 1, 2 };

            route.HasVisitedAllRequiredNodes(requiredNodes).ShouldBeFalse();
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenRequiredNodesIsNull()
        {
            var route = new Route(0);

            Should.Throw<ArgumentNullException>(() => route.HasVisitedAllRequiredNodes(null!));
        }
    }

    [TestOf(typeof(Route), nameof(Route.Equals))]
    public sealed class EqualsTests
    {
        [Fact]
        public void ShouldBeEqualWhenVisitedNodesAndDistanceAreEqual()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(1, 10);

            route1.Equals(route2).ShouldBeTrue();
        }

        [Fact]
        public void ShouldNotBeEqualWhenVisitedNodesAreDifferent()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(2, 10);

            route1.Equals(route2).ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotBeEqualWhenDistancesAreDifferent()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(1, 20);

            route1.Equals(route2).ShouldBeFalse();
        }
    }

    [TestOf(typeof(Route), nameof(Route.GetHashCode))]
    public sealed class GetHashCodeTests
    {
        [Fact]
        public void ShouldHaveSameHashCodeWhenEqual()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(1, 10);

            route1.GetHashCode().ShouldBe(route2.GetHashCode());
        }

        [Fact]
        public void ShouldHaveDifferentHashCodesWhenNotEqual()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(1, 20);

            route1.GetHashCode().ShouldNotBe(route2.GetHashCode());
        }
    }

    [TestOf(typeof(Route), ILObjectNames.op_Equality)]
    public sealed class EqualityTest
    {
        [Fact]
        public void ShouldReturnTrueWhenRoutesAreEqual()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(1, 10);

            (route1 == route2).ShouldBeTrue();
        }

        [Fact]
        public void ShouldReturnFalseWhenRoutesAreNotEqual()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(2, 10);

            (route1 == route2).ShouldBeFalse();
        }
    }

    [TestOf(typeof(Route), ILObjectNames.op_Inequality)]
    public sealed class InequalityTest
    {
        [Fact]
        public void ShouldReturnTrueWhenRoutesAreNotEqual()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(2, 10);

            (route1 != route2).ShouldBeTrue();
        }

        [Fact]
        public void ShouldReturnFalseWhenRoutesAreEqual()
        {
            var route1 = new Route(0).Append(1, 10);
            var route2 = new Route(0).Append(1, 10);

            (route1 != route2).ShouldBeFalse();
        }
    }

    [TestOf(typeof(Route), nameof(Route.VisitedNodes))]
    public sealed class VisitedNodesTests
    {
        [Fact]
        public void ShouldMaintainCorrectOrderOfVisitedNodes()
        {
            var route = new Route(0).Append(1, 10).Append(2, 20);

            route.VisitedNodes.Reverse().ShouldBe([0, 1, 2]);
        }
    }
}