using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class BiconnectedComponentTest
    {
        [Fact]
        public void BiconnectedTest1()
        {
            var g1 = GraphGenerator.CreateComplete(4, "A_");
            var g2 = GraphGenerator.CreateComplete(4, "B_");
            var g = g1.MergeWith(g2);

            var c = g.FindBiconnectedComponents();
            Assert.Equal(2, c.Count);
        }

        [Fact]
        public void BiconnectedTest2()
        {
            var g1 = GraphGenerator.CreateComplete(3, "A_");
            var g2 = GraphGenerator.CreateComplete(3, "B_");
            var g3 = g1.MergeWith(g2);
            var edges = g3.GetEdges();
            edges.Add(new Edge(new Node("A_0"), new Node("B_1")));
            var g = new Graph(edges);
            var c = g.FindBiconnectedComponents();
            // bcc's are left complete, right complete, edge in middle.
            Assert.Equal(3, c.Count);

            // add another edge 
            edges.Add(new Edge(new Node("A_1"), new Node("B_0")));
            var gn = new Graph(edges);
            var c2 = gn.FindBiconnectedComponents();
            //now all biconnected

            Assert.Single(c2);
        }

        [Fact]
        public void BiconnectedTest3()
        {
            var g = GraphGenerator.CreateComplete(2, "A_");
            var c = g.FindBiconnectedComponents();
            Assert.Single(c);
        }
    }
}