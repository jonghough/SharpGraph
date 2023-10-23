// <copyright file="SteinerTreeTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph.Tests.Test
{
    public class SteinerTreeTest
    {
        [Fact]
        public void TestSteinerTreeForK4_1()
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

            var treeSet = new HashSet<Node>();
            treeSet.Add(nodes[0]);
            treeSet.Add(nodes[1]);
            var tree = g.GenerateMinimalSpanningSteinerTree(treeSet);

            Assert.Contains(nodes[0], tree.GetNodes());
            Assert.Contains(nodes[1], tree.GetNodes());
            Assert.Contains(nodes[2], tree.GetNodes());
            Assert.True(tree.GetNodes().Count == 3);
        }

        [Fact]
        public void TestSteinerTreeForK4_2()
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

            var treeSet = new HashSet<Node>();
            treeSet.Add(nodes[0]);
            treeSet.Add(nodes[2]);
            var tree = g.GenerateMinimalSpanningSteinerTree(treeSet);

            Assert.Contains(nodes[0], tree.GetNodes());
            Assert.Contains(nodes[2], tree.GetNodes());
            Assert.True(tree.GetNodes().Count == 2);
        }

        [Fact]
        public void TestSteinerTreeForK6_1()
        {
            var g = GraphGenerator.CreateComplete(6);
            var nodes = new List<Node>(g.GetNodes());
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[1] }).Value
            ).Weight = 2.5f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[2] }).Value
            ).Weight = 12.35f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[3] }).Value
            ).Weight = 13f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[4] }).Value
            ).Weight = 83f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[5] }).Value
            ).Weight = 74f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[2] }).Value
            ).Weight = 1.6f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[3] }).Value
            ).Weight = 2.2f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[4] }).Value
            ).Weight = 400f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[5] }).Value
            ).Weight = 212f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[2], nodes[3] }).Value
            ).Weight = 1.4f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[2], nodes[4] }).Value
            ).Weight = 4.3f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[2], nodes[5] }).Value
            ).Weight = 916.545f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[3], nodes[4] }).Value
            ).Weight = 7.0f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[3], nodes[5] }).Value
            ).Weight = 9f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[4], nodes[5] }).Value
            ).Weight = 0.5f;

            var treeSet = new HashSet<Node>();
            treeSet.Add(nodes[0]);
            treeSet.Add(nodes[1]);
            treeSet.Add(nodes[4]);
            treeSet.Add(nodes[5]);
            var tree = g.GenerateMinimalSpanningSteinerTree(treeSet);

            Assert.Contains(nodes[0], tree.GetNodes());
            Assert.Contains(nodes[1], tree.GetNodes());
            Assert.Contains(nodes[2], tree.GetNodes());
            Assert.Contains(nodes[4], tree.GetNodes());
            Assert.Contains(nodes[5], tree.GetNodes());
            Assert.True(tree.GetNodes().Count == 5);
            Assert.True(tree.FindSimpleCycles().Count == 0);
        }

        [Fact]
        public void TestSteinerTreeForK6_2()
        {
            var g = GraphGenerator.CreateComplete(6);
            var nodes = new List<Node>(g.GetNodes());
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[1] }).Value
            ).Weight = 346.7f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[2] }).Value
            ).Weight = 400.3f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[3] }).Value
            ).Weight = 123.54f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[4] }).Value
            ).Weight = 500.0f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[0], nodes[5] }).Value
            ).Weight = 100.543f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[2] }).Value
            ).Weight = 90.6f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[3] }).Value
            ).Weight = 901.3f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[4] }).Value
            ).Weight = 400.5f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[1], nodes[5] }).Value
            ).Weight = 5.456f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[2], nodes[3] }).Value
            ).Weight = 3.2f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[2], nodes[4] }).Value
            ).Weight = 1.9f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[2], nodes[5] }).Value
            ).Weight = 9.0f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[3], nodes[4] }).Value
            ).Weight = 2.0f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[3], nodes[5] }).Value
            ).Weight = 3.2f;
            g.AddComponent<EdgeWeight>(
                g.GetEdge(new HashSet<Node>() { nodes[4], nodes[5] }).Value
            ).Weight = 0.9f;

            var treeSet = new HashSet<Node>();
            treeSet.Add(nodes[0]);
            treeSet.Add(nodes[1]);
            var tree = g.GenerateMinimalSpanningSteinerTree(treeSet);

            Assert.Contains(nodes[0], tree.GetNodes());
            Assert.Contains(nodes[1], tree.GetNodes());
            Assert.Contains(nodes[5], tree.GetNodes());
            Assert.True(tree.GetNodes().Count == 3);
            Assert.True(tree.FindSimpleCycles().Count == 0);
        }

        [Fact]
        public void TestSteinerTreeForGrid1()
        {
            var ns = NodeGenerator.GenerateNodes(5 * 5 * 2);
            var g = GraphGenerator.Generate3DGrid(ns, 5, 2);
            var rand = new Random();
            g.GetEdges().ForEach(e => g.AddComponent<EdgeWeight>(e).Weight = rand.Next(1000));

            var treeSet = new HashSet<Node>();
            var nsList = new List<Node>(ns);
            treeSet.Add(nsList[0]);
            treeSet.Add(nsList[10]);
            treeSet.Add(nsList[20]);
            treeSet.Add(nsList[30]);
            treeSet.Add(nsList[40]);
            var tree = g.GenerateMinimalSpanningSteinerTree(treeSet);

            Assert.True(tree.FindSimpleCycles().Count == 0);
        }

        [Fact]
        public void TestSteinerTreeForGrid2()
        {
            var ns = NodeGenerator.GenerateNodes(7 * 3 * 3);
            var g = GraphGenerator.Generate3DGrid(ns, 3, 3);
            var rand = new Random();
            g.GetEdges().ForEach(e => g.AddComponent<EdgeWeight>(e).Weight = 1);

            var treeSet = new HashSet<Node>();
            var nsList = new List<Node>(ns);
            treeSet.Add(nsList[0]);
            treeSet.Add(nsList[(7 * 3 * 3) - 1]);
            var tree = g.GenerateMinimalSpanningSteinerTree(treeSet);
            System.Console.WriteLine(tree.GetNodes().Count);
            Assert.True(tree.GetNodes().Count == 11); // 7 down + 2 across + 2 deep
            Assert.True(tree.FindSimpleCycles().Count == 0);
        }
    }
}
