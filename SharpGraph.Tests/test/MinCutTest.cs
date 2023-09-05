using System;
using Xunit;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{
    public class MinCutTest
    {
        [Fact]
        public void TestMinCutOfComplete()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(8);
            var g = GraphGenerator.CreateComplete(nodes);

            int cutEdges = g.FindMinCut();
            Assert.True(cutEdges >= 7);
        }

        [Fact]
        public void TestMinCutOfCycle()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(8);
            var g = GraphGenerator.GenerateCycle(nodes);

            int cutEdges = g.FindMinCut();

            Assert.Equal(2, cutEdges);
        }

        [Fact]
        public void TestMinCutOfTree()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(18);
            var g = GraphGenerator.CreateComplete(nodes);
            var tree = g.GenerateSpanningTree();
            var gtree = new Graph(tree);
            int cutEdges = gtree.FindMinCut();

            Assert.Equal(1, cutEdges);
        }

        [Fact]
        public void TestMinCutMultiRun()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(10);
            var g = GraphGenerator.CreateComplete(nodes);
            int minSize = int.MaxValue;
            int runs = 1000;
            while (runs-- > 0)
            {
                var cutEdges = g.FindMinCut();
                int size = cutEdges;

                if (size < minSize)
                    minSize = size;
            }
            Assert.True(minSize >= 9);
        }
    }
}
