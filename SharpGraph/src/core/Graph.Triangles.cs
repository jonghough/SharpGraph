using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {

        /// <summary>
        /// Finds triangles in the graph where the given node is in each traingle, 
        /// where a <i>triangle</i> is defined to be three pairwise 
        /// adjacent nodes.
        /// The returned list contains sublists where each sublist contians three nodes, together
        /// representing a triangle.
        /// </summary>
        /// <param name="node">node for which to find triangles</param>
        /// <returns>list of trinagles.</returns>
        public List<List<Node>> FindTriangles(Node node)
        {
            var result = new List<List<Node>>();
            var dic = new Dictionary<Edge, int>();
            var incident = this.GetIncidentEdges(node);
            for (int i = 0; i < incident.Count - 1; i++)
            {

                var e1 = incident[i];
                dic[e1] = i;
                for (int j = i + 1; j < incident.Count; j++)
                {

                    var e2 = incident[j];
                    if (e1.From() == e2.From())
                    {
                        var ep1 = new Edge(e1.To(), e2.To());
                        var ep2 = new Edge(e2.To(), e1.To());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new List<Node>();
                            l.Add(e1.From());
                            l.Add(e1.To());
                            l.Add(e2.To());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                    else if (e1.From() == e2.To())
                    {
                        var ep1 = new Edge(e1.To(), e2.From());
                        var ep2 = new Edge(e2.From(), e1.To());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new List<Node>();
                            l.Add(e1.From());
                            l.Add(e1.To());
                            l.Add(e2.From());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                    else if (e1.To() == e2.To())
                    {
                        var ep1 = new Edge(e1.From(), e2.From());
                        var ep2 = new Edge(e2.From(), e1.From());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new List<Node>();
                            l.Add(e1.To());
                            l.Add(e1.From());
                            l.Add(e2.From());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                    else if (e1.To() == e2.From())
                    {
                        var ep1 = new Edge(e1.From(), e2.To());
                        var ep2 = new Edge(e2.To(), e1.From());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new List<Node>();
                            l.Add(e1.To());
                            l.Add(e1.From());
                            l.Add(e2.To());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Finds all triangles in the graph, where <i>triangle</i> is three pairwise adjacent nodes.
        /// </summary>
        /// <returns>List of Node triples.</returns>
        public List<Tuple<Node, Node, Node>> FindTriangles()
        {
            var result = new List<Tuple<Node, Node, Node>>();
            var dic = new Dictionary<Edge, int>();
            for (int i = 0; i < _edges.Count - 2; i++)
            {

                var e1 = _edges[i];
                dic[e1] = i;
                for (int j = i + 1; j < _edges.Count - 1; j++)
                {

                    var e2 = _edges[j];
                    if (e1.From() == e2.From())
                    {
                        var ep1 = new Edge(e1.To(), e2.To());
                        var ep2 = new Edge(e2.To(), e1.To());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new Tuple<Node, Node, Node>(e1.From(), e1.To(), e2.To());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                    else if (e1.From() == e2.To())
                    {
                        var ep1 = new Edge(e1.To(), e2.From());
                        var ep2 = new Edge(e2.From(), e1.To());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new Tuple<Node, Node, Node>(e1.From(), e1.To(), e2.From());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                    else if (e1.To() == e2.To())
                    {
                        var ep1 = new Edge(e1.From(), e2.From());
                        var ep2 = new Edge(e2.From(), e1.From());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new Tuple<Node, Node, Node>(e1.To(), e1.From(), e2.From());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                    else if (e1.To() == e2.From())
                    {
                        var ep1 = new Edge(e1.From(), e2.To());
                        var ep2 = new Edge(e2.To(), e1.From());
                        if ((_edges.Contains(ep1) && !dic.ContainsKey(ep1)) ||
                            (_edges.Contains(ep2) && !dic.ContainsKey(ep2)))
                        {
                            var l = new Tuple<Node, Node, Node>(e1.To(), e1.From(), e2.To());
                            dic[e2] = j;
                            result.Add(l);
                        }
                    }
                }
            }
            return result;
        }

        private int ChoosePairs(int n)
        {
            return n * (n - 1) / 2;
        }
        public int GetTriadCount()
        {
            var sum = this._nodes.ToList().Select(v => ChoosePairs(Degree(v)))
                .Sum();
            return sum;
        }

        public int Transitivity()
        {
            return 3 * FindTriangles().Count / GetTriadCount();
        }

    }
}