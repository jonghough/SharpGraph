using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpGraph
{
    [TestFixture]
    public class TrainglesTest
    {
        [Test]
        public void TestCycleGraphTriangles1()
        {
            var g = GraphGenerator.GenerateCycle(3);
            var triangles = g.FindTriangles();

            Assert.AreEqual(1, triangles.Count);
        }

        [Test]
        public void TestCycleGraphTriangles2()
        {
            var g = GraphGenerator.GenerateCycle(5);
            var triangles = g.FindTriangles();
            Assert.AreEqual(0, triangles.Count);
        }

        [Test]
        public void TestTriangles3()
        {
            var g = new Graph();
            g.AddEdge("1", "2");
            g.AddEdge("2", "3");
            g.AddEdge("3", "4");
            g.AddEdge("4", "1");
            g.AddEdge("1", "3");
            var triangles = g.FindTriangles();

            Assert.AreEqual(2, triangles.Count);
        }

        [Test]
        public void TestTriangles4()
        {
            var g = new Graph();
            g.AddEdge("1", "2");
            g.AddEdge("2", "3");
            g.AddEdge("3", "4");
            g.AddEdge("4", "5");
            g.AddEdge("5", "1");
            g.AddEdge("1", "3");
            var triangles = g.FindTriangles();

            Assert.AreEqual(1, triangles.Count);

            g.AddEdge("1", "4");
            triangles = g.FindTriangles();
            Assert.AreEqual(3, triangles.Count);
        }

        [Test]
        public void TestTrianglesForNode1()
        {
            var g = new Graph();
            g.AddEdge("1", "2");
            g.AddEdge("2", "3");
            g.AddEdge("3", "4");
            g.AddEdge("4", "5");
            g.AddEdge("5", "1");
            g.AddEdge("1", "3");
            var triangles = g.FindTriangles(new Node("1"));

            Assert.AreEqual(1, triangles.Count);

            triangles = g.FindTriangles(new Node("4"));

            Assert.AreEqual(0, triangles.Count);

            g.AddEdge("1", "4");
            triangles = g.FindTriangles(new Node("1"));
             
            Assert.AreEqual(3, triangles.Count);
        }

        [Test]
        public void TransitivityTest1()
        {
            var g = GraphGenerator.CreateComplete(5);
            int n = g.Transitivity();
            Assert.AreEqual(1, n);
        }
    }
}