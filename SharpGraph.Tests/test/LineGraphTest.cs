// <copyright file="LineGraphTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license. See LICENSE file in the samples root for full license information.
// </copyright>

using Xunit;

namespace SharpGraph
{
    public class LineGraphTest
    {
        [Fact]
        public void TestLineGraphOfComplete()
        {
            var nodes = NodeGenerator.GenerateNodes(7);
            var k7 = GraphGenerator.CreateComplete(nodes);

            var lineGraph = LineGraph.GenerateLineGraph(k7, new LineGraphBuilder());

            Assert.Equal(21, k7.GetEdges().Count);
            Assert.Equal(21 * 10 / 2, lineGraph.GetEdges().Count);
        }

        [Fact]
        public void TestLineGraphOfCycle1()
        {
            var c10 = GraphGenerator.GenerateCycle(10);

            var lineGraph = LineGraph.GenerateLineGraph(c10, new LineGraphBuilder());

            Assert.Equal(10, c10.GetEdges().Count);
            Assert.Equal(10, lineGraph.GetEdges().Count);
            Assert.Equal(10, c10.GetNodes().Count);
            Assert.Equal(10, lineGraph.GetNodes().Count);
        }

        private class LineGraphBuilder : ILineGraphBuilder
        {
            public Edge CreateEdge(Node nodeFrom, Node nodeTo)
            {
                return new Edge(nodeFrom, nodeTo);
            }

            public Node CreateNode(Edge edge)
            {
                var bn = new Node("(" + edge.From().GetLabel() + ", " + edge.To().GetLabel() + ")");
                return bn;
            }
        }
    }
}
