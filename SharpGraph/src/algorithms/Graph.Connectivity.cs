using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System;

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
            int counter = 0;
            BFS((g, p, c) =>
            {
                counter++;
            });

            return _nodes.Count == counter;
        }


        /// <summary>
        /// Finds all maximally connected subgraphs in the graph.
        /// </summary>
        /// <returns>List of graphs, where eahc graph is a maximally connected subgraph.</returns>
        public List<Graph> FindMaximallyConnectedSubgraphs()
        {
            List<Graph> graphList = new List<Graph>();
            List<List<Node>> connectedNodes = GetConnectedComponents();
            List<Edge> edges = GetEdges();
            foreach (List<Node> nodeList in connectedNodes)
            {
                HashSet<Edge> edgeSet = new HashSet<Edge>();
                foreach (Node node in nodeList)
                {
                    edgeSet.UnionWith(GetIncidentEdges(node));
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
            List<List<Node>> connectedComponents = new List<List<Node>>();
            var nodeDict = new Dictionary<Node, NodeSearchMemory>();
            foreach (var nd in _nodes)
            {
                nodeDict[nd] = new NodeSearchMemory();
            }
            Node? nn = GetFirstUnvisitedNode(nodeDict);
            if (nn == null)
            {
                return connectedComponents; // nothing to do
            }
            Node n = nn.GetValueOrDefault();
            while (true)
            {

                List<Node> conn = GetConnected(n, nodeDict);
                conn.Add(n);
                foreach (Node ed in conn)
                {

                }
                connectedComponents.Add(conn);
                nn = GetFirstUnvisitedNode(nodeDict);
                if (nn == null)
                    break;
                else
                {
                    n = nn.GetValueOrDefault();
                }

            }

            return connectedComponents;
        }


        private Node? GetFirstUnvisitedNode(Dictionary<Node, NodeSearchMemory> nodeDict)
        {
            List<Node> nodeL = new List<Node>(_nodes);
            foreach (Node n in nodeL)
            {
                if (!nodeDict[n].Visited)
                    return n;
            }
            return null;
        }



        private List<Node> GetConnected(Node node, Dictionary<Node, NodeSearchMemory> nodeDict)
        {
            nodeDict[node].Visited = true;
            List<Node> connList = new List<Node>();
            List<Node> adjNodes = GetAdjacentUnvisited(node,nodeDict);
            connList.AddRange(adjNodes);
            foreach (Node t in adjNodes)
            {
                nodeDict[t].Visited = true;
            }
            foreach (Node t in adjNodes)
            {
                connList.AddRange(GetConnected(t, nodeDict));
            }
            return connList;
        }

       

        private HashSet<Node> Assign(Node n, HashSet<Node> visited)
        {
            visited.Add(n);
            List<Node> adjTrans = GetAdjacentTransposed(n);
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
                    hs.UnionWith(Assign(adj, visited));
                }
            }
            return hs;

        }

        private void Bridge(Node current, Dictionary<Node, Node> parents,
            int[] counter, HashSet<Node> visited, Dictionary<Node, float> disc,
            Dictionary<Node, float> low, HashSet<Edge> bridges)
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
                    Bridge(adj, parents, counter, visited, disc, low, bridges);

                    low[current] = Math.Min(low[adj], low[current]);
                    if (low[adj] > disc[current])
                    {
                        var hs = new HashSet<Edge>(_incidenceMap[current]);
                        hs.IntersectWith(_incidenceMap[adj]);
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
            foreach (var node in this._nodes)
            { 
                disc[node] = float.PositiveInfinity;
                low[node] = float.PositiveInfinity;
            }
            int[] counter = new int[] { 1 };
            foreach (var node in this._nodes)
            {
                if (!visited.Contains(node))
                {
                    visited.Add(node);
                    Bridge(node, parents, counter, visited, disc, low, bridges);

                }
            }
            return new List<Edge>(bridges);
        }
    }

    public class NotConnectedException : Exception{

        public NotConnectedException(string msg):base(msg){ 
        }
    }

}