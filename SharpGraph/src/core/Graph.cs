﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph
{

    public abstract class GraphComponent
    {
        private Graph _owner;
        public Graph Owner { get { return _owner; } internal set { _owner = value; } }
        public GraphComponent()
        {

        }

        public abstract void Copy(GraphComponent graphComponent);
    }
    public sealed partial class Graph : IGraph<Node, Edge>
    {

        List<Edge> _edges;
        HashSet<Node> _nodes;
        private Dictionary<Node, HashSet<Edge>> _incidenceMap;
        private Dictionary<Node, Dictionary<string, NodeComponent>> _nodeComponents;
        private Dictionary<Edge, Dictionary<string, EdgeComponent>> _edgeComponents;
        private Dictionary<string, GraphComponent> _graphComponents;

        public Graph()
        {
            Init();
        }
        public Graph(List<Edge> edges) : this()
        {
            foreach (Edge e in edges)
            {
                AddEdge(e);
            }
        }


        public Graph(List<Edge> edges, HashSet<Node> nodes) : this(edges)
        {
            foreach (var node in nodes)
            {
                AddNode(node);
            }
        }

        public Graph(params string[] nodes) : this()
        {

            foreach (var nodeLabel in nodes)
            {
                this.AddNode(nodeLabel);
            }
        }

        public Graph(string fromToString) : this()
        {
            Match nodeListMatcher = Regex.Match(fromToString, "^(\\d+)\\s*..\\s*(\\d+)$");
            Match edgeArrowMatcher = Regex.Match(fromToString, "(\\w+)\\s*->\\s*(\\w+)");

            if (nodeListMatcher.Success)
            {
                while (nodeListMatcher.Success)
                {
                    var nodes = new List<String>();
                    for (int i = 1; i < nodeListMatcher.Groups.Count; i++)
                    {
                        nodes.Add(nodeListMatcher.Groups[i].ToString().Trim());
                    }
                    if (nodes.Count != 2)
                    {
                        throw new GraphConstructorException("Could not create edge in graph constructor, with argument " + fromToString + ", " + nodes.Count);
                    }
                    var a = int.Parse(nodes[0].Trim());
                    var b = int.Parse(nodes[1].Trim());
                    if (b <= a)
                    {
                        throw new GraphConstructorException("Cannot instantiate group from argument " + fromToString + ", " + b + "is not greater than " + a);
                    }
                    for (int i = a; i <= b; i++)
                    {
                        this.AddNode("" + i);
                    }
                    nodeListMatcher = nodeListMatcher.NextMatch();
                }

            }
            else if (edgeArrowMatcher.Success)
            {
                while (edgeArrowMatcher.Success)
                {
                    var nodes = new List<String>();
                    for (int i = 1; i < edgeArrowMatcher.Groups.Count; i++)
                    {
                        nodes.Add(edgeArrowMatcher.Groups[i].ToString().Trim());
                    }
                    if (nodes.Count != 2)
                    {
                        throw new GraphConstructorException("Could not create edge in graph constructor, with argument " + fromToString);
                    }
                    this.AddEdge(nodes[0], nodes[1]);
                    edgeArrowMatcher = edgeArrowMatcher.NextMatch();
                }
            }
            else
            {
                throw new GraphConstructorException("Unrecognized regular expression in graph constructor, with argument " + fromToString);
            }
        }


        public Graph(Graph g, HashSet<Node> nodes)
        {
            Init();
            var filteredEdges = g._edges.Where(e => nodes.Contains(e.From()) && nodes.Contains(e.To())).ToList();
            foreach (Edge e in filteredEdges)
            {

                AddEdge(e);
            }
        }

        private void Init()
        {
            _incidenceMap = new Dictionary<Node, HashSet<Edge>>();
            _edges = new List<Edge>();
            _nodes = new HashSet<Node>();
            _nodeComponents = new Dictionary<Node, Dictionary<string, NodeComponent>>();
            _edgeComponents = new Dictionary<Edge, Dictionary<string, EdgeComponent>>();
            _graphComponents = new Dictionary<string, GraphComponent>();
        }

        public bool AddNode(string node)
        {
            return AddNode(new Node(node));
        }
        public bool AddNode(Node node)
        {
            if (_nodeComponents.ContainsKey(node))
            {
                return false;
            }
            if (_nodes.Contains(node))
            {
                return false;
            }
            var comps = new Dictionary<string, NodeComponent>();
            _nodeComponents[node] = comps;
            _nodes.Add(node);
            return true;
        }

        private void AddIncidence(Node node, Edge edge)
        {
            HashSet<Edge> edges;
            if (_incidenceMap.TryGetValue(node, out edges))
            {
                edges.Add(edge);
                _incidenceMap[node] = edges;
            }
            else
            {
                var el = new HashSet<Edge>();
                el.Add(edge);
                _incidenceMap.Add(node, el);
            }
        }

        private void RemoveIncidence(Node node, Edge edge)
        {
            HashSet<Edge> edges;
            if (_incidenceMap.TryGetValue(node, out edges))
            {
                edges.Remove(edge);
                _incidenceMap[node] = edges;
            }
        }

        private bool IsIncident(Node node, Edge edge)
        {
            HashSet<Edge> edges;
            if (_incidenceMap.TryGetValue(node, out edges))
            {
                return edges.Contains(edge);
            }
            else
            {
                return false;
            }
        }

        public ReadOnlyCollection<Edge> GetEdgesAsReadOnly()
        {
            return new ReadOnlyCollection<Edge>(_edges);
        }
        public List<Edge> GetEdges()
        {
            return new List<Edge>(_edges);
        }

        public HashSet<Node> GetNodes()
        {
            HashSet<Node> hs = new HashSet<Node>();
            foreach (var n in _nodes)
            {
                hs.Add(n);
            }
            return hs;
        }

        public int Degree(Node node)
        {
            if (_nodes.Contains(node) == false)
            {
                return -1;
            }
            else
            {
                int deg = 0;
                foreach (Edge e in _edges)
                {
                    if (e.Nodes().Contains(node))
                        deg++;
                }
                return deg;
            }
        }

        public void AddEdge(string node1, string node2)
        {
            var n1 = new Node(node1);
            var n2 = new Node(node2);
            AddEdge(new Edge(n1, n2));
        }

        public void AddEdge(Node node1, Node node2)
        {
            AddEdge(new Edge(node1, node2));
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
        /// <param name="edgeString">input string</param>
        public void AddEdge(string edgeString)
        {
            Match edgeArrowMatcher = Regex.Match(edgeString, "^\\s*(\\w+)\\s*->\\s*(\\w+)\\s*$");
            if (edgeArrowMatcher.Success)
            {
                while (edgeArrowMatcher.Success)
                {
                    var nodes = new List<String>();
                    for (int i = 1; i < edgeArrowMatcher.Groups.Count; i++)
                    {
                        nodes.Add(edgeArrowMatcher.Groups[i].ToString().Trim());
                    }
                    if (nodes.Count != 2)
                    {
                        throw new Exception("Could not create edge in with argument " + edgeString);
                    }
                    this.AddEdge(nodes[0], nodes[1]);
                    edgeArrowMatcher = edgeArrowMatcher.NextMatch();
                }

            }
        }


        internal bool AddEdge(Edge edge)
        {
            if (GetEdges().Contains(edge))
                return false;

            if (_edgeComponents.ContainsKey(edge))
            {
                return false;
            }
            var comps = new Dictionary<string, EdgeComponent>();
            _edgeComponents[edge] = comps;

            _edges.Add(edge);
            var from = edge.From();
            var to = edge.To();
            AddNode(to);
            AddNode(from);
            AddIncidence(from, edge);
            AddIncidence(to, edge);

            return true;
        }

        public Edge? GetEdge(HashSet<Node> nodes)
        {
            if (nodes.Count != 2)
            {
                throw new Exception("GetEdge can only take a set of two nodes. i.e. the nodes that are incident with the edge.");

            }
            var nodeList = nodes.ToList();
            var maybeEdge = GetEdge(nodeList[0], nodeList[1]);
            if (maybeEdge == null)
            {
                return GetEdge(nodeList[1], nodeList[0]);
            }
            return maybeEdge;
        }
        public Edge? GetEdge(Node node1, Node node2)
        {

            var edge = new Edge(node1, node2);
            foreach (var e in _edges)
            {
                if (edge == e) return e;
            }
            return null;
        }
        public Edge? GetEdge(string node1, string node2)
        {
            return GetEdge(new Node(node1), new Node(node2));
        }
        public bool RemoveNode(Node node)
        {
            if (!_nodes.Contains(node))
                return false;
            var incidentEdges = _incidenceMap[node];
            if (incidentEdges != null)
            {
                _edges.RemoveAll(e => incidentEdges.Contains(e));
                incidentEdges.RemoveWhere(e => incidentEdges.Contains(e));

            }
            _nodes.Remove(node);
            return true;
        }

        public bool RemoveEdge(HashSet<Node> nodes)
        {
            if (nodes.Count != 2)
            {
                throw new Exception("RemoveEdge can only take a set of two nodes. i.e. the nodes that are incident with the edge.");

            }
            var nodeList = nodes.ToList();
            if (!RemoveEdge((nodeList[0], nodeList[1])))
            {
                return RemoveEdge((nodeList[1], nodeList[0]));
            }
            return true;
        }
        public bool RemoveEdge(string fromNode, string toNode)
        {
            return RemoveEdge((new Node(fromNode), new Node(toNode)));
        }
        public bool RemoveEdge((Node, Node) fromToNodes)
        {
            var edge = new Edge(fromToNodes.Item1, fromToNodes.Item2);
            return RemoveEdge(edge);
        }

        public bool RemoveEdge(Edge edge)
        {
            if (!_edges.Contains(edge))
                return false;

            var f = edge.From();
            var t = edge.To();
            var fromIncidentEdges = _incidenceMap[f];
            if (fromIncidentEdges != null)
            {
                fromIncidentEdges.RemoveWhere(e => e.Equals(edge));

            }

            var toIncidentEdges = _incidenceMap[t];
            if (toIncidentEdges != null)
            {
                toIncidentEdges.RemoveWhere(e => e.Equals(edge));

            }

            this._edgeComponents.Remove(edge);
            _edges.Remove(edge);

            return true;
        }
        public Graph MergeWith(Graph graph2)
        {
            List<Edge> edges1 = GetEdges();
            List<Edge> edges2 = graph2.GetEdges();
            var nodes = this.GetNodes();
            nodes.UnionWith(graph2.GetNodes());
            var copy = new List<Edge>(edges1);
            copy.AddRange(edges2);
            return new Graph(copy, nodes);
        }

    }

    public class GraphConstructorException : Exception
    {
        public GraphConstructorException(string message) : base(message)
        {
        }
    }
}


