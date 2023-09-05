// <copyright file="Graph.Cycles.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Graph
    {
        /// <summary>
        /// Finds all simple cycles in the graph. Graph is not assumed to be connected. The returned
        /// lists contain the unique cycles of the graph, i.e. there should be no duplicate
        /// cycles.
        /// <see href="https://mathworld.wolfram.com/GraphCycle.html">Graph Cycles</see>.
        /// </summary>
        /// <returns>A list of lists, where each list represents a single cycle.</returns>
        public List<List<Node>> FindSimpleCycles()
        {
            var nodeDict = new Dictionary<Node, NodeSearchMemory>();
            var edgeDict = new Dictionary<Edge, EdgeSearchMemory>();
            this.nodes.ToList().ForEach(n => nodeDict[n] = new NodeSearchMemory());
            this.edges.ForEach(e => edgeDict[e] = new EdgeSearchMemory());

            List<Graph> connectedSubgraphs = this.FindMaximallyConnectedSubgraphs();

            List<List<Node>> cycleList = new List<List<Node>>();
            foreach (Graph g in connectedSubgraphs)
            {
                cycleList.AddRange(g.FindAllCycles(nodeDict, edgeDict));
            }

            return cycleList;
        }

        /// <summary>
        /// Returns <i>true</i> if the graph is <b>chordal</b>, <i>false</i> otherwise. A chordal graph
        /// is a graph in which all cycles of length at least 4 have no chords; edges connecting
        /// two nodes on the graph which are not part of the cycle.
        /// </summary>
        /// <returns>true, if chordal. false otherwise.</returns>
        public bool IsChordal()
        {
            var nodeDict = new Dictionary<Node, NodeSearchMemory>();
            var edgeDict = new Dictionary<Edge, EdgeSearchMemory>();
            this.nodes.ToList().ForEach(n => nodeDict[n] = new NodeSearchMemory());
            this.edges.ForEach(e => edgeDict[e] = new EdgeSearchMemory());
            var cycles = this.FindAllCycles(nodeDict, edgeDict);
            var cycles4 = cycles.Where(c => c.Count >= 4).ToList();
            foreach (var cycle in cycles4)
            {
                var hashSet = new HashSet<Node>(cycle);
                var cycDict = new Dictionary<Node, int>();
                for (int i = 0; i < cycle.Count; i++)
                {
                    cycDict[cycle[i]] = i;
                }

                var chords = this.edges
                    .Where(
                        e =>
                            hashSet.Contains(e.From())
                            && hashSet.Contains(e.To())
                            && Math.Abs(cycDict[e.From()] - cycDict[e.To()]) > 1
                            && Math.Abs(cycDict[e.From()] - cycDict[e.To()]) < cycle.Count - 1
                    )
                    .ToList();

                if (chords.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static List<Node> ReorderList(List<Node> lst)
        {
            int len = lst.Count;
            int minIndex = 0;
            int minHash = 0;
            for (int i = 0; i < lst.Count; i++)
            {
                if (i == 0)
                {
                    minIndex = 0;
                    minHash = lst[0].GetHashCode();
                }
                else
                {
                    int h = lst[i].GetHashCode();
                    if (h < minHash)
                    {
                        minIndex = i;
                        minHash = h;
                    }
                }
            }

            if (
                lst[(len + minIndex - 1) % len].GetHashCode()
                < lst[(minIndex + 1) % len].GetHashCode()
            )
            {
                lst = lst.Reverse<Node>().ToList<Node>();
                return ReorderList(lst);
            }
            else
            {
                List<Node> rotateLst = new List<Node>();
                for (int i = 0; i < lst.Count; i++)
                {
                    rotateLst.Add(lst[(i + minIndex) % lst.Count]);
                }

                return rotateLst;
            }
        }

        private List<List<Node>> FindAllCycles(
            Dictionary<Node, NodeSearchMemory> nodeMemDict,
            Dictionary<Edge, EdgeSearchMemory> edgeMemDict
        )
        {
            // Get the initial start node. Search for cycles from this node.
            // Because the graph is connected we will be able to find all possible
            // cycles.
            Node first = this.nodes.First();

            List<List<Node>> cycleList = new List<List<Node>>();
            List<ListWrapper<Node>> wrappedCycleList = new List<ListWrapper<Node>>();
            List<Node> adjacent = this.GetAdjacent(first);

            // begin the cycle search...
            foreach (Node adj in adjacent)
            {
                Stack<Node> path = new Stack<Node>();
                path.Push(first);
                wrappedCycleList.AddRange(
                    this.AddToPath(first, adj, path, nodeMemDict, edgeMemDict)
                );
            }

            wrappedCycleList.OrderBy(l => l.GetHashCode());
            HashSet<ListWrapper<Node>> wrappedSet = new HashSet<ListWrapper<Node>>();
            foreach (ListWrapper<Node> l in wrappedCycleList)
            {
                wrappedSet.Add(l);
            }

            foreach (ListWrapper<Node> w in wrappedSet)
            {
                cycleList.Add(w.Data);
            }

            return cycleList;
        }

        private List<ListWrapper<Node>> AddToPath(
            Node previous,
            Node current,
            Stack<Node> path,
            Dictionary<Node, NodeSearchMemory> nodeMemDict,
            Dictionary<Edge, EdgeSearchMemory> edgeMemDict
        )
        {
            List<ListWrapper<Node>> cycleList = new List<ListWrapper<Node>>();
            List<Node> pathList = new List<Node>(path);

            // reverse the pathList because iterating is like popping for Lists derived from Stacks.
            pathList.Reverse();

            for (int i = 0; i < pathList.Count - 1; i++)
            {
                if (pathList[i].Equals(current))
                {
                    List<Node> cycle = new List<Node>();

                    for (int j = i; j < pathList.Count; j++)
                    {
                        cycle.Add(pathList[j]);
                    }

                    cycle = ReorderList(cycle);
                    ListWrapper<Node> lw = new ListWrapper<Node>(cycle);
                    cycleList.Add(lw);

                    return cycleList;
                }
            }

            List<Edge> incident = this.GetIncidentEdges(current);

            path.Push(current);
            HashSet<Node> currentHS = new HashSet<Node>();
            currentHS.Add(current);
            foreach (Edge e in incident)
            {
                if (!edgeMemDict[e].Visited)
                {
                    edgeMemDict[e].Visited = true;
                    HashSet<Node> searchNodes = new HashSet<Node>(e.Nodes());
                    searchNodes.ExceptWith(currentHS);

                    foreach (Node n in searchNodes)
                    {
                        if (!n.Equals(previous))
                        {
                            cycleList.AddRange(
                                this.AddToPath(current, n, path, nodeMemDict, edgeMemDict)
                            );
                        }
                    }
                }
            }

            foreach (Edge e in incident)
            {
                edgeMemDict[e].Visited = false;
            }

            path.Pop();
            return cycleList;
        }
    }

    internal class ListWrapper<T>
    {
        public readonly List<T> Data;

        public ListWrapper(List<T> d)
        {
            this.Data = d;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ListWrapper<T>;

            if (other == null)
            {
                return false;
            }

            if (this.Data.Count != other.Data.Count)
            {
                return false;
            }

            for (int i = 0; i < this.Data.Count; i++)
            {
                if (this.Data[i].GetHashCode() != other.Data[i].GetHashCode())
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < this.Data.Count; i++)
            {
                int sum = this.Data[i].GetHashCode();
                hash += sum;
            }

            return hash;
        }
    }
}
