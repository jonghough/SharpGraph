using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {

        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the
        /// <code>EdgeWeight</code> component's <code>Weight</code> value.
        /// The algorithm uses <i>Kruskal's Algorithm</i>.
        /// </summary>
        /// <returns>Minimum spanning tree, as a list of edges.</returns>
        public List<Edge> GenerateMinimumSpanningTree()
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
        /// Generates a spanning tree on the graph.
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