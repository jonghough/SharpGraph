﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SharpGraph
{

    public static class GraphGenerator
    {

        /// <summary>
        /// Creates a <i>complete graph</i> for the given input size. The node names are
        /// the stringified numbers less than or equal to the given size.
        /// </summary>
        /// <param name="size">number of nodes in the complete graph</param>
        /// <returns>A complete graph</returns>
        public static Graph CreateComplete(uint size)
        {

            var hs = new HashSet<Node>();
            for (int i = 0; i < size; i++)
            {
                hs.Add(new Node("" + i));
            }
            return CreateComplete(hs);
        }

        /// <summary>
        /// Creates a <i>complete graph</i> for the given input arguments. The input argument 
        /// should be the names of the nodes.
        /// </summary>
        /// <param name="args">the names of the nodes in the generated graph</param>
        /// <returns>A complete graph</returns>
        public static Graph CreateComplete(params string[] args)
        {
            var hs = new HashSet<Node>();
            for (int i = 0; i < args.Length; i++)
            {
                hs.Add(new Node(args[i]));
            }
            return CreateComplete(hs);
        }

        /// <summary>
        /// Creates a complete graph generated by the given set of nodes. Each of the nodes
        /// in the input argument will become a node of the graph.
        /// </summary>
        /// <param name="nodeSet">set of nodes</param>
        /// <returns>A complete graph</returns>
        public static Graph CreateComplete(HashSet<Node> nodeSet)
        {
            List<Node> nodeList = new List<Node>(nodeSet);
            List<Edge> edgeList = new List<Edge>();
            for (int i = 0; i < nodeList.Count - 1; i++)
            {
                for (int j = i + 1; j < nodeList.Count; j++)
                {
                    Edge e = new Edge(nodeList[i], nodeList[j]);
                    edgeList.Add(e);
                }
            }
            Graph graph = new Graph(edgeList);
            return graph;
        }

        public static Graph CreateComplete(int size, string nodePrefix)
        {

            var nodes = new HashSet<Node>();
            for (int i = 0; i < size; i++)
            {
                var n = new Node(nodePrefix + "" + i);
                nodes.Add(n);
            }
            var nodeList = new List<Node>(nodes);
            List<Edge> edgeList = new List<Edge>();
            for (int i = 0; i < nodeList.Count - 1; i++)
            {
                for (int j = i + 1; j < nodeList.Count; j++)
                {
                    Edge e = new Edge(nodeList[i], nodeList[j]);
                    edgeList.Add(e);
                }
            }
            Graph graph = new Graph(edgeList);
            return graph;
        }

        public static Graph GenerateRandomGraph(HashSet<Node> nodes, float edgeProb)
        {
            float ep = edgeProb;
            ep = ep < 0 ? 0 : ep > 1.0f ? 1.0f : ep;
            Graph compG = CreateComplete(nodes);
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            for (int i = compG.GetEdges().Count - 1; i >= 0; i--)
            {

                float r = (float)rand.NextDouble();
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
            for (int i = 0; i < leftCount; i++)
            {
                leftNodes.Add(new Node("" + i));
            }
            for (int i = 0; i < rightCount; i++)
            {
                rightNodes.Add(new Node("" + (i + leftCount)));
            }
            return GenerateBipartiteComplete(leftNodes, rightNodes);
        }

        /// <summary>
        /// Generates the complete bipartite graph with left sdie having <code>leftNodes</code> nodes and
        /// the right side having <code>rightNodes</code> nodes. Each node on left side is adjacent to each and only the 
        /// nodes on the right side, and vice versa.
        /// </summary>
        /// <param name="leftNodes">nodes for the left side</param>
        /// <param name="rightNodes">nodes for the right side</param>
        /// <returns>Complete bipartite graph</returns>
        public static Graph GenerateBipartiteComplete(HashSet<Node> leftNodes, HashSet<Node> rightNodes)
        {
            int ls = leftNodes.Count;
            int rs = rightNodes.Count;
            if (ls < 1 || rs < 1)
            {
                throw new Exception("Cannot Create Graph. Number of nodes in left and right partitions must be at least 1.");
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
        /// Creates a cyclic graph on n vertices, where n is the given arguemtn positive integer.
        /// 
        /// </summary>
        /// <param name="size">number of nodes in the generated cyclic graph</param>
        /// <returns>A cyclic graph</returns>
        public static Graph GenerateCycle(uint size)
        {

            var hs = new HashSet<Node>();
            for (int i = 0; i < size; i++)
            {
                hs.Add(new Node("" + i));
            }
            return GenerateCycle(hs);

        }

        /// <summary>
        /// Creates a cyclic graph produced from the given set of nodes, where
        /// each node becomes a node of the created graph.
        /// </summary>
        /// <param name="nodeSet">nodes of the cyclic graph</param>
        /// <returns>A cyclic graph</returns>
        public static Graph GenerateCycle(HashSet<Node> nodeSet)
        {
            int len = nodeSet.Count;
            List<Node> nodeList = new List<Node>(nodeSet);
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < len - 1; i++)
            {
                Edge e = new Edge(nodeList[i], nodeList[i + 1]);
                edges.Add(e);
            }
            edges.Add(new Edge(nodeList[len - 1], nodeList[0]));
            return new Graph(edges);
        }

        /// <summary>
        /// Creates a barbell graph, which is defined as two complete graphs connected by a single edge.
        /// The <i>leftSet</i> and <i>rightSet>\i? arguments give the nodes of the two generated complete graphs.
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

            Node left = gLeft.GetEdges()[0].From();
            Node right = gRight.GetEdges()[0].From();

            //the bar edge.
            Edge e = new Edge(left, right);

            List<Edge> allEdges = gLeft.GetEdges();
            allEdges.AddRange(gRight.GetEdges());
            allEdges.Add(e);

            return new Graph(allEdges);
        }
        /// <summary>
        /// Creates a barbell graph, which is defined as two complete graphs connected by a single edge.
        /// The <i>leftSize</i> and <i>rightSize>\i? arguments give the number of nodes of the two generated complete graphs.
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
                throw new Exception("To generate a barbell graph. Left node count and right node count must be positive.");
            }

            List<Node> nodes = new List<Node>(NodeGenerator.GenerateNodes(leftSize + rightSize));

            var left = new HashSet<Node>();
            var right = new HashSet<Node>();

            for (int i = 0; i < leftSize + rightSize; i++)
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

        public static Graph GenerateGrid(HashSet<Node> nodeSet, int width)
        {
            if (nodeSet.Count % width != 0)
                throw new Exception("Number of nodes must be a multiple of width.");
            int height = nodeSet.Count / width;
            List<Edge> edgeList = new List<Edge>();
            List<Node> nodeList = new List<Node>(nodeSet);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    Edge e = new Edge(nodeList[i + j * width], nodeList[i + (j + 1) * width]);
                    edgeList.Add(e);
                }

            }
            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Edge e = new Edge(nodeList[i + j * width], nodeList[i + j * width + 1]);
                    edgeList.Add(e);
                }
            }

            return new Graph(edgeList);
        }


        /// <summary>
        /// Generates a 3D grid (lattice) of nodes where each node is adjacent to the nearest nodes in
        /// the cardinal directions (left,right, up,down, front,back). The dimensions (in terms of nodes) 
        /// of the resulting graph will be width x depth x height, where height is calcualted as 
        /// the number of nodes in the nodeSet divided by (width x depth).
        /// This means the number of nodes in nodeSet must be a multiple of widthxdepth.
        /// </summary>
        /// <param name="nodeSet">Set of nodes</param>
        /// <param name="width">width of the graph in terms of nodes</param>
        /// <param name="depth">depth of the graph in terms of nodes</param>
        /// <returns>3D grid.</returns>
        public static Graph Generate3DGrid(HashSet<Node> nodeSet, int width, int depth)
        {
            if (nodeSet.Count % (width * depth) != 0)
                throw new Exception("Number of nodes must be a multiple of width * depth.");
            int height = nodeSet.Count / (width * depth);

            List<List<Edge>> edgeLists = new List<List<Edge>>();
            List<Node> masterList = new List<Node>(nodeSet);
            List<List<Node>> nodeLists = new List<List<Node>>();
            int ctr = 0;
            for (int i = 0; i < depth; i++)
            {
                HashSet<Node> set2 = new HashSet<Node>();
                List<Node> nl = new List<Node>();
                for (int j = 0; j < width * height; j++)
                {
                    set2.Add(masterList[width * height * i + j]);

                    nl.Add(masterList[width * height * i + j]);
                    ctr++;
                }
                var g = GenerateGrid(set2, width);
                edgeLists.Add(g.GetEdges());
                nodeLists.Add(nl);
            }

            List<Edge> edges = new List<Edge>();

            for (int i = 0; i < nodeLists.Count - 1; i++)
            {
                edges.AddRange(edgeLists[i]);
                for (int j = 0; j < nodeLists[i].Count; j++)
                {
                    edges.Add(new Edge(nodeLists[i][j], nodeLists[i + 1][j]));
                }
            }
            edges.AddRange(edgeLists[edgeLists.Count - 1]);
            return new Graph(edges);
        }

        /// <summary>
        /// Creates a <i>wheel graph</i> with the given number of spokes.
        /// 
        /// <see href="https://en.wikipedia.org/wiki/Wheel_graph">Wheel Graph definition.</see> 
        /// </summary>
        /// <param name="spokesCount">Number of spokes of the graph. Mus tbe at least 3.</param>
        /// <returns>Wheel graph.</returns>
        public static Graph GenerateWheelGraph(int spokesCount)
        {
            if (spokesCount < 3)
            {
                throw new Exception(String.Format("The number of spokes must be at least 3. Number provided {0}.", spokesCount));
            }
            int c = 0;
            var edges = new List<Edge>();
            var center = new Node("center");
            while (c++ < spokesCount)
            {
                var edge = new Edge(String.Format("{0}", c), String.Format("{0}", (c + 1) % spokesCount));
                edges.Add(edge);
                edges.Add(new Edge(center.GetLabel(), String.Format("{0}", c)));
            }
            return new Graph(edges);
        }


        /// <summary>
        /// Creates the <i>Peterson Graph</i>
        /// </summary>
        /// <returns>A copy of the Peterson Graph</returns>
        public static Graph GeneratePetersonGraph()
        {
            var nodes = new HashSet<Node>();
            for (int i = 0; i < 5; i++)
            {
                var n = new Node("" + i);
                nodes.Add(n);
            }
            var nodeList = new List<Node>(nodes);
            List<Edge> edgeList = new List<Edge>();
            for (int i = 0; i < nodeList.Count - 1; i++)
            {
                for (int j = i + 2; j < nodeList.Count; j++)
                {
                    Edge e = new Edge(nodeList[i], nodeList[j]);
                    edgeList.Add(e);
                }

            }
            for (int i = 0; i < nodeList.Count; i++)
            {
                Edge outer = new Edge(nodeList[i], new Node("" + (i + 5)));
                edgeList.Add(outer);
                edgeList.Add(new Edge(new Node("" + (i + 5)), new Node("" + (i + 6) % 10)));
            }
            Graph graph = new Graph(edgeList);
            return graph;

        }

        /// <summary>
        /// Generates random weights on the edges of the graph. The random weights are sampled
        /// from a uniform (0,1) distribution. Weights will be scaled and shifted to fit int he range
        /// <i>(minWeight, maxWeight)</i>.
        /// </summary>
        /// <param name="graph">The graph</param>
        /// <param name="minWeight">The minimum weight allowed</param>
        /// <param name="maxWeight">The maximum weight allowed</param>
        public static void GenerateRandomWeights(Graph graph, float minWeight,
            float maxWeight)
        {

            Random random = new Random(Guid.NewGuid().GetHashCode());

            //visit all the edges and wrap them with a weighted edge.
            foreach (Edge edge in graph.GetEdges())
            {
                var ew = graph.AddComponent<EdgeWeight>(edge);
                float weight = (float)random.NextDouble() * (maxWeight - minWeight) + minWeight;
                ew.Weight = weight;
            }

        }
    }

}