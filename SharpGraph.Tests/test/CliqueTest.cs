// <copyright file="CliqueTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license. See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class CliqueTest
    {
        [Fact]
        public void CliqueTest2()
        {
            var nodes = new List<Node>(NodeGenerator.GenerateNodes(6));

            var e1 = new Edge(nodes[4], nodes[1]);
            var e2 = new Edge(nodes[4], nodes[0]);
            var e3 = new Edge(nodes[0], nodes[1]);
            var e4 = new Edge(nodes[1], nodes[2]);
            var e5 = new Edge(nodes[2], nodes[3]);
            var e6 = new Edge(nodes[3], nodes[4]);
            var e7 = new Edge(nodes[3], nodes[5]);

            var edges = new List<Edge>();
            edges.Add(e1);
            edges.Add(e2);
            edges.Add(e3);
            edges.Add(e4);
            edges.Add(e5);
            edges.Add(e6);
            edges.Add(e7);
            var g = new Graph(edges);
            var cliques = g.FindMaximalCliques();
            Assert.Equal(5, cliques.Count);
        }
    }
}
