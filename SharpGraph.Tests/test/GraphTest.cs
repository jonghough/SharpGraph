using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;

namespace SharpGraph
{
    [TestFixture]
    public class GraphTest
    {
        [Test]
        public void TestNodeAndEdgeEquality()
        {
            Node n1 = new Node("A");
            Node n2 = new Node("B");
            Assert.AreEqual(n1, new Node("A"));
            Assert.IsTrue(n1 == new Node("A"));
            Assert.AreNotEqual(n1, n2);
            Assert.IsTrue(n1 != n2);

            Node n3 = new Node("C");
            Edge e1 = new Edge(n1, n2);
            Edge e2 = new Edge(new Node("A"), new Node("B"));
            Edge e3 = new Edge(n1, n3);
            Edge e4 = new Edge(n3, n1);

            Assert.AreEqual(e1, e2);
            Assert.IsTrue(e1 == e2);
            Assert.IsTrue(e3 != e4);
            Assert.IsTrue(e3.From() == e4.To());
        }

        [Test]
        public void TestGraphCreation1()
        {
            var g = new Graph();
            g.AddEdge("A", "B");
            g.AddEdge("B", "C");
            g.AddEdge("C", "D");
            g.AddEdge("D", "E");
            g.AddEdge("A", "E");

            Assert.AreEqual(5, g.GetNodes().Count);
            Assert.IsTrue(g.IsConnected());

        }

        [Test]
        public void TestGraphCreation2()
        {
            var g = new Graph();

            Assert.Throws<Exception>(
                () => g.AddEdge("A", "A"));

        }

        [Test]
        public void TestMerge1()
        {
            var g1 = new Graph();
            g1.AddEdge("A", "B");
            g1.AddEdge("A", "C");
            g1.AddEdge("A", "D");
            g1.AddEdge("B", "E");
            g1.AddEdge("E", "F");

            var g2 = new Graph();
            g2.AddEdge("A", "R");
            g2.AddEdge("R", "S");
            g2.AddEdge("S", "T");

            var g3 = g1.MergeWith(g2);
            Assert.AreEqual(9, g3.GetNodes().Count);
        }

        [Test]
        public void TestMerge2()
        {
            var g1 = new Graph();
            g1.AddEdge("A", "B");
            g1.AddEdge("A", "C");
            g1.AddEdge("A", "D");
            g1.AddEdge("B", "E");
            g1.AddEdge("E", "F");

            var g2 = new Graph();
            g2.AddEdge("A", "B");
            g2.AddEdge("R", "S");
            g2.AddEdge("S", "T");

            g2.AddNode("U");

            var g3 = g1.MergeWith(g2);
            Assert.AreEqual(10, g3.GetNodes().Count);
            Assert.AreEqual(7, g3.GetEdges().Count);
        }

        [Test]
        public void TestRemoveEdge1()
        {
            var g1 = new Graph();
            g1.AddEdge("A", "B");
            g1.AddEdge("A", "C");
            g1.AddEdge("A", "D");
            g1.AddEdge("B", "E");
            g1.AddEdge("E", "F");


            Assert.AreEqual(5, g1.GetEdges().Count);
            g1.RemoveEdge("A", "B");

            Assert.AreEqual(4, g1.GetEdges().Count);
        }

        [Test]
        public void TestRemoveEdge2()
        {
            var g1 = GraphGenerator.GenerateWheelGraph(4);
            var edgeCount = g1.GetEdges().Count;

            g1.RemoveEdge("center", "1");

            Assert.AreEqual(edgeCount - 1, g1.GetEdges().Count);
        }


        [Test]
        public void TestRemoveEdge3()
        {
            var g1 = GraphGenerator.CreateComplete(6);
            var edgeCount = g1.GetEdges().Count;

            Assert.AreEqual(15, g1.GetEdges().Count);
            bool success = g1.RemoveEdge("3", "1");
            Assert.IsFalse(success, "the edge with from=3, to=1 does not exist");

            Assert.AreEqual(15, g1.GetEdges().Count);

            var nodes = new HashSet<Node>();
            nodes.Add(new Node("3"));
            nodes.Add(new Node("1"));
            g1.RemoveEdge(nodes);
            Assert.AreEqual(14, g1.GetEdges().Count);
        }

        [Test]
        public void TestCopy()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(3);
            var g = GraphGenerator.CreateComplete(nodes);

            foreach (Node n in nodes)
            {
                var x = g.AddComponent<TestComponent>(n);
                x.Distance = 11.5f;
            }

            foreach (Edge e in g.GetEdges())
            {
                var a = g.AddComponent<TestComponent2>(e);
                a.x = 12f;
                a.y = 200f;
            }

            var h = g.Copy();
            foreach (Node n in h.GetNodes())
            {
                var x = h.GetComponent<TestComponent>(n);
                Assert.AreEqual(11.5f, x.Distance);
            }

            foreach (Edge e in h.GetEdges())
            {
                var a = g.GetComponent<TestComponent2>(e);
                Assert.AreEqual(12f, a.x);
                Assert.AreEqual(200f, a.y);
            }
        }
    }

    class TestComponent : NodeComponent
    {
        public float Distance { get; set; }

        public override void Copy(NodeComponent nodeComponent)
        {
            if (nodeComponent == null)
                throw new Exception("Null node component");
            if (nodeComponent is TestComponent)
                (nodeComponent as TestComponent).Distance = Distance;
            else throw new Exception("Type is not correct");
        }
    }

    class TestComponent2 : EdgeComponent
    {
        public float x { get; set; }
        public float y { get; set; }

        public override void Copy(EdgeComponent nodeComponent)
        {
            if (nodeComponent == null)
                throw new Exception("Null edge component");
            if (nodeComponent is TestComponent2)
            {
                (nodeComponent as TestComponent2).x = x;
                (nodeComponent as TestComponent2).y = y;
            }
            else throw new Exception("Type is not correct");
        }
    }
}