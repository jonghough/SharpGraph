// <copyright file="TrianglesTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using Xunit;

namespace SharpGraph
{
    public class TrainglesTest
    {
        [Fact]
        public void TestCycleGraphTriangles1()
        {
            var g = GraphGenerator.GenerateCycle(3);
            var triangles = g.FindTriangles();

            Assert.Single(triangles);
        }

        [Fact]
        public void TestCycleGraphTriangles2()
        {
            var g = GraphGenerator.GenerateCycle(5);
            var triangles = g.FindTriangles();
            Assert.Empty(triangles);
        }

        [Fact]
        public void TestTriangles3()
        {
            var g = new Graph();
            g.AddEdge("1", "2");
            g.AddEdge("2", "3");
            g.AddEdge("3", "4");
            g.AddEdge("4", "1");
            g.AddEdge("1", "3");
            var triangles = g.FindTriangles();

            Assert.Equal(2, triangles.Count);
        }

        [Fact]
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

            Assert.Single(triangles);

            g.AddEdge("1", "4");
            triangles = g.FindTriangles();
            Assert.Equal(3, triangles.Count);
        }

        [Fact]
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

            Assert.Single(triangles);

            triangles = g.FindTriangles(new Node("4"));

            Assert.Empty(triangles);

            g.AddEdge("1", "4");
            triangles = g.FindTriangles(new Node("1"));

            Assert.Equal(3, triangles.Count);
        }

        [Fact]
        public void TransitivityTest1()
        {
            var g = GraphGenerator.CreateComplete(5);
            var n = g.Transitivity();
            Assert.Equal(1, n);
        }
    }
}
