// <copyright file="MinCutTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using Xunit;

namespace SharpGraph
{
    public class MinCutTest
    {
        [Fact]
        public void TestMinCutOfComplete()
        {
            var nodes = NodeGenerator.GenerateNodes(8);
            var g = GraphGenerator.CreateComplete(nodes);

            var cutEdges = g.FindMinCut();
            Assert.True(cutEdges >= 7);
        }

        [Fact]
        public void TestMinCutOfCycle()
        {
            var nodes = NodeGenerator.GenerateNodes(8);
            var g = GraphGenerator.GenerateCycle(nodes);

            var cutEdges = g.FindMinCut();

            Assert.Equal(2, cutEdges);
        }

        [Fact]
        public void TestMinCutOfTree()
        {
            var nodes = NodeGenerator.GenerateNodes(18);
            var g = GraphGenerator.CreateComplete(nodes);
            var tree = g.GenerateSpanningTree();
            var gtree = new Graph(tree);
            var cutEdges = gtree.FindMinCut();

            Assert.Equal(1, cutEdges);
        }

        [Fact]
        public void TestMinCutMultiRun()
        {
            var nodes = NodeGenerator.GenerateNodes(10);
            var g = GraphGenerator.CreateComplete(nodes);
            var minSize = int.MaxValue;
            var runs = 1000;
            while (runs-- > 0)
            {
                var cutEdges = g.FindMinCut();
                var size = cutEdges;

                if (size < minSize)
                {
                    minSize = size;
                }
            }

            Assert.True(minSize >= 9);
        }
    }
}
