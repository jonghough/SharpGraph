// <copyright file="JohnsonTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using Xunit;

namespace SharpGraph
{
    public class JohnsonTest
    {
        [Fact]
        public void JohnsonTest1()
        {
            var nodes = NodeGenerator.GenerateNodes(4);
            var g = GraphGenerator.CreateComplete(nodes);

            foreach (var e in g.GetEdges())
            {
                var a = g.AddComponent<EdgeWeight>(e);
                a.Weight = 2;
                var b = g.AddComponent<EdgeDirection>(e);
                b.Direction = Direction.Forwards;
            }

            var minPath = g.FindShortestPaths();

            Assert.Equal(12, minPath.Count);
        }

        [Fact]
        public void JohnsonTest2()
        {
            var g = GraphGenerator.GenerateCycle(7);

            var r = new Random();
            foreach (var e in g.GetEdges())
            {
                var a = g.AddComponent<EdgeWeight>(e);
                a.Weight = r.NextSingle();
                var b = g.AddComponent<EdgeDirection>(e);
                b.Direction = Direction.Both;
            }

            var minPath = g.FindShortestPaths();

            // 7*6 all possible ordered pairs
            Assert.Equal(7 * 6, minPath.Count);
        }

        [Fact]
        public void JohnsonExceptionTest1()
        {
            var g = GraphGenerator.CreateComplete(5);

            var r = new Random();

            // do not add direcitons
            foreach (var e in g.GetEdges())
            {
                var a = g.AddComponent<EdgeWeight>(e);
                a.Weight = r.NextSingle();
            }

            // exception because no direction
            Assert.Throws<Exception>(() => g.FindShortestPaths());
        }

        [Fact]
        public void JohnsonExceptionTest2()
        {
            var g = GraphGenerator.CreateComplete(5);

            var r = new Random();

            // do not add weights
            foreach (var e in g.GetEdges())
            {
                var b = g.AddComponent<EdgeDirection>(e);
                b.Direction = Direction.Both;
            }

            // exception because no weights
            Assert.Throws<Exception>(() => g.FindShortestPaths());
        }
    }
}
