using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {
        public List<Edge> GetIncidentEdges(Node node)
        {
            HashSet<Edge> edgeSet;
            if (_incidenceMap.TryGetValue(node, out edgeSet))
            {
                return new List<Edge>(edgeSet);
            }
            else
            {
                return new List<Edge>();
            }
        }

        /// <summary>
        /// Gets all incident edges with the given node, excluding the edges defined by the
        /// forbidden predicate.
        /// </summary>
        /// <param name="node">Node for which to find the incident edges</param>
        /// <param name="forbidden">Predicate defining the exclusion edges</param>
        /// <returns></returns>
        public List<Edge> GetIncidentEdges(Node node, Predicate<Edge> forbidden)
        {
            HashSet<Edge> edgeSet;
            if (_incidenceMap.TryGetValue(node, out edgeSet))
            {
                var edges = new List<Edge>(edgeSet);
                edges.RemoveAll(forbidden);
                return edges;
            }
            else return new List<Edge>();
        }

        /// <summary>
        /// Gets all adjacent nodes to the given node, on the graph. If the <i>isDirected</i>
        /// parameter is <b>true</b> then the graph will be assumed to be  directed and
        /// only the nodes that are outgoing from <i>node</i> will be included in the 
        /// returned list of adjacent nodes.
        /// </summary>
        /// <param name="node">Node for which to find adjacent nodes</param>
        /// <param name="isDirected">If true, grpah is assume dot be directed and edges are asusmed to contain <i>EdgeDirection</i> components.</param>
        /// <returns></returns>
        public List<Node> GetAdjacent(Node node, bool isDirected = false)
        {
            HashSet<Node> adjacent = new HashSet<Node>();
            HashSet<Edge> edgeSet;
            if (_incidenceMap.TryGetValue(node, out edgeSet))
            {
                foreach (Edge e in edgeSet)
                {
                    EdgeDirection ed = GetComponent<EdgeDirection>(e);
                    if (ed == null)
                    {
                        adjacent.UnionWith(e.Nodes());
                    }
                    else
                    {
                        if (e.From() == node)
                        {
                            if (ed.Direction != Direction.Backwards)
                            {
                                adjacent.Add(e.To());
                            }
                        }
                        else
                        {
                            if (ed.Direction != Direction.Forwards)
                            {
                                adjacent.Add(e.From());
                            }
                        }
                    }

                }
                adjacent.Remove(node);
            }
            return new List<Node>(adjacent);
        }

        public List<Node> GetAdjacentTransposed(Node node)
        {
            HashSet<Node> adjacent = new HashSet<Node>();
            HashSet<Edge> edgeSet;
            if (_incidenceMap.TryGetValue(node, out edgeSet))
            {
                foreach (Edge e in edgeSet)
                {
                    EdgeDirection ed = GetComponent<EdgeDirection>(e);
                    if (ed == null)
                    {

                        adjacent.UnionWith(e.Nodes());
                    }
                    else
                    {
                        if (e.From() == node)
                        {
                            if (ed.Direction != Direction.Forwards)
                            {
                                adjacent.Add(e.To());
                            }
                        }
                        else
                        {
                            if (ed.Direction != Direction.Backwards)
                            {
                                adjacent.Add(e.From());
                            }
                        }
                    }

                }
                adjacent.Remove(node);
            }
            return new List<Node>(adjacent);
        }

        /// <summary>
        /// Returns true if the two nodes are adjacent on the graph.
        /// </summary>
        /// <param name="t1">first node</param>
        /// <param name="t2">second node</param>
        /// <returns></returns>
        public bool IsAdjacent(Node t1, Node t2)
        {
            foreach (Edge e in GetEdges())
            {
                if (e.Nodes().Contains(t1) && e.Nodes().Contains(t2))
                {
                    return true;
                }
            }
            return false;
        }

        internal List<Node> GetAdjacentUnvisited(Node node, Dictionary<Node, NodeSearchMemory> memory, bool isDirected = false)
        {
            var adjNodes = GetAdjacent(node, isDirected);
            var unvisited = adjNodes.Where(
                other => memory[other].Visited == false).ToList();

            unvisited.Remove(node);
            return new List<Node>(unvisited);
        }

        /// <summary>
        /// Gets all incident edges of the given edge in the graph. If the <i>isDirected</i> flag is 
        /// true then the graph will be assumed ot be a directed graph and if edges have <i>EdgeDirection</i>
        /// components, only outgoing edges will be included in the result list of incident edges.
        /// If the <i>isDirected</i> flag is false, then all incident edges will be returned.
        /// </summary>
        /// <param name="edge">Edge for which to find incident edges</param>
        /// <param name="isDirected">flag defining if the graph should be considered a directed graph.</param>
        /// <returns></returns>
        public List<Edge> GetIncidentEdges(Edge edge, bool isDirected = false)
        {
            HashSet<Edge> filtered = new HashSet<Edge>();
            foreach (Edge e in this._edges)
            {
                var dir = GetComponent<EdgeDirection>(e);
                if (e.Nodes().Contains(edge.From()))
                {
                    if ((isDirected && dir != null && dir.Direction != Direction.Backwards) || dir == null)
                        filtered.Add(e);
                }
                else if (e.Nodes().Contains(edge.To()))
                {
                    if ((isDirected && dir != null && dir.Direction != Direction.Forwards) || dir == null)
                        filtered.Add(e);
                }
            }
            filtered.Remove(edge);
            return new List<Edge>(filtered);
        }

        internal List<Edge> GetIncidentEdgesUnvisited(Edge edge, Dictionary<Edge, EdgeSearchMemory> memory, bool isDirected = false)
        {
            List<Edge> filtered = new List<Edge>();
            foreach (Edge e in this._edges)
            {
                if (memory[e].Visited)
                    continue;
                var dir = GetComponent<EdgeDirection>(e);
                if (e.Nodes().Contains(edge.From()))
                {
                    if ((isDirected && dir != null && dir.Direction != Direction.Backwards) || dir == null)
                        filtered.Add(e);
                }
                else if (e.Nodes().Contains(edge.To()))
                {
                    if ((isDirected && dir != null && dir.Direction != Direction.Forwards) || dir == null)
                        filtered.Add(e);
                }
            }

            return filtered;
        }

        internal Dictionary<Node, float> GetAdjacentNodesWithWeights(Node node, bool isDirected = false)
        {
            Dictionary<Node, float> adj = new Dictionary<Node, float>();
            foreach (Edge e in GetEdges())
            {
                var dir = GetComponent<EdgeDirection>(e);

                if (e.From().Equals(node))
                {
                    if ((isDirected && dir != null && dir.Direction != Direction.Backwards) || dir == null)
                    {
                        adj.Add(e.To(), GetComponent<EdgeWeight>(e).Weight);
                    }
                }
                else if (e.To().Equals(node))
                {
                    if ((isDirected && dir != null && dir.Direction != Direction.Forwards) || dir == null)
                    {
                        adj.Add(e.From(), GetComponent<EdgeWeight>(e).Weight);
                    }
                }
            }
            return adj;
        }
    }
}