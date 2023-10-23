// <copyright file="Graph.Cycles.cs" company="Jonathan Hough">
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

            var connectedSubgraphs = this.FindMaximallyConnectedSubgraphs();

            var cycleList = new List<List<Node>>();
            foreach (var g in connectedSubgraphs)
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
                for (var i = 0; i < cycle.Count; i++)
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
            var len = lst.Count;
            var minIndex = 0;
            var minHash = 0;
            for (var i = 0; i < lst.Count; i++)
            {
                if (i == 0)
                {
                    minIndex = 0;
                    minHash = lst[0].GetHashCode();
                }
                else
                {
                    var h = lst[i].GetHashCode();
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
                var rotateLst = new List<Node>();
                for (var i = 0; i < lst.Count; i++)
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
            var first = this.nodes.First();

            var cycleList = new List<List<Node>>();
            var wrappedCycleList = new List<ListWrapper<Node>>();
            var adjacent = this.GetAdjacent(first);

            // begin the cycle search...
            foreach (var adj in adjacent)
            {
                var path = new Stack<Node>();
                path.Push(first);
                wrappedCycleList.AddRange(
                    this.AddToPath(first, adj, path, nodeMemDict, edgeMemDict)
                );
            }

            wrappedCycleList.OrderBy(l => l.GetHashCode());
            var wrappedSet = new HashSet<ListWrapper<Node>>();
            foreach (var l in wrappedCycleList)
            {
                wrappedSet.Add(l);
            }

            foreach (var w in wrappedSet)
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
            var cycleList = new List<ListWrapper<Node>>();
            var pathList = new List<Node>(path);

            // reverse the pathList because iterating is like popping for Lists derived from Stacks.
            pathList.Reverse();

            for (var i = 0; i < pathList.Count - 1; i++)
            {
                if (pathList[i].Equals(current))
                {
                    var cycle = new List<Node>();

                    for (var j = i; j < pathList.Count; j++)
                    {
                        cycle.Add(pathList[j]);
                    }

                    cycle = ReorderList(cycle);
                    var lw = new ListWrapper<Node>(cycle);
                    cycleList.Add(lw);

                    return cycleList;
                }
            }

            var incident = this.GetIncidentEdges(current);

            path.Push(current);
            var currentHS = new HashSet<Node>();
            currentHS.Add(current);
            foreach (var e in incident)
            {
                if (!edgeMemDict[e].Visited)
                {
                    edgeMemDict[e].Visited = true;
                    var searchNodes = new HashSet<Node>(e.Nodes());
                    searchNodes.ExceptWith(currentHS);

                    foreach (var n in searchNodes)
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

            foreach (var e in incident)
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
            if (obj is not ListWrapper<T> other)
            {
                return false;
            }

            if (this.Data.Count != other.Data.Count)
            {
                return false;
            }

            for (var i = 0; i < this.Data.Count; i++)
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
            var hash = 0;
            for (var i = 0; i < this.Data.Count; i++)
            {
                var sum = this.Data[i].GetHashCode();
                hash += sum;
            }

            return hash;
        }
    }
}
