using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{


    public enum SpanningTreeAlgorithm
    {
        Kruskal,
        Prim
    }
    internal class PrimNodeComparer : IComparer<Node>
    {
        internal Dictionary<Node, (float, Edge)> nodeMap;
        public PrimNodeComparer(Dictionary<Node, (float, Edge)> map)
        {
            nodeMap = map;
        }
        public int Compare(Node o1, Node o2)
        {
            float o1f = nodeMap[o1].Item1;
            float o2f = nodeMap[o2].Item1;

            return o1f.CompareTo(o2f);
        }
    }

    public class MinimumSpanningTreeException : Exception
    {
        public MinimumSpanningTreeException(string msg) : base(msg) { }
    }
    public partial class Graph
    {


        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the
        /// <code>EdgeWeight</code> component's <code>Weight</code> value. The graph is assumed to be weighted. If any edge on the
        /// graph does not have a weight, a <code>MinimumSpanningTreeException</code> will be thrown.
        /// The algorithm can be defined by the callee. Either <i>Kruskal</i> or <i>Prim</i> algorithms are available.
        /// The <code>msutBeConnected</code> flag, if set to true, will cause this method to throw a <code>MinimumSpanningTreeException</code>
        /// if the graph is not connected. By default this value is false.
        /// The default is Kruskal's algorithm.
        /// <example>
        /// Given a weighted graph, g.
        /// <code>
        /// var mst = g.GenerateMinimumSpanningTree(SpanningTreeAlgorithm.Prim);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="spanningTreeAlgorithm">Algorithm to use.</param>
        /// <param name="mustBeConnected">Flag. If true the graph must be connected.</param>
        /// <returns>List of edgese, representing the minimum spanning tree</returns> 
        public List<Edge> GenerateMinimumSpanningTree(SpanningTreeAlgorithm spanningTreeAlgorithm = SpanningTreeAlgorithm.Kruskal, bool mustBeConnected = false)
        {
            if (mustBeConnected && !IsConnected())
            {
                throw new MinimumSpanningTreeException("Graph is not connected.");
            }
            foreach (var edge in _edges)
            {
                if (this.GetComponent<EdgeWeight>(edge) == null)
                {
                    throw new MinimumSpanningTreeException(String.Format("Edge {0} does not have an attached EdgeWeight component.", edge));
                }
            }
            switch (spanningTreeAlgorithm)
            {
                case SpanningTreeAlgorithm.Kruskal: return GenerateMinimumSpanningTreeKruskal();
                case SpanningTreeAlgorithm.Prim: return GenerateMinimumSpanningTreePrim();
                default: throw new MinimumSpanningTreeException("Unknown minimum spanning tree algorithm.");

            };


        }

        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the
        /// <code>EdgeWeight</code> component's <code>Weight</code> value.
        /// The algorithm uses <i>Kruskal's Algorithm</i>.
        /// </summary>
        /// <returns>Minimum spanning tree, as a list of edges.</returns>
        private List<Edge> GenerateMinimumSpanningTreeKruskal()
        {
            var edges = this.GetEdges();
            edges.Sort((Edge x, Edge y) =>
            {
                return GetComponent<EdgeWeight>(x).Weight.CompareTo(GetComponent<EdgeWeight>(y).Weight);
            });
            List<Edge> gcopy = new List<Edge>(edges);
            List<Edge> stEdges = new List<Edge>();
            List<DisjointSet> ds = new List<DisjointSet>();
            var nodeIndexDict = new Dictionary<Node, int>();
            var nodeList = _nodes.ToList();
            for (int i = 0; i < nodeList.Count; i++)
            {
                ds.Add(new DisjointSet(i, 0));
                nodeIndexDict[nodeList[i]] = i;

            }

            foreach (var edge in edges)
            {
                int x = Find(ds, nodeIndexDict[edge.From()]);
                int y = Find(ds, nodeIndexDict[edge.To()]);

                if (x != y)
                {
                    stEdges.Add(edge);
                    Union(ds, x, y);
                }
            }

            return stEdges;
        }

        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the
        /// <code>EdgeWeight</code> component's <code>Weight</code> value.
        /// The algorithm uses <i>Prim's Algorithm</i>.
        /// </summary>
        /// <returns>Minimum spanning tree, as a list of edges.</returns>
        private List<Edge> GenerateMinimumSpanningTreePrim()
        {
            var treeEdges = new List<Edge>();
            var r = new System.Random();
            var minDistMap = new Dictionary<Node, (float, Edge)>();
            _nodes.ToList().ForEach(n =>
            {
                minDistMap[n] = (float.PositiveInfinity, GetIncidentEdges(n)[0]); // (A,B) is a dummy edge

            });

            var dPQ = new C5.IntervalHeap<Node>(new PrimNodeComparer(minDistMap));
            dPQ.AddAll(_nodes);
            var next = r.Next(this._nodes.Count);
            var node = _nodes.ToList()[next];

            var treeNodes = new HashSet<Node>();
            while (treeNodes.Count < this._nodes.Count)
            {
                var adjacentNodes = this.GetAdjacent(node);
                adjacentNodes.ForEach(n =>
                {
                    var hs = new HashSet<Node>();
                    hs.Add(node);
                    hs.Add(n);
                    var edge = GetEdge(hs).Value; // we can safely assume not null
                    var w = GetComponent<EdgeWeight>(edge).Weight;
                    if (treeNodes.Contains(n) == false && minDistMap[n].Item1 > w)
                    {
                        minDistMap[n] = (w, edge);
                    }
                });
                treeNodes.Add(node);

                dPQ = new C5.IntervalHeap<Node>(new PrimNodeComparer(minDistMap));
                dPQ.AddAll(_nodes.Except(treeNodes));
                while (dPQ.Count > 0)
                {

                    node = dPQ.DeleteMin();

                    if (!treeNodes.Contains(node))
                    {
                        break; // if node is not in tree nodes we continue in outer loop
                    }
                }

                if (dPQ.Count == 0)
                {
                    break;
                }


            }


            return minDistMap.Values.ToList()
            .Where(i => i.Item1 < float.PositiveInfinity) // remove the initial node's edge.
            .Select(v => v.Item2).ToHashSet().ToList(); // listify the result edges, after dropping duplicates
        }


        /// <summary>
        /// Generates a spanning tree on the graph. THe graph need not be weighted or connected.
        /// The resulting spanning tree, need not be minimum in any sense, and may not be unique.
        /// <example>
        /// Given a graph, g.
        /// <code>
        /// var mst = g.GenerateSpanningTree();
        /// </code>
        /// </example> 
        /// </summary>
        /// <returns>Spanning tree, as a list of edges.</returns>
        public List<Edge> GenerateSpanningTree()
        {
            var edges = this.GetEdges();
            List<Edge> gcopy = new List<Edge>(edges);
            List<Edge> stEdges = new List<Edge>();
            List<DisjointSet> ds = new List<DisjointSet>();
            var nodeIndexDict = new Dictionary<Node, int>();
            var nodeList = _nodes.ToList();
            for (int i = 0; i < nodeList.Count; i++)
            {
                ds.Add(new DisjointSet(i, 0));
                nodeIndexDict[nodeList[i]] = i;

            }

            foreach (var edge in edges)
            {
                int x = Find(ds, nodeIndexDict[edge.From()]);
                int y = Find(ds, nodeIndexDict[edge.To()]);

                if (x != y)
                {
                    stEdges.Add(edge);
                    Union(ds, x, y);
                }
            }

            return stEdges;
        }

        internal int Find(List<DisjointSet> subsets, int node)
        {
            if (subsets[node].parent != node)
            {
                subsets[node].parent = Find(subsets, subsets[node].parent);
            }
            return subsets[node].parent;
        }


        internal void Union(List<DisjointSet> subsets, int a, int b)
        {
            var rootA = Find(subsets, a);
            var rootB = Find(subsets, b);

            if (subsets[rootA].rank < subsets[rootB].rank)
            {
                subsets[rootA].parent = rootB;
            }
            else
            {
                subsets[rootB].parent = rootA;
                if (subsets[rootB].rank == subsets[rootA].rank)
                {
                    subsets[rootA].rank++;
                }
            }
        }
    }
}