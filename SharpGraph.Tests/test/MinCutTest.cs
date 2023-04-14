using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{
    [TestFixture]
    public class MinCutTest
    {

        [Test]
        public void TestMinCutOfComplete()
        {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes(8);
            var g = GraphGenerator.CreateComplete(nodes);

            int cutEdges = g.FindMinCut();
            Assert.IsTrue(cutEdges >= 7);
        }


        [Test]
        public void TestMinCutOfCycle()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(8);
            var g = GraphGenerator.GenerateCycle(nodes);

            int cutEdges = g.FindMinCut();

            Assert.AreEqual(2, cutEdges);
        }

        [Test]
        public void TestMinCutOfTree()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(18);
            var g = GraphGenerator.CreateComplete(nodes);
            var tree = g.GenerateSpanningTree();
            var gtree = new Graph(tree);
            int cutEdges = gtree.FindMinCut();

            Assert.AreEqual(1, cutEdges);
        }


        [Test]
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
            Assert.IsTrue(minSize >= 9);
        }
    }
}

