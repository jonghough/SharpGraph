// <copyright file="FloydWarshallTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using Xunit;

namespace SharpGraph.Tests.Test
{
    public class FloydWarshallTest
    {
        [Fact]
        public void TestDistancesInK4()
        {
            var g = GraphGenerator.CreateComplete(4);
            var nodes = new List<Node>(g.GetNodes());
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[1] }).Value
            ).Weight = 10;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[2] }).Value
            ).Weight = 1;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[3] }).Value
            ).Weight = 3;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[2] }).Value
            ).Weight = 3;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[3] }).Value
            ).Weight = 2;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[2], nodes[3] }).Value
            ).Weight = 9;

            var distances = g.GetDistances();
            Assert.Equal(4, distances[(nodes[0], nodes[1])]);
            Assert.Equal(4, distances[(nodes[1], nodes[0])]);
            Assert.Equal(1, distances[(nodes[0], nodes[2])]);
            Assert.Equal(3, distances[(nodes[0], nodes[3])]);
            Assert.Equal(3, distances[(nodes[1], nodes[2])]);
            Assert.Equal(2, distances[(nodes[1], nodes[3])]);
            Assert.Equal(4, distances[(nodes[2], nodes[3])]);
        }

        [Fact]
        public void TestDistancesInC6()
        {
            var g = GraphGenerator.GenerateCycle(6);
            g.AddComponent<EdgeWeight>(g.GetEdge("0", "1").Value).Weight = 18;
            g.AddComponent<EdgeWeight>(g.GetEdge("1", "2").Value).Weight = 2;
            g.AddComponent<EdgeWeight>(g.GetEdge("2", "3").Value).Weight = 7;
            g.AddComponent<EdgeWeight>(g.GetEdge("3", "4").Value).Weight = 1;
            g.AddComponent<EdgeWeight>(g.GetEdge("4", "5").Value).Weight = 6;
            g.AddComponent<EdgeWeight>(g.GetEdge("5", "0").Value).Weight = 1;

            var distances = g.GetDistances();

            Assert.Equal(17, distances[(new Node("0"), new Node("1"))]);
            Assert.Equal(2, distances[(new Node("1"), new Node("2"))]);
            Assert.Equal(16, distances[(new Node("1"), new Node("5"))]);
        }
    }
}
