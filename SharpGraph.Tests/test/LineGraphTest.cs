using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class LineGraphTest
    {

        [Fact]
        public void TestLineGraphOfComplete()
        {
            var nodes = NodeGenerator.GenerateNodes(7);
            var K7 = GraphGenerator.CreateComplete(nodes);

            var lineGraph = LineGraph.GenerateLineGraph(K7, new LineGraphBuilder());

            Assert.Equal(21, K7.GetEdges().Count);
            Assert.Equal(21 * 10 / 2, lineGraph.GetEdges().Count);

        }

        [Fact]
        public void TestLineGraphOfCycle1()
        {
            var C10 = GraphGenerator.GenerateCycle(10);

            var lineGraph = LineGraph.GenerateLineGraph(C10, new LineGraphBuilder());

            Assert.Equal(10, C10.GetEdges().Count);
            Assert.Equal(10, lineGraph.GetEdges().Count);
            Assert.Equal(10, C10.GetNodes().Count);
            Assert.Equal(10, lineGraph.GetNodes().Count);

        }

        class LineGraphBuilder : ILineGraphBuilder
        {
            public Edge createEdge(Node nodeFrom, Node nodeTo)
            {
                return new Edge(nodeFrom, nodeTo);
            }

            public Node createNode(Edge edge)
            {
                var bn = new Node("(" + edge.From().GetLabel() + ", " + edge.To().GetLabel() + ")");
                return bn;
            }
        }
    }
}