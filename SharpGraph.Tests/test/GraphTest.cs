// <copyright file="GraphTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class GraphTest
    {
        [Fact]
        public void TestNodeAndEdgeEquality()
        {
            var n1 = new Node("A");
            var n2 = new Node("B");
            Assert.Equal(n1, new Node("A"));
            Assert.True(n1 == new Node("A"));
            Assert.NotEqual(n1, n2);
            Assert.True(n1 != n2);

            var n3 = new Node("C");
            var e1 = new Edge(n1, n2);
            var e2 = new Edge(new Node("A"), new Node("B"));
            var e3 = new Edge(n1, n3);
            var e4 = new Edge(n3, n1);

            Assert.Equal(e1, e2);
            Assert.True(e1 == e2);
            Assert.True(e3 != e4);
            Assert.True(e3.From() == e4.To());
        }

        [Fact]
        public void TestGraphCreation1()
        {
            var g = new Graph();
            g.AddEdge("A", "B");
            g.AddEdge("B", "C");
            g.AddEdge("C", "D");
            g.AddEdge("D", "E");
            g.AddEdge("A", "E");

            Assert.Equal(5, g.GetNodes().Count);
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void TestGraphCreation2()
        {
            var g = new Graph();

            Assert.Throws<Exception>(() => g.AddEdge("A", "A"));
        }

        [Fact]
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
            Assert.Equal(9, g3.GetNodes().Count);
        }

        [Fact]
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
            Assert.Equal(10, g3.GetNodes().Count);
            Assert.Equal(7, g3.GetEdges().Count);
        }

        [Fact]
        public void TestRemoveEdge1()
        {
            var g1 = new Graph();
            g1.AddEdge("A", "B");
            g1.AddEdge("A", "C");
            g1.AddEdge("A", "D");
            g1.AddEdge("B", "E");
            g1.AddEdge("E", "F");

            Assert.Equal(5, g1.GetEdges().Count);
            g1.RemoveEdge("A", "B");

            Assert.Equal(4, g1.GetEdges().Count);
        }

        [Fact]
        public void TestRemoveEdge2()
        {
            var g1 = GraphGenerator.GenerateWheelGraph(4);
            var edgeCount = g1.GetEdges().Count;

            g1.RemoveEdge("center", "1");

            Assert.Equal(edgeCount - 1, g1.GetEdges().Count);
        }

        [Fact]
        public void TestRemoveEdge3()
        {
            var g1 = GraphGenerator.CreateComplete(6);
            _ = g1.GetEdges().Count;

            Assert.Equal(15, g1.GetEdges().Count);
            var success = g1.RemoveEdge("3", "1");
            Assert.False(success, "the edge with from=3, to=1 does not exist");

            Assert.Equal(15, g1.GetEdges().Count);

            var nodes = new HashSet<Node>();
            nodes.Add(new Node("3"));
            nodes.Add(new Node("1"));
            g1.RemoveEdge(nodes);
            Assert.Equal(14, g1.GetEdges().Count);
        }

        [Fact]
        public void TestCopy()
        {
            var nodes = NodeGenerator.GenerateNodes(3);
            var g = GraphGenerator.CreateComplete(nodes);

            foreach (var n in nodes)
            {
                var x = g.AddComponent<TestComponent>(n);
                x.Distance = 11.5f;
            }

            foreach (var e in g.GetEdges())
            {
                var a = g.AddComponent<TestComponent2>(e);
                a.X = 12f;
                a.Y = 200f;
            }

            var h = g.Copy();
            foreach (var n in h.GetNodes())
            {
                var x = h.GetComponent<TestComponent>(n);
                Assert.Equal(11.5f, x.Distance);
            }

            foreach (var e in h.GetEdges())
            {
                var a = g.GetComponent<TestComponent2>(e);
                Assert.Equal(12f, a.X);
                Assert.Equal(200f, a.Y);
            }
        }

        [Fact]
        public void TestGraphConstructorRegex1()
        {
            var g = new Graph("1..10");
            Assert.True(g != null);
            Assert.True(g.GetNodes().Count == 10);
        }

        [Fact]
        public void TestGraphConstructorRegex2()
        {
            var g = new Graph("a ->b, b -> c");
            Assert.True(g != null);
            Assert.True(g.GetEdges().Count == 2);
            Assert.Contains(new Node("a"), g.GetNodes());
            Assert.Contains(new Node("b"), g.GetNodes());
            Assert.Contains(new Node("c"), g.GetNodes());
            Assert.True(g.GetAdjacent(new Node("b")).Count == 2);
        }

        [Fact]
        public void TestGraphConstructorRegex3()
        {
            var g = new Graph(
                "node1 ->node2, node3 -> node4, node5->node6, node6-> node7, node7->node5"
            );
            Assert.True(g != null);
            Assert.True(g.GetNodes().Count == 7);
            for (var i = 1; i <= 7; i++)
            {
                Assert.Contains(new Node("node" + i), g.GetNodes());
            }

            Assert.True(g.GetAdjacent(new Node("node1")).Count == 1);
            Assert.True(g.GetAdjacent(new Node("node5")).Count == 2);
            Assert.True(g.GetAdjacent(new Node("node6")).Count == 2);
            Assert.True(g.GetAdjacent(new Node("node7")).Count == 2);
            Assert.True(g.GetConnectedComponents().Count == 3);
            Assert.Single(g.FindSimpleCycles());
        }

        [Fact]
        public void TestGraphConstructorRegex4()
        {
            Assert.Throws<GraphConstructorException>(() => new Graph("1..1"));
        }

        [Fact]
        public void TestGraphConstructorRegex5()
        {
            Assert.Throws<GraphConstructorException>(() => new Graph("10..1"));
        }

        [Fact]
        public void TestGraphConstructorRegex6()
        {
            Assert.Throws<GraphConstructorException>(() => new Graph("a->"));
        }

        [Theory]
        [InlineData("a -> b", 1)]
        [InlineData("1 -> 2, 2 -> other, 2->NodethreeX", 3)]
        [InlineData(
            @"n1 -> n2, n3 -> n4,n5->n6,n6->n7,n1->n5, 
        n2->n5, n3-> n5, n4->n6, n6->n8,  
        n4->n9,n6->n9",
            11
        )]
        [InlineData("abc -> def, xyz -> ABC,def->ABC", 3)]
        public void TestGraphConstructorRegexTheory(string constructorArg, int edgeCount)
        {
            var g = new Graph(constructorArg);
            Assert.NotNull(g);
            Assert.True(g.GetEdges().Count == edgeCount);
        }

        [Theory]
        [InlineData("a -> b", new string[] { "a", "b" })]
        [InlineData("123 -> ABC", new string[] { "123", "ABC" })]
        public void TestAddEdgeRegexTheory(string edgeString, string[] nodes)
        {
            var g = new Graph();
            g.AddEdge(edgeString);
            foreach (var node in nodes)
            {
                Assert.Contains(new Node(node), g.GetNodes());
            }

            Assert.NotNull(g.GetEdge(nodes[0], nodes[1]));
        }
    }

    internal class TestComponent : NodeComponent
    {
        public float Distance { get; set; }

        public override void Copy(NodeComponent nodeComponent)
        {
            if (nodeComponent == null)
            {
                throw new Exception("Null node component");
            }

            if (nodeComponent is TestComponent)
            {
                (nodeComponent as TestComponent).Distance = this.Distance;
            }
            else
            {
                throw new Exception("Type is not correct");
            }
        }
    }

    internal class TestComponent2 : EdgeComponent
    {
        public float X { get; set; }

        public float Y { get; set; }

        public override void Copy(EdgeComponent nodeComponent)
        {
            if (nodeComponent == null)
            {
                throw new Exception("Null edge component");
            }

            if (nodeComponent is TestComponent2)
            {
                (nodeComponent as TestComponent2).X = this.X;
                (nodeComponent as TestComponent2).Y = this.Y;
            }
            else
            {
                throw new Exception("Type is not correct");
            }
        }
    }
}
