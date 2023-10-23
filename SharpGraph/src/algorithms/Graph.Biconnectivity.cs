// <copyright file="Graph.Biconnectivity.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace SharpGraph
{
    public partial class Graph
    {
        /// <summary>
        /// Finds biconnected components of the graph. The graph is assumed to be undirected.
        /// The returned biconnected components are represented as sets of edges. A graph, or subgraph of a graph,
        /// is biconnected if removing any node still leaves a connected graph, or subgraph, respectively.
        /// <example>
        /// We can create the biconnected subgraphs of a group, g, by first finding the biconnected components.
        /// <code>
        /// var biconnectedComps = g.FindBiconnectedComponents();
        /// var connectedSubgraphs = biconnectedComps.ToList().Select(c => new Graph(c.ToList())).toList();
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Biconnected components.</returns>
        public HashSet<HashSet<Edge>> FindBiconnectedComponents()
        {
            var nodeDataMap = new Dictionary<Node, BCNodeData>();
            foreach (var node in this.nodes)
            {
                var m = new BCNodeData();
                nodeDataMap[node] = m;
                m.Visited = false;
            }

            var edgeStack = new Stack<Edge>();
            var connectedSets = new HashSet<HashSet<Edge>>();
            var counter = new int[] { 0 };
            foreach (var node in this.nodes)
            {
                if (!nodeDataMap[node].Visited)
                {
                    this.VisitNode_BC(node, counter, edgeStack, connectedSets, nodeDataMap);
                }
            }

            return connectedSets;
        }

        private static HashSet<Edge> GatherBCNodes(Edge lastEdge, Stack<Edge> edgeStack)
        {
            var nl = new HashSet<Edge>();

            while (true)
            {
                var edge = edgeStack.Pop();
                nl.Add(edge);
                if (lastEdge == edge)
                {
                    break;
                }
            }

            return nl;
        }

        private void VisitNode_BC(
            Node node,
            int[] counter,
            Stack<Edge> edgeStack,
            HashSet<HashSet<Edge>> connectedSets,
            Dictionary<Node, BCNodeData> nodeDataMap
        )
        {
            var m = nodeDataMap[node];
            m.Visited = true;
            counter[0]++;
            m.Depth = counter[0];
            m.Low = counter[0];
            foreach (var nextEdge in this.GetIncidentEdges(node))
            {
                var nextNode = nextEdge.From() == node ? nextEdge.To() : nextEdge.From();
                if (!nodeDataMap[nextNode].Visited)
                {
                    edgeStack.Push(nextEdge);
                    nodeDataMap[nextNode].Parent = node;
                    this.VisitNode_BC(nextNode, counter, edgeStack, connectedSets, nodeDataMap);
                    if (nodeDataMap[nextNode].Low >= m.Depth)
                    {
                        connectedSets.Add(GatherBCNodes(nextEdge, edgeStack));
                    }

                    m.Low = Math.Min(nodeDataMap[nextNode].Low, m.Low);
                }
                else if (m.Parent != nextNode && m.Depth > nodeDataMap[nextNode].Depth)
                {
                    edgeStack.Push(nextEdge);
                    m.Low = Math.Min(nodeDataMap[nextNode].Depth, m.Low);
                }
            }
        }
    }

    internal class BCNodeData
    {
        public bool Visited = false;
        public Node? Parent = null;
        public int Depth = 0;
        public int Low = 0;

        public BCNodeData() { }
    }
}
