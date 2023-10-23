// <copyright file="Graph.Planarity.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SharpGraph
{
    internal enum Orientation
    {
        Forward,
        Backward,
        None,
    }

    public partial class Graph
    {
        /// <summary>
        /// Checks whether the graph is planar or not. A <i>Planar</i> graph is a graph that has a <i>planar embedding</i>, i.e. it can be drawn on the plane
        /// with no crossing edges. This method uses an implementation of the "left-right approach" to planarity testing,
        /// and is heavily based off the networkx implementation.
        /// This method will return true if the graph is planar, and false otherwise. No "proof": i.e. a counterexample or an example embedding are provided.
        /// <see href="https://networkx.org/documentation/stable/_modules/networkx/algorithms/planarity.html#CheckPlanarity" networkx implementation..................... </see>
        /// <see href="https://citeseerx.ist.psu.edu/viewdoc/download;jsessionid=70329F8449DE1E839C85438D379BE36C?doi=10.1.1.217.9208&rep=rep1&type=pdf">algorithm.</see>
        ///
        /// <returns></returns></summary>
        /// <returns>true if planar, false otherwise.</returns>
        public bool IsPlanar()
        {
            return this.CheckPlanarity().Item1;
        }

        /// <summary>
        /// Checks whether the graph is planar. A bool result is returned in a tuple with a planar embedding if the graph is planar or null otherwise.
        /// <see href="https://networkx.org/documentation/stable/_modules/networkx/algorithms/planarity.html#CheckPlanarity" networkx implementation..................... </see>
        /// <see href="https://citeseerx.ist.psu.edu/viewdoc/download;jsessionid=70329F8449DE1E839C85438D379BE36C?doi=10.1.1.217.9208&rep=rep1&type=pdf">algorithm.</see>
        ///
        /// <returns></returns></summary>
        /// <returns>Tuple of bool and PlanarEmbedding. If no planar, no planar embedding is returned.</returns>
        public (bool, PlanarEmbedding) CheckPlanarity()
        {
            if (this.edges.Count > (3 * this.nodes.Count) - 6)
            {
                return (false, null);
            }

            var planarEmbedding = new PlanarEmbedding(this);
            var hasEmbedding = this.FindOrientation(new PlanarityChecker(), planarEmbedding);

            return (hasEmbedding, planarEmbedding);
        }

        internal void DFSEmbedding(Node v, PlanarityChecker pc, PlanarEmbedding pe)
        {
            foreach (var w in pc.OrderedAdjacentNodes[v])
            {
                var ei = (v, w);
                if (pc.ParentEdge.ContainsKey(w) && pc.ParentEdge[w] == ei)
                {
                    pe.AddHalfEdgeFirst(w, v);
                    pc.LeftRef[v] = w;
                    pc.RightRef[v] = w;
                    this.DFSEmbedding(w, pc, pe);
                }
                else
                {
                    if (pc.Side[ei] == 1)
                    {
                        pe.AddHalfEdgeCW(w, v, pc.RightRef[w]);
                    }
                    else
                    {
                        pe.AddHalfEdgeCCW(w, v, pc.LeftRef[w]);
                        pc.LeftRef[w] = v;
                    }
                }
            }
        }

        private bool FindOrientation(
            PlanarityChecker planarityChecker,
            PlanarEmbedding planarEmbedding
        )
        {
            planarityChecker.Height = new Dictionary<Node, int>();

            foreach (var n in this.nodes)
            {
                planarityChecker.Height[n] = int.MaxValue;
            }

            foreach (var n in this.nodes)
            {
                if (planarityChecker.Height[n] == int.MaxValue)
                {
                    planarityChecker.Height[n] = 0;
                    planarityChecker.Roots.Add(n);
                    this.DfsOrientation(n, planarityChecker);
                }
            }

            foreach (var oe in planarityChecker.OrientedEdges)
            {
                planarityChecker.Side[oe] = 1;
            }

            this.GenerateOrderedAdjLists(planarityChecker);
            foreach (var n in planarityChecker.Roots)
            {
                if (!this.DFSTesting(n, planarityChecker))
                {
                    return false; // no planar embedding
                }
            }

            foreach (var oe in planarityChecker.OrientedEdges)
            {
                planarityChecker.NestingDepth[oe] =
                    planarityChecker.Sign(oe) * planarityChecker.NestingDepth[oe];
            }

            this.GenerateOrderedAdjLists(planarityChecker);

            foreach (var v in this.nodes)
            {
                Node? prevNode = null;
                foreach (var w in planarityChecker.OrderedAdjacentNodes[v])
                {
                    planarEmbedding.AddHalfEdgeCW(v, w, prevNode);
                    prevNode = w;
                }
            }

            foreach (var n in planarityChecker.Roots)
            {
                this.DFSEmbedding(n, planarityChecker, planarEmbedding);
            }

            return true;
        }

        private void DfsOrientation(Node v, PlanarityChecker planarityChecker)
        {
            _ = planarityChecker.ParentEdge.TryGetValue(v, out var e);
            foreach (var w in this.GetAdjacent(v))
            {
                if (
                    planarityChecker.OrientedEdges.Contains((w, v))
                    || planarityChecker.OrientedEdges.Contains((v, w))
                )
                {
                    continue;
                }

                var vw = (v, w);
                planarityChecker.OrientedEdges.Add(vw);
                planarityChecker.LowPt[vw] = planarityChecker.Height[v];
                planarityChecker.LowPt2[vw] = planarityChecker.Height[v];
                if (planarityChecker.Height[w] == int.MaxValue)
                {
                    planarityChecker.ParentEdge[w] = vw;
                    planarityChecker.Height[w] = planarityChecker.Height[v] + 1;
                    this.DfsOrientation(w, planarityChecker);
                }
                else
                {
                    planarityChecker.LowPt[vw] = planarityChecker.Height[w];
                }

                planarityChecker.NestingDepth[vw] = 2 * planarityChecker.LowPt[vw];
                if (planarityChecker.LowPt2[vw] < planarityChecker.Height[v])
                {
                    planarityChecker.NestingDepth[vw] += 1;
                }

                if (e != null)
                {
                    if (planarityChecker.LowPt[vw] < planarityChecker.LowPt[e.Value])
                    {
                        planarityChecker.LowPt2[e.Value] = Math.Min(
                            planarityChecker.LowPt[e.Value],
                            planarityChecker.LowPt2[vw]
                        );
                        planarityChecker.LowPt[e.Value] = planarityChecker.LowPt[vw];
                    }
                    else if (planarityChecker.LowPt[vw] > planarityChecker.LowPt[e.Value])
                    {
                        planarityChecker.LowPt2[e.Value] = Math.Min(
                            planarityChecker.LowPt2[e.Value],
                            planarityChecker.LowPt[vw]
                        );
                    }
                    else
                    {
                        planarityChecker.LowPt2[e.Value] = Math.Min(
                            planarityChecker.LowPt2[e.Value],
                            planarityChecker.LowPt2[vw]
                        );
                    }
                }
            }
        }

        private void GenerateOrderedAdjLists(PlanarityChecker pc)
        {
            pc.OrderedAdjacentNodes = new Dictionary<Node, List<Node>>();
            var orderedAdjacent = new Dictionary<Node, List<Node>>();
            foreach (var n in this.nodes)
            {
                var adj = this.GetAdjacent(n)
                    .Where(x =>
                    {
                        return pc.NestingDepth.ContainsKey((n, x))
                            && pc.OrientedEdges.Contains((n, x));
                    })
                    .ToList();

                adj.Sort(
                    (e1, e2) =>
                    {
                        var nd1 = pc.NestingDepth[(n, e1)];
                        var nd2 = pc.NestingDepth[(n, e2)];
                        return nd1.CompareTo(nd2);
                    }
                );

                pc.OrderedAdjacentNodes[n] = adj;
            }
        }

        private bool DFSTesting(Node v, PlanarityChecker planarityChecker)
        {
            _ = planarityChecker.ParentEdge.TryGetValue(v, out var parentEdge);
            var orderedAdjacents = planarityChecker.OrderedAdjacentNodes;

            foreach (var w in orderedAdjacents[v])
            {
                var ei = (v, w);
                planarityChecker.StackBottom[ei] = planarityChecker.TopOfStack();
                _ = planarityChecker.ParentEdge.TryGetValue(w, out var wParentEdge);
                if (wParentEdge != null && ei == wParentEdge)
                {
                    if (!this.DFSTesting(w, planarityChecker))
                    {
                        return false;
                    }
                }
                else
                {
                    planarityChecker.LowPtEdge[ei] = ei;
                    planarityChecker.S.Add(new ConflictPair(null, new Interval(ei, ei)));
                }

                if (planarityChecker.LowPt[ei] < planarityChecker.Height[v])
                {
                    var zeroIdx = orderedAdjacents[v][0];
                    if (w == zeroIdx)
                    {
                        planarityChecker.LowPtEdge[parentEdge.Value] = planarityChecker.LowPtEdge[
                            ei
                        ];
                    }
                    else if (planarityChecker.AddConstraints(ei, parentEdge.Value) == false)
                    {
                        return false;
                    }
                }
            }

            if (parentEdge != null)
            {
                planarityChecker.RemoveBackEdges(this, parentEdge.Value);
            }

            return true;
        }
    }

    /// <summary>
    /// Planar Embedding of a graph.
    /// </summary>
    public class PlanarEmbedding
    {
        public Graph OriginalGraph;
        public List<(Node, Node)> Edges;
        public Dictionary<(Node, Node), Node> Cw;
        public Dictionary<(Node, Node), Node> Ccw;
        public Dictionary<Node, Node> FirstNeighbor;

        public PlanarEmbedding(Graph g)
        {
            this.OriginalGraph = g;
            this.Edges = new List<(Node, Node)>();
            this.Cw = new Dictionary<(Node, Node), Node>();
            this.Ccw = new Dictionary<(Node, Node), Node>();
            this.FirstNeighbor = new Dictionary<Node, Node>();
        }

        internal static void TraverseFace(Node n, Node m, bool markHalfEdges = false) { }

        internal void AddHalfEdgeCW(Node first, Node end, Node? refN)
        {
            this.Edges.Add((first, end));
            if (refN == null)
            {
                var edge = (first, end);
                this.Cw[edge] = end;
                this.Ccw[edge] = end;
                this.FirstNeighbor[first] = end;
                return;
            }

            var cwRef = this.Cw[(first, refN.Value)];
            this.Cw[(first, refN.Value)] = end;
            this.Cw[(first, end)] = cwRef;
            this.Ccw[(first, cwRef)] = end;
            this.Ccw[(first, end)] = refN.Value;
        }

        internal void AddHalfEdgeCCW(Node first, Node end, Node? refN)
        {
            if (refN == null)
            {
                var edge = (first, end);
                this.Cw[edge] = end;
                this.Ccw[edge] = end;
                this.FirstNeighbor[first] = end;
            }
            else
            {
                var ccwRef = this.Ccw[(first, refN.Value)];
                this.AddHalfEdgeCW(first, end, ccwRef);

                if (this.FirstNeighbor.ContainsKey(first) && refN == this.FirstNeighbor[first])
                {
                    this.FirstNeighbor[first] = end;
                }
            }
        }

        internal void ConnectComponents(Node n, Node m)
        {
            this.AddHalfEdgeFirst(n, m);
            this.AddHalfEdgeFirst(m, n);
        }

        internal void AddHalfEdgeFirst(Node start, Node end)
        {
            if (this.FirstNeighbor.ContainsKey(start))
            {
                var r = this.FirstNeighbor[start];
                this.AddHalfEdgeCCW(start, end, r);
            }
            else
            {
                this.AddHalfEdgeCCW(start, end, null);
            }
        }

        internal (Node, Node) NextFaceHalfEdge(Node n, Node m)
        {
            return (m, this.Ccw[(m, n)]);
        }
    }

    public class ConflictPair
    {
        internal Interval Left;
        internal Interval Right;

        public ConflictPair(Interval l, Interval r)
        {
            if (l == null)
            {
                l = new Interval(null, null);
            }

            this.Left = l;

            if (r == null)
            {
                r = new Interval(null, null);
            }

            this.Right = r;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConflictPair)
            {
                return this.Equals((ConflictPair)obj);
            }

            return false;
        }

        public bool Equals(ConflictPair other)
        {
            return this.Left.Equals(other.Left) && this.Right.Equals(other.Right);
        }

        public override int GetHashCode()
        {
            var h = (this.Left.GetHashCode() ^ 38334421) + (this.Right.GetHashCode() * 97);
            return h;
        }

        internal void Swap()
        {
            var tmp = this.Left;
            this.Left = this.Right;
            this.Right = tmp;
        }

        internal int Lowest(PlanarityChecker pc)
        {
            if (this.Left.Empty())
            {
                return pc.LowPt[this.Right.Low.Value];
            }

            if (this.Right.Empty())
            {
                return pc.LowPt[this.Left.Low.Value];
            }

            return System.Math.Min(pc.LowPt[this.Left.Low.Value], pc.LowPt[this.Right.Low.Value]);
        }
    }

    public class Interval
    {
        internal (Node, Node)? Low;
        internal (Node, Node)? High;

        public Interval((Node, Node)? low, (Node, Node)? high)
        {
            this.Low = low;
            this.High = high;
        }

        public override bool Equals(object obj)
        {
            if (obj is Interval)
            {
                return this.Equals((Interval)obj);
            }

            return false;
        }

        public bool Equals(Interval other)
        {
            return this.Low == other.Low && this.High == other.High;
        }

        public override int GetHashCode()
        {
            var x = 38334421;
            if (this.Low != null)
            {
                x ^= this.Low.GetHashCode();
            }

            var y = 97;
            if (this.High != null)
            {
                x *= this.High.GetHashCode();
            }

            var h = x + y;
            return h;
        }

        internal bool Empty()
        {
            return this.Low == null && this.High == null;
        }

        internal Interval Copy()
        {
            return new Interval(this.Low, this.High); // TODO deep copy?
        }

        internal bool Conflicting((Node, Node) edge, PlanarityChecker pc)
        {
            return this.Empty() == false && pc.LowPt[this.High.Value] > pc.LowPt[edge];
        }
    }

    internal class HalfEdge { }

    /// <summary>
    /// Class that provides the majority of the algorithm implementation for checking
    /// planarity.
    /// </summary>
    internal class PlanarityChecker
    {
        internal Dictionary<Node, int> Height;
        internal Dictionary<Node, (Node, Node)?> ParentEdge;
        internal Dictionary<Node, List<Node>> OrderedAdjacentNodes;
        internal Dictionary<(Node, Node), int> LowPt;
        internal Dictionary<(Node, Node), int> LowPt2;
        internal Dictionary<(Node, Node), int> Side;
        internal Dictionary<(Node, Node), (Node, Node)> LowPtEdge;
        internal Dictionary<(Node, Node), (Node, Node)?> Refx;
        internal List<Node> Roots;
        internal Dictionary<(Node, Node), ConflictPair> StackBottom;
        internal List<ConflictPair> S;
        internal Dictionary<(Node, Node), int> NestingDepth;
        internal List<(Node, Node)> OrientedEdges;
        internal Dictionary<Node, Node> RightRef;
        internal Dictionary<Node, Node> LeftRef;

        internal PlanarityChecker()
        {
            this.Height = new Dictionary<Node, int>();
            this.LowPtEdge = new Dictionary<(Node, Node), (Node, Node)>();
            this.ParentEdge = new Dictionary<Node, (Node, Node)?>();
            this.LowPt = new Dictionary<(Node, Node), int>();
            this.LowPt2 = new Dictionary<(Node, Node), int>();
            this.Side = new Dictionary<(Node, Node), int>();
            this.NestingDepth = new Dictionary<(Node, Node), int>();
            this.Refx = new Dictionary<(Node, Node), (Node, Node)?>();
            this.Roots = new List<Node>();
            this.OrientedEdges = new List<(Node, Node)>();
            this.RightRef = new Dictionary<Node, Node>();
            this.LeftRef = new Dictionary<Node, Node>();
            this.StackBottom = new Dictionary<(Node, Node), ConflictPair>();
            this.S = new List<ConflictPair>();
        }

        public int Sign((Node, Node) edge)
        {
            if (this.Refx.ContainsKey(edge) && this.Refx[edge] != null)
            {
                this.Side[edge] *= this.Sign(this.Refx[edge].Value);
                this.Refx[edge] = null;
            }

            return this.Side[edge];
        }

        internal ConflictPair TopOfStack()
        {
            if (this.S.Count == 0)
            {
                return null;
            }
            else
            {
                return this.S[this.S.Count - 1];
            }
        }

        internal bool AddConstraints((Node, Node) ei, (Node, Node) e)
        {
            var p = new ConflictPair(null, null);
            while (true)
            {
                var q = this.PopS();

                if (!q.Left.Empty())
                {
                    q.Swap();
                }

                if (!q.Left.Empty())
                {
                    return false; // not planar
                }

                if (this.LowPt[q.Right.Low.Value] > this.LowPt[e])
                {
                    if (p.Right.Empty())
                    { // topmost interval
                        p.Right = q.Right.Copy();
                    }
                    else
                    {
                        this.Refx[p.Right.Low.Value] = q.Right.High.Value;
                    }

                    p.Right.Low = q.Right.Low;
                }
                else
                {
                    this.Refx[q.Right.Low.Value] = this.LowPtEdge[e];
                }

                _ = this.S[this.S.Count - 1];
                _ = this.StackBottom[ei];
                if (this.S[this.S.Count - 1].Equals(this.StackBottom[ei]))
                {
                    break;
                }
            }

            while (
                this.S[this.S.Count - 1].Left.Conflicting(ei, this)
                || this.S[this.S.Count - 1].Right.Conflicting(ei, this)
            )
            {
                var q = this.PopS();
                if (q.Right.Conflicting(ei, this))
                {
                    q.Swap();
                }

                if (q.Right.Conflicting(ei, this))
                {
                    return false;
                }

                this.Refx[p.Right.Low.Value] = q.Right.High;
                if (q.Right.Low != null)
                {
                    p.Right.Low = q.Right.Low;
                }

                if (p.Left.Empty())
                {
                    p.Left = q.Left.Copy();
                }
                else
                {
                    this.Refx[p.Left.Low.Value] = q.Left.High;
                }

                p.Left.Low = q.Left.Low;
            }

            if (!(p.Left.Empty() && p.Right.Empty()))
            {
                this.S.Add(p);
            }

            return true;
        }

        internal void RemoveBackEdges(Graph g, (Node, Node) e)
        {
            var u = e.Item1;
            while (this.S.Count > 0 && this.TopOfStack().Lowest(this) == this.Height[u])
            {
                var p = this.PopS();
                if (p.Left.Low != null)
                {
                    this.Side[p.Left.Low.Value] = -1;
                }
            }

            if (this.S.Count > 0)
            {
                var p = this.PopS();
                while (p.Left.High != null && p.Left.High.Value.Item2 == u)
                {
                    p.Left.High = this.Refx[p.Left.High.Value];
                }

                if (p.Left.High == null && p.Left.Low != null)
                {
                    this.Refx[p.Left.Low.Value] = p.Right.Low.Value;
                    this.Side[p.Left.Low.Value] = -1;
                    p.Left.Low = null;
                }

                while (p.Right.High != null && p.Right.High.Value.Item2 == u)
                {
                    p.Right.High = this.Refx[p.Right.High.Value];
                }

                if (p.Right.High == null && p.Right.Low != null)
                {
                    this.Refx[p.Right.Low.Value] = p.Left.Low.Value;
                    this.Side[p.Right.Low.Value] = -1;
                    p.Right.Low = null;
                }

                this.S.Add(p);
            }

            if (this.LowPt[e] < this.Height[u])
            {
                var hl = this.TopOfStack().Left.High;
                var hr = this.TopOfStack().Right.High;
                if (hl != null && (hr == null || this.LowPt[hl.Value] > this.LowPt[hr.Value]))
                {
                    this.Refx[e] = hl.Value;
                }
                else
                {
                    this.Refx[e] = hr.Value;
                }
            }
        }

        private ConflictPair PopS()
        {
            var q = this.S[this.S.Count - 1];
            this.S.RemoveAt(this.S.Count - 1);
            return q;
        }
    }
}
