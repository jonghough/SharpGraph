// <copyright file="Graph.Connectivity.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {
        /// <summary>
        /// Checks if the graph is connected.
        /// </summary>
        /// <returns>Returns <i>true</i> if the graph is connected, <i>false</i> otherwise.</returns>
        public bool IsConnected()
        {
            var counter = 0;
            this.BFS(
                (g, p, c) =>
                {
                    counter++;
                }
            );

            return this.nodes.Count == counter;
        }

        /// <summary>
        /// Finds all maximally connected subgraphs in the graph.
        /// </summary>
        /// <returns>List of graphs, where eahc graph is a maximally connected subgraph.</returns>
        public List<Graph> FindMaximallyConnectedSubgraphs()
        {
            var graphList = new List<Graph>();
            var connectedNodes = this.GetConnectedComponents();
            _ = this.GetEdges();
            foreach (var nodeList in connectedNodes)
            {
                var edgeSet = new HashSet<Edge>();
                foreach (var node in nodeList)
                {
                    edgeSet.UnionWith(this.GetIncidentEdges(node));
                }

                graphList.Add(new Graph(new List<Edge>(edgeSet), new HashSet<Node>(nodeList)));
            }

            return graphList;
        }

        /// <summary>
        /// Finds all the <i>Connected Components</i> of the graph.
        /// </summary>
        /// <returns></returns>
        public List<List<Node>> GetConnectedComponents()
        {
            var connectedComponents = new List<List<Node>>();
            var nodeDict = new Dictionary<Node, NodeSearchMemory>();
            foreach (var nd in this.nodes)
            {
                nodeDict[nd] = new NodeSearchMemory();
            }

            var nn = this.GetFirstUnvisitedNode(nodeDict);
            if (nn == null)
            {
                return connectedComponents; // nothing to do
            }

            var n = nn.GetValueOrDefault();
            while (true)
            {
                var conn = this.GetConnected(n, nodeDict);
                conn.Add(n);

                connectedComponents.Add(conn);
                nn = this.GetFirstUnvisitedNode(nodeDict);
                if (nn == null)
                {
                    break;
                }
                else
                {
                    n = nn.GetValueOrDefault();
                }
            }

            return connectedComponents;
        }

        /// <summary>
        /// Finds all the bridge edges of the graph. Bridge edges are the edges whose removal
        /// will increase the number of connected components.
        /// </summary>
        /// <returns>List of bridge edges.</returns>
        public List<Edge> FindBridges()
        {
            var visited = new HashSet<Node>();
            var disc = new Dictionary<Node, float>();
            var low = new Dictionary<Node, float>();
            var parents = new Dictionary<Node, Node>();
            var bridges = new HashSet<Edge>();
            foreach (var node in this.nodes)
            {
                disc[node] = float.PositiveInfinity;
                low[node] = float.PositiveInfinity;
            }

            var counter = new int[] { 1 };
            foreach (var node in this.nodes)
            {
                if (!visited.Contains(node))
                {
                    visited.Add(node);
                    this.Bridge(node, parents, counter, visited, disc, low, bridges);
                }
            }

            return new List<Edge>(bridges);
        }

        private Node? GetFirstUnvisitedNode(Dictionary<Node, NodeSearchMemory> nodeDict)
        {
            var nodeL = new List<Node>(this.nodes);
            foreach (var n in nodeL)
            {
                if (!nodeDict[n].Visited)
                {
                    return n;
                }
            }

            return null;
        }

        private List<Node> GetConnected(Node node, Dictionary<Node, NodeSearchMemory> nodeDict)
        {
            nodeDict[node].Visited = true;
            var connList = new List<Node>();
            var adjNodes = this.GetAdjacentUnvisited(node, nodeDict);
            connList.AddRange(adjNodes);
            foreach (var t in adjNodes)
            {
                nodeDict[t].Visited = true;
            }

            foreach (var t in adjNodes)
            {
                connList.AddRange(this.GetConnected(t, nodeDict));
            }

            return connList;
        }

        private HashSet<Node> Assign(Node n, HashSet<Node> visited)
        {
            visited.Add(n);
            var adjTrans = this.GetAdjacentTransposed(n);
            var hs = new HashSet<Node>();
            hs.Add(n);
            if (adjTrans.Count == 0)
            {
                return hs;
            }

            foreach (var adj in adjTrans)
            {
                if (!visited.Contains(adj))
                {
                    visited.Add(adj);
                    hs.UnionWith(this.Assign(adj, visited));
                }
            }

            return hs;
        }

        private void Bridge(
            Node current,
            Dictionary<Node, Node> parents,
            int[] counter,
            HashSet<Node> visited,
            Dictionary<Node, float> disc,
            Dictionary<Node, float> low,
            HashSet<Edge> bridges
        )
        {
            visited.Add(current);
            disc[current] = counter[0];
            low[current] = counter[0];
            counter[0] += 1;

            foreach (var adj in this.GetAdjacent(current))
            {
                if (!visited.Contains(adj))
                {
                    parents[adj] = current;
                    this.Bridge(adj, parents, counter, visited, disc, low, bridges);

                    low[current] = Math.Min(low[adj], low[current]);
                    if (low[adj] > disc[current])
                    {
                        var hs = new HashSet<Edge>(this.incidenceMap[current]);
                        hs.IntersectWith(this.incidenceMap[adj]);
                        var edge = hs.ToList()[0];
                        bridges.Add(edge);
                    }
                }
                else if (parents.ContainsKey(current) && parents[current] != adj)
                {
                    low[current] = Math.Min(disc[adj], low[current]);
                }
            }
        }
    }

    public class NotConnectedException : Exception
    {
        public NotConnectedException(string msg)
            : base(msg) { }
    }
}
