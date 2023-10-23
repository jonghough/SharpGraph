// <copyright file="CentralityTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using Xunit;

namespace SharpGraph.Tests.Test
{
    public class CentralityTest
    {
        [Fact]
        public void TestCentralityOfCompleteGraph()
        {
            var k10 = GraphGenerator.CreateComplete(10);
            var degCen = k10.GetDegreeCentrality();
            foreach (var kvp in degCen)
            {
                // centrality is 0.9 for all nodes
                Assert.Equal(0.9f, kvp.Value);
            }
        }

        [Fact]
        public void TestCentrality2()
        {
            var g = new Graph();
            g.AddEdge("A", "B");
            g.AddEdge("A", "C");
            g.AddEdge("A", "D");
            g.AddEdge("A", "E");
            g.AddEdge("B", "C");
            var da = g.GetDegreeCentrality(new Node("A"));
            Assert.Equal(0.8f, da);

            var dd = g.GetDegreeCentrality(new Node("D"));
            Assert.Equal(0.2f, dd);
        }
    }
}
