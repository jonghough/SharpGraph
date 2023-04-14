using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public class CliqueTest {
        [Test]
        public void cliqueTest2 () {
            var nodes = new List<Node>(NodeGenerator.GenerateNodes (6));

            Edge e1 = new Edge (nodes[4], nodes[1]);
            Edge e2 = new Edge (nodes[4], nodes[0]);
            Edge e3 = new Edge (nodes[0], nodes[1]);
            Edge e4 = new Edge (nodes[1], nodes[2]);
            Edge e5 = new Edge (nodes[2], nodes[3]);
            Edge e6 = new Edge (nodes[3], nodes[4]);
            Edge e7 = new Edge (nodes[3], nodes[5]);

            var edges = new List<Edge> ();
            edges.Add (e1);
            edges.Add (e2);
            edges.Add (e3);
            edges.Add (e4);
            edges.Add (e5);
            edges.Add (e6);
            edges.Add (e7);
            var g = new Graph (edges);
            var cliques = g.FindMaximalCliques ();
            Assert.AreEqual (5, cliques.Count);
        }
    }
}