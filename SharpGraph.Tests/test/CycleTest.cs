// <copyright file="CycleTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using Xunit;

namespace SharpGraph
{
    public class CompleteGraphTest
    {
        [Fact]
        public void CompleteGenerationTest()
        {
            // complete graph on 10 nodes ~ 10 * 9 * 0.5 = 45 edges.
            var nodes = NodeGenerator.GenerateNodes(10);
            var g = GraphGenerator.CreateComplete(nodes);
            Assert.Equal(45, g.GetEdges().Count);
        }
    }

    public class OtherGraphTest
    {
        [Fact]
        public void CompleteGenerationTest()
        {
            const int nodeNum = 15;
            const int width = 3;
            var nodes = NodeGenerator.GenerateNodes(nodeNum);
            var g = GraphGenerator.GenerateGrid(nodes, width);
            Assert.Equal(
                ((width - 1) * (nodeNum / width)) + (width * ((nodeNum / width) - 1)),
                g.GetEdges().Count
            );
        }
    }

    public class CycleTesting
    {
        [Fact]
        public void CycleTestComplete4()
        {
            // ------- There are 7 possible cycles in the complete graph K4. --------//
            var nodes4 = NodeGenerator.GenerateNodes(4);
            var graph4 = GraphGenerator.CreateComplete(nodes4);
            _ = graph4.FindSimpleCycles();
            Assert.Equal(7, graph4.FindSimpleCycles().Count);
        }

        [Fact]
        public void CycleTestComplete5()
        {
            // ------- There are 37 possible cycles in the complete graph K5. --------//
            var nodes5 = NodeGenerator.GenerateNodes(5);
            var graph5 = GraphGenerator.CreateComplete(nodes5);
            Assert.Equal(37, graph5.FindSimpleCycles().Count);
        }

        [Fact]
        public void CycleTestComplete6()
        {
            // ------- There are 197 possible cycles in the complete graph K6. --------//
            var nodes6 = NodeGenerator.GenerateNodes(6);
            var graph6 = GraphGenerator.CreateComplete(nodes6);
            Assert.Equal(197, graph6.FindSimpleCycles().Count);
        }

        [Fact]
        public void CycleTestComplete7()
        {
            // ------- There are 1172 possible cycles in the complete graph K7. --------//
            var nodes7 = NodeGenerator.GenerateNodes(7);
            var graph7 = GraphGenerator.CreateComplete(nodes7);
            Assert.Equal(1172, graph7.FindSimpleCycles().Count);
        }

        [Fact]
        public void CycleTestCyclic()
        {
            // Test there is only one cycle in a ... cycle graph //
            var nodes5 = NodeGenerator.GenerateNodes(5);
            var cycleGraph = GraphGenerator.GenerateCycle(nodes5);
            Assert.Single(cycleGraph.FindSimpleCycles());
        }

        [Fact]
        public void CycleTestCyclicWithPath()
        {
            var ns = NodeGenerator.GenerateNodes(3);

            var g = GraphGenerator.GenerateCycle(ns);

            // nodex, nodey are jsut some nodes to add, that do not create any new cycles.
            g.AddEdge("Node_1", "nodex");
            g.AddEdge("nodex", "nodey");
            Assert.Single(g.FindSimpleCycles());
        }

        [Fact]
        public void ChordalTest1()
        {
            var nodes5 = NodeGenerator.GenerateNodes(5);
            var cycleGraph = GraphGenerator.GenerateCycle(nodes5);
            var chords = cycleGraph.IsChordal();

            Assert.False(chords);
        }
    }
}
