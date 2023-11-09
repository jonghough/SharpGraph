// <copyright file="Graph.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph
{
    public abstract class GraphComponent
    {
        private Graph owner;

        protected GraphComponent() { }

        public Graph Owner
        {
            get => this.owner;
            internal set => this.owner = value;
        }

        public abstract void Copy(GraphComponent graphComponent);
    }

    public sealed partial class Graph : IGraph<Node, Edge>
    {
        private List<Edge> edges;
        private HashSet<Node> nodes;
        private Dictionary<Node, HashSet<Edge>> incidenceMap;
        private Dictionary<Node, Dictionary<string, NodeComponent>> nodeComponents;
        private Dictionary<Edge, Dictionary<string, EdgeComponent>> edgeComponents;
        private Dictionary<string, GraphComponent> graphComponents;

        public Graph()
        {
            this.Init();
        }

        public Graph(List<Edge> edges)
            : this()
        {
            foreach (var e in edges)
            {
                this.AddEdge(e);
            }
        }

        public Graph(List<Edge> edges, HashSet<Node> nodes)
            : this(edges)
        {
            foreach (var node in nodes)
            {
                this.AddNode(node);
            }
        }

        public Graph(params string[] nodes)
            : this()
        {
            foreach (var nodeLabel in nodes)
            {
                this.AddNode(nodeLabel);
            }
        }

        public Graph(params Node[] nodes)
            : this()
        {
            foreach (var node in nodes)
            {
                this.AddNode(node);
            }
        }

        public Graph(string fromToString)
            : this()
        {
            var nodeListMatcher = Regex.Match(fromToString, "^(\\d+)\\s*..\\s*(\\d+)$");
            var edgeArrowMatcher = Regex.Match(fromToString, "(\\w+)\\s*->\\s*(\\w+)");

            if (nodeListMatcher.Success)
            {
                while (nodeListMatcher.Success)
                {
                    var nodes = new List<string>();
                    for (var i = 1; i < nodeListMatcher.Groups.Count; i++)
                    {
                        nodes.Add(nodeListMatcher.Groups[i].ToString().Trim());
                    }

                    if (nodes.Count != 2)
                    {
                        throw new GraphConstructorException(
                            "Could not create edge in graph constructor, with argument "
                                + fromToString
                                + ", "
                                + nodes.Count
                        );
                    }

                    var a = int.Parse(nodes[0].Trim());
                    var b = int.Parse(nodes[1].Trim());
                    if (b <= a)
                    {
                        throw new GraphConstructorException(
                            "Cannot instantiate group from argument "
                                + fromToString
                                + ", "
                                + b
                                + "is not greater than "
                                + a
                        );
                    }

                    for (var i = a; i <= b; i++)
                    {
                        this.AddNode(string.Empty + i);
                    }

                    nodeListMatcher = nodeListMatcher.NextMatch();
                }
            }
            else if (edgeArrowMatcher.Success)
            {
                while (edgeArrowMatcher.Success)
                {
                    var nodes = new List<string>();
                    for (var i = 1; i < edgeArrowMatcher.Groups.Count; i++)
                    {
                        nodes.Add(edgeArrowMatcher.Groups[i].ToString().Trim());
                    }

                    if (nodes.Count != 2)
                    {
                        throw new GraphConstructorException(
                            "Could not create edge in graph constructor, with argument "
                                + fromToString
                        );
                    }

                    this.AddEdge(nodes[0], nodes[1]);
                    edgeArrowMatcher = edgeArrowMatcher.NextMatch();
                }
            }
            else
            {
                throw new GraphConstructorException(
                    "Unrecognized regular expression in graph constructor, with argument "
                        + fromToString
                );
            }
        }

        public Graph(Graph g, HashSet<Node> nodes)
        {
            this.Init();
            var filteredEdges = g.edges
                .Where(e => nodes.Contains(e.From()) && nodes.Contains(e.To()))
                .ToList();
            foreach (var e in filteredEdges)
            {
                this.AddEdge(e);
            }
        }

        public bool AddNode(string node)
        {
            return this.AddNode(new Node(node));
        }

        public bool AddNode(Node node)
        {
            if (this.nodeComponents.ContainsKey(node))
            {
                return false;
            }

            if (this.nodes.Contains(node))
            {
                return false;
            }

            var comps = new Dictionary<string, NodeComponent>();
            this.nodeComponents[node] = comps;
            this.nodes.Add(node);
            return true;
        }

        public ReadOnlyCollection<Edge> GetEdgesAsReadOnly()
        {
            return new ReadOnlyCollection<Edge>(this.edges);
        }

        public List<Edge> GetEdges()
        {
            return new List<Edge>(this.edges);
        }

        public HashSet<Node> GetNodes()
        {
            var hs = new HashSet<Node>();
            foreach (var n in this.nodes)
            {
                hs.Add(n);
            }

            return hs;
        }

        public Node? GetNode(string label)
        {
            return this.nodes.Where(n => n.GetLabel() == label).FirstOrDefault();
        }

        public int Degree(Node node)
        {
            if (this.nodes.Contains(node) == false)
            {
                return -1;
            }
            else
            {
                var deg = 0;
                foreach (var e in this.edges)
                {
                    if (e.Nodes().Contains(node))
                    {
                        deg++;
                    }
                }

                return deg;
            }
        }

        public void AddEdge(string node1, string node2)
        {
            var n1 = new Node(node1);
            var n2 = new Node(node2);
            this.AddEdge(new Edge(n1, n2));
        }

        public void AddEdge(Node node1, Node node2)
        {
            this.AddEdge(new Edge(node1, node2));
        }

        /// <summary>
        /// Adds an edge to the graph. The string argument is expected to be of the form:
        ///  <br/>
        /// <i> N1 -> N2 </i>,
        /// <br/>
        /// where N1 is the first node's label (expected to be alphanumeric values only),
        /// and N2 is the second node's label (similarly, alphanumeric only).
        /// Whitespace is ignored.
        /// Because N1 and N2 are expected to be only alphanumeric, this method is not suitable for nodes with
        /// labels using characters outside this range (e.g. underscores, or special characters).
        /// </summary>
        /// <param name="edgeString">input string.</param>
        public void AddEdge(string edgeString)
        {
            var edgeArrowMatcher = Regex.Match(edgeString, "^\\s*(\\w+)\\s*->\\s*(\\w+)\\s*$");
            if (edgeArrowMatcher.Success)
            {
                while (edgeArrowMatcher.Success)
                {
                    var nodes = new List<string>();
                    for (var i = 1; i < edgeArrowMatcher.Groups.Count; i++)
                    {
                        nodes.Add(edgeArrowMatcher.Groups[i].ToString().Trim());
                    }

                    if (nodes.Count != 2)
                    {
                        throw new Exception("Could not create edge with argument " + edgeString);
                    }

                    this.AddEdge(nodes[0], nodes[1]);
                    edgeArrowMatcher = edgeArrowMatcher.NextMatch();
                }
            }
        }

        public Edge? GetEdge(HashSet<Node> nodes)
        {
            if (nodes.Count != 2)
            {
                throw new Exception(
                    "GetEdge can only take a set of two nodes. i.e. the nodes that are incident with the edge."
                );
            }

            var nodeList = nodes.ToList();
            var maybeEdge = this.GetEdge(nodeList[0], nodeList[1]);
            if (maybeEdge == null)
            {
                return this.GetEdge(nodeList[1], nodeList[0]);
            }

            return maybeEdge;
        }

        public Edge? GetEdge(Node node1, Node node2)
        {
            var edge = new Edge(node1, node2);
            foreach (var e in this.edges)
            {
                if (edge == e)
                {
                    return e;
                }
            }

            return null;
        }

        public Edge? GetEdge(string node1, string node2)
        {
            return this.GetEdge(new Node(node1), new Node(node2));
        }

        public bool RemoveNode(Node node)
        {
            if (!this.nodes.Contains(node))
            {
                return false;
            }

            var incidentEdges = this.incidenceMap[node];
            if (incidentEdges != null)
            {
                this.edges.RemoveAll(e => incidentEdges.Contains(e));
                incidentEdges.RemoveWhere(e => incidentEdges.Contains(e));
            }

            this.nodes.Remove(node);
            return true;
        }

        public bool RemoveEdge(HashSet<Node> nodes)
        {
            if (nodes.Count != 2)
            {
                throw new Exception(
                    "RemoveEdge can only take a set of two nodes. i.e. the nodes that are incident with the edge."
                );
            }

            var nodeList = nodes.ToList();
            if (!this.RemoveEdge((nodeList[0], nodeList[1])))
            {
                return this.RemoveEdge((nodeList[1], nodeList[0]));
            }

            return true;
        }

        public bool RemoveEdge(string fromNode, string toNode)
        {
            return this.RemoveEdge((new Node(fromNode), new Node(toNode)));
        }

        public bool RemoveEdge((Node, Node) fromToNodes)
        {
            var edge = new Edge(fromToNodes.Item1, fromToNodes.Item2);
            return this.RemoveEdge(edge);
        }

        public bool RemoveEdge(Edge edge)
        {
            if (!this.edges.Contains(edge))
            {
                return false;
            }

            var f = edge.From();
            var t = edge.To();
            var fromIncidentEdges = this.incidenceMap[f];
            if (fromIncidentEdges != null)
            {
                fromIncidentEdges.RemoveWhere(e => e.Equals(edge));
            }

            var toIncidentEdges = this.incidenceMap[t];
            if (toIncidentEdges != null)
            {
                toIncidentEdges.RemoveWhere(e => e.Equals(edge));
            }

            this.edgeComponents.Remove(edge);
            this.edges.Remove(edge);

            return true;
        }

        public Graph MergeWith(Graph graph2)
        {
            var edges1 = this.GetEdges();
            var edges2 = graph2.GetEdges();
            var nodes = this.GetNodes();
            nodes.UnionWith(graph2.GetNodes());
            var copy = new List<Edge>(edges1);
            copy.AddRange(edges2);
            return new Graph(copy, nodes);
        }

        internal bool AddEdge(Edge edge)
        {
            if (this.GetEdges().Contains(edge))
            {
                return false;
            }

            if (this.edgeComponents.ContainsKey(edge))
            {
                return false;
            }

            var comps = new Dictionary<string, EdgeComponent>();
            this.edgeComponents[edge] = comps;

            this.edges.Add(edge);
            var from = edge.From();
            var to = edge.To();
            this.AddNode(to);
            this.AddNode(from);
            this.AddIncidence(from, edge);
            this.AddIncidence(to, edge);

            return true;
        }

        private void Init()
        {
            this.incidenceMap = new Dictionary<Node, HashSet<Edge>>();
            this.edges = new List<Edge>();
            this.nodes = new HashSet<Node>();
            this.nodeComponents = new Dictionary<Node, Dictionary<string, NodeComponent>>();
            this.edgeComponents = new Dictionary<Edge, Dictionary<string, EdgeComponent>>();
            this.graphComponents = new Dictionary<string, GraphComponent>();
        }

        private void AddIncidence(Node node, Edge edge)
        {
            HashSet<Edge> edges;
            if (this.incidenceMap.TryGetValue(node, out edges))
            {
                edges.Add(edge);
                this.incidenceMap[node] = edges;
            }
            else
            {
                var el = new HashSet<Edge>();
                el.Add(edge);
                this.incidenceMap.Add(node, el);
            }
        }

        private void RemoveIncidence(Node node, Edge edge)
        {
            HashSet<Edge> edges;
            if (this.incidenceMap.TryGetValue(node, out edges))
            {
                edges.Remove(edge);
                this.incidenceMap[node] = edges;
            }
        }

        private bool IsIncident(Node node, Edge edge)
        {
            HashSet<Edge> edges;
            if (this.incidenceMap.TryGetValue(node, out edges))
            {
                return edges.Contains(edge);
            }
            else
            {
                return false;
            }
        }
    }

    public class GraphConstructorException : Exception
    {
        public GraphConstructorException(string message)
            : base(message) { }
    }
}
