// <copyright file="RandomTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license. See LICENSE file in the samples root for full license information.
// </copyright>

using System.Linq;
using Xunit;

namespace SharpGraph
{
    public class RandomTest
    {
        [Fact]
        public void RandomTest1()
        {
            var g = Graph.GenerateRandom(10, 5, false, 10);
            var ns = g.GetNodes().ToList();

            Assert.Equal(10, ns.Count);
            Assert.Equal(5, g.GetEdges().Count);
        }

        [Fact]
        public void RandomTest2()
        {
            var g = Graph.GenerateRandom(140, 125, false, 10);
            var ns = g.GetNodes().ToList();

            Assert.Equal(140, ns.Count);
            Assert.Equal(125, g.GetEdges().Count);
            foreach (var edge in g.GetEdges())
            {
                var comp = g.GetComponent<EdgeDirection>(edge);
                Assert.Null(comp);
            }
        }

        [Fact]
        public void RandomTest3()
        {
            var g = Graph.GenerateRandom(8, 15, true, 50);
            var ns = g.GetNodes().ToList();

            Assert.Equal(8, ns.Count);
            Assert.Equal(15, g.GetEdges().Count);
            foreach (var edge in g.GetEdges())
            {
                var comp = g.GetComponent<EdgeDirection>(edge);
                Assert.NotNull(comp);
            }
        }

        [Fact]
        public void RandomTest4()
        {
            var g = Graph.GenerateRandom(20, 10, false, 50);
            var ns = g.GetNodes().ToList();

            Assert.Equal(20, ns.Count);
            Assert.Equal(10, g.GetEdges().Count);

            // not enough edges to remain connected.
            Assert.False(g.IsConnected());
        }

        [Fact]
        public void RandomExceptionTest1()
        {
            Assert.Throws<RandomGraphException>(() => Graph.GenerateRandom(5, 11, false, 10));
        }

        [Fact]
        public void RandomExceptionTest2()
        {
            Assert.Throws<RandomGraphException>(() => Graph.GenerateRandom(5, -1, false, 2));
        }

        [Fact]
        public void RandomSpanningTreeTest1()
        {
            var g = Graph.GenerateRandomSpanningTree(10, 10);
            var ns = g.GetNodes().ToList();

            Assert.Equal(10, ns.Count);
            Assert.Equal(9, g.GetEdges().Count);

            // tree so there should be no cyclesw
            var simpleCycles = g.FindSimpleCycles();
            Assert.Empty(simpleCycles);
        }

        [Fact]
        public void RandomSpanningTreeExceptionTest1()
        {
            Assert.Throws<RandomGraphException>(() => Graph.GenerateRandomSpanningTree(2, -1));
        }
    }
}
