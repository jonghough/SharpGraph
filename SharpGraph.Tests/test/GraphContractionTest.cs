// <copyright file="GraphContractionTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class GraphContractionTest
    {
        [Theory]
        [InlineData(6, "1", "2", 5, 10, 4)]
        [InlineData(10, "2", "6", 9, 36, 8)]
        public void TestContractEdgeFromComplete(
            uint totalNodeCount,
            string nodeToKeep,
            string nodeToRemove,
            int expectedNumberOfNodes,
            int expectedNumberOfEdges,
            int expectedAdjacentToKeptNode
        )
        {
            var edgeToRemove = new Edge(nodeToKeep, nodeToRemove);
            var g = GraphGenerator.CreateComplete(totalNodeCount);
            var h = g.ContractEdge(edgeToRemove, new Node(nodeToKeep));

            Assert.Equal(expectedNumberOfNodes, h.GetNodes().Count);
            Assert.Equal(expectedNumberOfEdges, h.GetEdges().Count);
            Assert.Equal(expectedAdjacentToKeptNode, h.GetAdjacent(new Node(nodeToKeep)).Count);
        }

        [Fact]
        public void TestContractionKeepsComponents()
        {
            var e1 = new Edge("1", "2");
            var e2 = new Edge("2", "3");
            var e3 = new Edge("2", "4");
            var e4 = new Edge("5", "8");
            var edges = new List<Edge> { e1, e2, e3, e4 };
            var g = new Graph(edges);
            foreach (var e in g.GetEdges())
            {
                g.AddComponent<EdgeDirection>(e);
            }

            var h = g.ContractEdge(e1, new Node("1"));

            Assert.Equal(5, h.GetNodes().Count);
            Assert.Equal(3, h.GetEdges().Count);
            Assert.Equal(2, h.GetAdjacent(new Node("1")).Count);

            // edge that was not updated should have kept the component
            Assert.True(h.HasComponent<EdgeDirection>(e4));
        }
    }
}
