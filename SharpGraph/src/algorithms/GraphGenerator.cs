﻿// <copyright file="GraphGenerator.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace SharpGraph
{
    public static class GraphGenerator
    {
        /// <summary>
        /// Creates a <i>complete graph</i> for the given input size. The node names are
        /// the stringified numbers less than or equal to the given size.
        /// </summary>
        /// <param name="size">number of nodes in the complete graph.</param>
        /// <returns>A complete graph.</returns>
        public static Graph CreateComplete(uint size)
        {
            var hs = new HashSet<Node>();
            for (var i = 0; i < size; i++)
            {
                hs.Add(new Node(string.Empty + i));
            }

            return CreateComplete(hs);
        }

        /// <summary>
        /// Creates a <i>complete graph</i> for the given input arguments. The input argument
        /// should be the names of the nodes.
        /// </summary>
        /// <param name="args">the names of the nodes in the generated graph.</param>
        /// <returns>A complete graph.</returns>
        public static Graph CreateComplete(params string[] args)
        {
            var hs = new HashSet<Node>();
            for (var i = 0; i < args.Length; i++)
            {
                hs.Add(new Node(args[i]));
            }

            return CreateComplete(hs);
        }

        /// <summary>
        /// Creates a complete graph generated by the given set of nodes. Each of the nodes
        /// in the input argument will become a node of the graph.
        /// </summary>
        /// <param name="nodeSet">set of nodes.</param>
        /// <returns>A complete graph.</returns>
        public static Graph CreateComplete(HashSet<Node> nodeSet)
        {
            if (nodeSet.Count == 0)
            {
                throw new ArgumentException(
                    $"Size of node set is {nodeSet.Count}, but must be at least 1."
                );
            }

            if (nodeSet.Count == 1)
            {
                return new Graph(nodeSet);
            }

            var nodeList = new List<Node>(nodeSet);
            var edgeList = new List<Edge>();
            for (var i = 0; i < nodeList.Count - 1; i++)
            {
                for (var j = i + 1; j < nodeList.Count; j++)
                {
                    var e = new Edge(nodeList[i], nodeList[j]);
                    edgeList.Add(e);
                }
            }

            var graph = new Graph(edgeList);
            return graph;
        }

        public static Graph CreateComplete(int size, string nodePrefix)
        {
            if (size == 0)
            {
                throw new ArgumentException($"Size of node set is {size}, but must be at least 1.");
            }

            if (size == 1)
            {
                var nodeSet = new HashSet<Node> { new Node(nodePrefix + "0") };

                return new Graph(nodeSet);
            }

            var nodes = new HashSet<Node>();
            for (var i = 0; i < size; i++)
            {
                var n = new Node(nodePrefix + string.Empty + i);
                nodes.Add(n);
            }

            var nodeList = new List<Node>(nodes);
            var edgeList = new List<Edge>();
            for (var i = 0; i < nodeList.Count - 1; i++)
            {
                for (var j = i + 1; j < nodeList.Count; j++)
                {
                    var e = new Edge(nodeList[i], nodeList[j]);
                    edgeList.Add(e);
                }
            }

            var graph = new Graph(edgeList);
            return graph;
        }

        public static Graph GenerateRandomGraph(HashSet<Node> nodes, float edgeProb)
        {
            var ep = edgeProb;
            ep =
                ep < 0 ? 0
                : ep > 1.0f ? 1.0f
                : ep;
            var compG = CreateComplete(nodes);
            var rand = new Random(Guid.NewGuid().GetHashCode());

            for (var i = compG.GetEdges().Count - 1; i >= 0; i--)
            {
                var r = (float)rand.NextDouble();
                if (r > ep)
                {
                    compG.GetEdges().RemoveAt(i);
                }
            }

            // include the possibility nodes have no edges, which would have
            // been removed automatically by CompleteGeneration.create(),
            // put them back here.
            return new Graph(compG.GetEdges(), nodes);
        }

        public static Graph GenerateBipartiteComplete(uint leftCount, uint rightCount)
        {
            var leftNodes = new HashSet<Node>();
            var rightNodes = new HashSet<Node>();
            for (var i = 0; i < leftCount; i++)
            {
                leftNodes.Add(new Node(string.Empty + i));
            }

            for (var i = 0; i < rightCount; i++)
            {
                rightNodes.Add(new Node(string.Empty + (i + leftCount)));
            }

            return GenerateBipartiteComplete(leftNodes, rightNodes);
        }

        /// <summary>
        /// Generates the complete bipartite graph with left sdie having. <code>leftNodes</code> nodes and
        /// the right side having. <code>rightNodes</code> nodes. Each node on left side is adjacent to each and only the
        /// nodes on the right side, and vice versa.
        /// </summary>
        /// <param name="leftNodes">nodes for the left side.</param>
        /// <param name="rightNodes">nodes for the right side.</param>
        /// <returns>Complete bipartite graph.</returns>
        public static Graph GenerateBipartiteComplete(
            HashSet<Node> leftNodes,
            HashSet<Node> rightNodes
        )
        {
            var ls = leftNodes.Count;
            var rs = rightNodes.Count;
            if (ls < 1 || rs < 1)
            {
                throw new Exception(
                    "Cannot Create Graph. Number of nodes in left and right partitions must be at least 1."
                );
            }

            var edges = new List<Edge>();
            foreach (var ln in leftNodes)
            {
                foreach (var rn in rightNodes)
                {
                    edges.Add(new Edge(ln, rn));
                }
            }

            return new Graph(edges);
        }

        /// <summary>
        /// Generates the complete bipartite graph with left sdie having. <code>leftNodeCount</code> nodes and
        /// the right side having. <code>rightNodeCount</code> nodes. Each node on left side is adjacent to each and only the
        /// nodes on the right side, and vice versa.
        /// </summary>
        /// <param name="leftNodeCount">nodes for the left side.</param>
        /// <param name="rightNodeCount">nodes for the right side.</param>
        /// <returns>Complete bipartite graph.</returns>
        public static Graph GenerateBipartiteComplete(int leftNodeCount, int rightNodeCount)
        {
            var leftNodes = Enumerable
                .Range(0, leftNodeCount)
                .Select(i => new Node("L" + i.ToString()))
                .ToHashSet();
            var rightNodes = Enumerable
                .Range(0, rightNodeCount)
                .Select(i => new Node("R" + i.ToString()))
                .ToHashSet();

            return GenerateBipartiteComplete(leftNodes, rightNodes);
        }

        /// <summary>
        /// Creates a cyclic graph on n vertices, where n is the given arguemtn positive integer.
        ///
        /// </summary>
        /// <param name="size">number of nodes in the generated cyclic graph.</param>
        /// <returns>A cyclic graph.</returns>
        public static Graph GenerateCycle(uint size)
        {
            var hs = new HashSet<Node>();
            for (var i = 0; i < size; i++)
            {
                hs.Add(new Node(string.Empty + i));
            }

            return GenerateCycle(hs);
        }

        /// <summary>
        /// Creates a cyclic graph produced from the given set of nodes, where
        /// each node becomes a node of the created graph.
        /// </summary>
        /// <param name="nodeSet">nodes of the cyclic graph.</param>
        /// <returns>A cyclic graph.</returns>
        public static Graph GenerateCycle(HashSet<Node> nodeSet)
        {
            var len = nodeSet.Count;
            var nodeList = new List<Node>(nodeSet);
            var edges = new List<Edge>();
            for (var i = 0; i < len - 1; i++)
            {
                var e = new Edge(nodeList[i], nodeList[i + 1]);
                edges.Add(e);
            }

            edges.Add(new Edge(nodeList[len - 1], nodeList[0]));
            return new Graph(edges);
        }

        ///
        /// <returns></returns><summary>
        /// Creates a barbell graph, which is defined as two complete graphs connected by a single edge.
        /// The <i>leftSet</i> and. <i>rightSet>\i? arguments give the nodes of the two generated complete graphs.
        ///
        /// The connecting edge is incident with arbitrary nodes on each of the complete graphs.
        /// </summary>
        /// <param name="leftSet">nodes of the "left" side barbell</param>
        /// <param name="rightSet">nodes of the "right" side barbell</param>
        /// <returns>A barbell graph</returns>
        public static Graph GenerateBarbell(HashSet<Node> leftSet, HashSet<Node> rightSet)
        {
            var gLeft = CreateComplete(leftSet);
            var gRight = CreateComplete(rightSet);

            var left = new List<Node>(gLeft.GetNodes())[0];
            var right = new List<Node>(gRight.GetNodes())[0];

            // the bar edge.
            var e = new Edge(left, right);

            var allEdges = gLeft.GetEdges();
            allEdges.AddRange(gRight.GetEdges());
            allEdges.Add(e);

            return new Graph(allEdges);
        }

        /// <summary>
        /// Creates a barbell graph, which is defined as two complete graphs connected by a single edge.
        /// The <i>leftSize</i> and. <i>rightSize>\i? arguments give the number of nodes of the two generated complete graphs.
        ///
        /// The connecting edge is incident with arbitrary nodes on each of the complete graphs.
        /// </summary>
        /// <param name="leftSize">number of nodes of the "left" side barbell</param>
        /// <param name="rightSize">number of nodes of the "right" side barbell</param>
        /// <returns>A barbell graph</returns>
        public static Graph GenerateBarbell(int leftSize, int rightSize)
        {
            if (leftSize < 1 || rightSize < 1)
            {
                throw new Exception(
                    "To generate a barbell graph. Left node count and right node count must be positive."
                );
            }

            var nodes = new List<Node>(NodeGenerator.GenerateNodes(leftSize + rightSize));

            var left = new HashSet<Node>();
            var right = new HashSet<Node>();

            for (var i = 0; i < leftSize + rightSize; i++)
            {
                if (i < leftSize)
                {
                    left.Add(nodes[i]);
                }
                else
                {
                    right.Add(nodes[i]);
                }
            }

            return GenerateBarbell(left, right);
        }

        public static Graph GenerateGrid(int width, int height, string nodePrefix = "")
        {
            if (width < 1 || height < 1)
            {
                throw new NonPositiveArgumentException(
                    $"Non positive argument in width and height: {width}, {height}."
                );
            }

            var edgeList = new List<Edge>();
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height - 1; j++)
                {
                    var e = new Edge(
                        new Node($"{nodePrefix}{i}_{j}"),
                        new Node($"{nodePrefix}{i}_{j + 1}")
                    );
                    edgeList.Add(e);
                }
            }

            for (var i = 0; i < width - 1; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var e = new Edge(
                        new Node($"{nodePrefix}{i}_{j}"),
                        new Node($"{nodePrefix}{i + 1}_{j}")
                    );
                    edgeList.Add(e);
                }
            }

            var g = new Graph(edgeList);

            // in case no edges, we still have a solitary node.
            g.AddNode($"{nodePrefix}0_0");
            return g;
        }

        /// <summary>
        /// Generates a 3D grid (lattice) of nodes where each node is adjacent to the nearest nodes in
        /// the cardinal directions (left,right, up,down, front,back). The dimensions (in terms of nodes)
        /// of the resulting graph will be width x depth x height, where height is calculated as
        /// the number of nodes in the nodeSet divided by (width x depth).
        /// This means the number of nodes in nodeSet must be a multiple of widthxdepth.
        /// </summary>
        /// <param name="nodeSet">Set of nodes.</param>
        /// <param name="width">width of the graph in terms of nodes.</param>
        /// <param name="depth">depth of the graph in terms of nodes.</param>
        /// <returns>3D grid.</returns>
        public static Graph Generate3DGrid(int width, int height, int depth)
        {
            if (width < 1 || height < 1 || depth < 1)
            {
                throw new NonPositiveArgumentException(
                    $"Non positive argument in width and height and depth: {width}, {height}, {depth}."
                );
            }

            var grid0 = GenerateGrid(width, height, "0_");
            var g = new Graph();
            g = g.MergeWith(grid0);
            for (int i = 1; i < depth; i++)
            {
                var grid = GenerateGrid(width, height, $"{i}_");
                g = g.MergeWith(grid);
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        var e = new Edge(new Node($"{i - 1}_{j}_{k}"), new Node($"{i}_{j}_{k}"));
                        g.AddEdge(e);
                    }
                }
            }

            return g;
        }

        /// <summary>
        /// Creates a <i>wheel graph</i> with the given number of spokes.
        /// Note that the number of nodes of the generated graph will be equal to then number
        /// of spokes +1.
        /// <see href="https://en.wikipedia.org/wiki/Wheel_graph">Wheel Graph definition.</see>.
        /// </summary>
        /// <param name="spokesCount">Number of spokes of the graph. Must be at least 3.</param>
        /// <returns>Wheel graph.</returns>
        public static Graph GenerateWheelGraph(int spokesCount)
        {
            if (spokesCount < 3)
            {
                throw new Exception(
                    string.Format(
                        "The number of spokes must be at least 3. Number provided {0}.",
                        spokesCount
                    )
                );
            }

            var c = 0;
            var edges = new List<Edge>();
            var center = new Node("center");
            while (c < spokesCount)
            {
                var edge = new Edge(
                    string.Format("{0}", c),
                    string.Format("{0}", (c + 1) % spokesCount)
                );
                edges.Add(edge);
                edges.Add(new Edge(center.GetLabel(), string.Format("{0}", c)));
                c++;
            }

            return new Graph(edges);
        }

        /// <summary>
        /// Creates the. <i>Petersen Graph</i>
        /// </summary>
        /// <returns>A copy of the Petersen Graph.</returns>
        public static Graph GeneratePetersenGraph(string nodePrefix = "")
        {
            var g = new Graph(
                $@"
            {nodePrefix}0->{nodePrefix}2,{nodePrefix}0->{nodePrefix}3,{nodePrefix}1->{nodePrefix}3,{nodePrefix}1->{nodePrefix}4,{nodePrefix}2->{nodePrefix}4,
            {nodePrefix}0->{nodePrefix}5,{nodePrefix}1->{nodePrefix}6,{nodePrefix}2->{nodePrefix}7,{nodePrefix}3->{nodePrefix}8,{nodePrefix}4->{nodePrefix}9,
            {nodePrefix}5->{nodePrefix}6,{nodePrefix}6->{nodePrefix}7,{nodePrefix}7->{nodePrefix}8,{nodePrefix}8->{nodePrefix}9,{nodePrefix}9->{nodePrefix}0"
            );

            return g;
        }

        /// <summary>
        /// Generates the <i>Turan</i> graph on n nodes, with r partitions. That is, the <i>multipartite</i> graph
        /// formed from n vertices with r parittions.
        /// If r does not evenly divide n then the  sizes of the subsets will be made as equal as possible.
        /// The nodes of the resulting graph shall be labelled
        /// <i>0,1,2,...,n</i>,
        /// and each partition will contain a sequential subset of these values.
        /// e.g. If n is 10, and r is 5, then there will be 5 partitions of the multipartite graph, each with 2 nodes.
        /// The partitions will be.
        /// <i>("0","1"), ("2","3"),...,("8","9")</i>
        ///
        /// </summary>
        /// <param name="n">The number of nodes / vertices in the resulting graph.</param>
        /// <param name="r">The number of subsets of the vertices.</param>
        /// <returns>A TuranGraph(n,r).</returns>
        public static Graph GenerateTuranGraph(uint n, uint r)
        {
            var g = CreateComplete(n);
            uint i = 0;
            while (i < n)
            {
                uint j = 0;
                var part = new HashSet<uint>();
                while (j < r)
                {
                    if (i + j > n)
                    {
                        return g;
                    }

                    part.Add(i + j);
                    j++;
                }

                foreach (var p in part)
                {
                    foreach (var q in part)
                    {
                        if (p != q)
                        {
                            g.RemoveEdge(string.Empty + p, string.Empty + q);
                        }
                    }
                }

                i += r;
            }

            return g;
        }

        /// <summary>
        /// Generates random weights on the edges of the graph. The random weights are sampled
        /// from a uniform (0,1) distribution. Weights will be scaled and shifted to fit int he range
        /// <i>(minWeight, maxWeight)</i>.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="minWeight">The minimum weight allowed.</param>
        /// <param name="maxWeight">The maximum weight allowed.</param>
        public static void GenerateRandomWeights(Graph graph, float minWeight, float maxWeight)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            // visit all the edges and wrap them with a weighted edge.
            foreach (var edge in graph.GetEdges())
            {
                var ew = graph.AddComponent<EdgeWeight>(edge);
                var weight = ((float)random.NextDouble() * (maxWeight - minWeight)) + minWeight;
                ew.Weight = weight;
            }
        }

        public static Graph GeneratePath(uint nodeCount, string nodePrefix = "Node")
        {
            var nodeNames = Enumerable
                .Range(0, ((int)nodeCount) - 1)
                .Select(i => $"{nodePrefix}{i}->{nodePrefix}{i + 1}");
            var constructorString = string.Join(",", nodeNames);
            return new Graph(constructorString);
        }

        public static Graph GenerateLadderGraph(uint pathNodeCount)
        {
            var path1 = GeneratePath(pathNodeCount, "A");
            var path2 = GeneratePath(pathNodeCount, "B");

            var nodes1 = path1.GetNodes().Select(n => n.GetLabel()).OrderBy(s => s).ToList();
            var nodes2 = path2.GetNodes().Select(n => n.GetLabel()).OrderBy(s => s).ToList();
            var zip = nodes1.Zip(nodes2, (a, b) => new { First = a, Second = b });
            var edges = zip.Select(z => z.First + "->" + z.Second).ToList();
            var edgeString = string.Join(",", edges);
            var g = path1.MergeWith(path2);
            g.AddEdges(edgeString);
            return g;
        }
    }
}
