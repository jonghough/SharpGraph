using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {

        /// <summary>
        /// Checks whether the graph is planar or not. A <i>Planar</i> graph is a graph that has a <i>planar embedding</i>, i.e. it can be drawn on the plane
        /// with no crossing edges. This method uses an implementation of the "left-right approach" to planarity testing, 
        /// and is heavily based off the networkx implementation.
        /// This method will return true if the graph is planar, and false otherwise. No "proof": i.e. a counterexample or an example embedding are provided.
        /// <see href="https://networkx.org/documentation/stable/_modules/networkx/algorithms/planarity.html#CheckPlanarity" networkx implementation </see>
        /// <see href="https://citeseerx.ist.psu.edu/viewdoc/download;jsessionid=70329F8449DE1E839C85438D379BE36C?doi=10.1.1.217.9208&rep=rep1&type=pdf">algorithm.</see> 
        /// </summary>
        /// <returns>true if planar, false otherwise.</returns>
        public bool IsPlanar()
        {
            return CheckPlanarity().Item1;
        }

        /// <summary>
        /// Checks whether the graph is planar. A bool result is returned in a tuple with a planar embedding if the graph is planar or null otherwise.
        /// <see href="https://networkx.org/documentation/stable/_modules/networkx/algorithms/planarity.html#CheckPlanarity" networkx implementation </see>
        /// <see href="https://citeseerx.ist.psu.edu/viewdoc/download;jsessionid=70329F8449DE1E839C85438D379BE36C?doi=10.1.1.217.9208&rep=rep1&type=pdf">algorithm.</see> 
        /// </summary>
        /// <returns>Tuple of bool and PlanarEmbedding. If no planar, no planar embedding is returned.</returns>
        public (bool, PlanarEmbedding) CheckPlanarity()
        {
            if (this._edges.Count > 3 * this._nodes.Count - 6)
            {
                return (false, null);
            }
            var planarEmbedding = new PlanarEmbedding(this);
            bool hasEmbedding = FindOrientation(new PlanarityChecker(), planarEmbedding);

            return (hasEmbedding, planarEmbedding);
        }


        private bool FindOrientation(PlanarityChecker planarityChecker, PlanarEmbedding planarEmbedding)
        {
            planarityChecker.height = new Dictionary<Node, int>();

            foreach (var n in _nodes)
            {
                planarityChecker.height[n] = int.MaxValue;
            }

            foreach (var n in _nodes)
            {
                if (planarityChecker.height[n] == int.MaxValue)
                {
                    planarityChecker.height[n] = 0;
                    planarityChecker.roots.Add(n);
                    DfsOrientation(n, planarityChecker);
                }

            }

            foreach (var oe in planarityChecker.orientedEdges)
            {
                planarityChecker.side[oe] = 1;
            }

            GenerateOrderedAdjLists(planarityChecker);
            foreach (var n in planarityChecker.roots)
            {
                if (!DFSTesting(n, planarityChecker))
                {
                    return false; //no planar embedding
                }
            }

            foreach (var oe in planarityChecker.orientedEdges)
            {
                planarityChecker.nestingDepth[oe] = planarityChecker.Sign(oe) * planarityChecker.nestingDepth[oe];
            }

            GenerateOrderedAdjLists(planarityChecker);

            foreach (var v in _nodes)
            {
                Node? prevNode = null;
                foreach (var w in planarityChecker.orderedAdjacentNodes[v])
                {
                    planarEmbedding.AddHalfEdgeCW(v, w, prevNode);
                    prevNode = w;
                }
            }


            foreach (var n in planarityChecker.roots)
            {
                DFSEmbedding(n, planarityChecker, planarEmbedding);
            }
            return true;
        }
        private void DfsOrientation(Node v, PlanarityChecker planarityChecker)
        {
            bool hasParent = planarityChecker.parentEdge.TryGetValue(v, out (Node, Node)? e);
            foreach (var w in this.GetAdjacent(v))
            {
                if (planarityChecker.orientedEdges.Contains((w, v)) || planarityChecker.orientedEdges.Contains((v, w)))
                {
                    continue;
                }
                var vw = (v, w);
                planarityChecker.orientedEdges.Add(vw);
                planarityChecker.lowPt[vw] = planarityChecker.height[v];
                planarityChecker.lowPt2[vw] = planarityChecker.height[v];
                if (planarityChecker.height[w] == int.MaxValue)
                {
                    planarityChecker.parentEdge[w] = vw;
                    planarityChecker.height[w] = planarityChecker.height[v] + 1;
                    DfsOrientation(w, planarityChecker);
                }
                else
                {
                    planarityChecker.lowPt[vw] = planarityChecker.height[w];
                }
                planarityChecker.nestingDepth[vw] = 2 * planarityChecker.lowPt[vw];
                if (planarityChecker.lowPt2[vw] < planarityChecker.height[v])
                {
                    planarityChecker.nestingDepth[vw] += 1;
                }
                if (e != null)
                {
                    if (planarityChecker.lowPt[vw] < planarityChecker.lowPt[e.Value])
                    {
                        planarityChecker.lowPt2[e.Value] = Math.Min(planarityChecker.lowPt[e.Value], planarityChecker.lowPt2[vw]);
                        planarityChecker.lowPt[e.Value] = planarityChecker.lowPt[vw];
                    }
                    else if (planarityChecker.lowPt[vw] > planarityChecker.lowPt[e.Value])
                    {

                        planarityChecker.lowPt2[e.Value] = Math.Min(planarityChecker.lowPt2[e.Value], planarityChecker.lowPt[vw]);
                    }
                    else
                    {

                        planarityChecker.lowPt2[e.Value] = Math.Min(planarityChecker.lowPt2[e.Value], planarityChecker.lowPt2[vw]);
                    }
                }
            }
        }


        private void GenerateOrderedAdjLists(PlanarityChecker pc)
        {
            pc.orderedAdjacentNodes = new Dictionary<Node, List<Node>>();
            var orderedAdjacent = new Dictionary<Node, List<Node>>();
            foreach (var n in this._nodes)
            {
                var adj = this.GetAdjacent(n).Where(x =>
                {

                    return pc.nestingDepth.ContainsKey((n, x)) && pc.orientedEdges.Contains((n, x));
                }).ToList();

                adj.Sort((e1, e2) =>
                        {

                            var nd1 = pc.nestingDepth[(n, e1)];
                            var nd2 = pc.nestingDepth[(n, e2)];
                            return nd1.CompareTo(nd2);
                        });

                pc.orderedAdjacentNodes[n] = adj;
            }

        }

        private bool DFSTesting(Node v, PlanarityChecker planarityChecker)
        {
            bool hasParent = planarityChecker.parentEdge.TryGetValue(v, out (Node, Node)? parentEdge);
            var orderedAdjacents = planarityChecker.orderedAdjacentNodes;

            foreach (var w in orderedAdjacents[v])
            {
                var ei = (v, w);
                planarityChecker.stackBottom[ei] = planarityChecker.TopOfStack();
                bool wHasParent = planarityChecker.parentEdge.TryGetValue(w, out (Node, Node)? wParentEdge);
                if (wParentEdge != null && ei == wParentEdge)
                {
                    if (!DFSTesting(w, planarityChecker))
                    {
                        return false;
                    }
                }
                else
                {
                    planarityChecker.lowPtEdge[ei] = ei;
                    planarityChecker.S.Add(new ConflictPair(null, new Interval(ei, ei)));

                }
                if (planarityChecker.lowPt[ei] < planarityChecker.height[v])
                {
                    var zeroIdx = orderedAdjacents[v][0];
                    if (w == zeroIdx)
                    {
                        planarityChecker.lowPtEdge[parentEdge.Value] = planarityChecker.lowPtEdge[ei];
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



        internal void DFSEmbedding(Node v, PlanarityChecker pc, PlanarEmbedding pe)
        {

            foreach (var w in pc.orderedAdjacentNodes[v])
            {
                var ei = (v, w);
                if (pc.parentEdge.ContainsKey(w) && pc.parentEdge[w] == ei)
                {
                    pe.AddHalfEdgeFirst(w, v);
                    pc.leftRef[v] = w;
                    pc.rightRef[v] = w;
                    DFSEmbedding(w, pc, pe);
                }
                else
                {
                    if (pc.side[ei] == 1)
                    {
                        pe.AddHalfEdgeCW(w, v, pc.rightRef[w]);
                    }
                    else
                    {
                        pe.AddHalfEdgeCCW(w, v, pc.leftRef[w]);
                        pc.leftRef[w] = v;
                    }
                }
            }
        }

    }

    internal enum Orientation
    {
        Forward,
        Backward,
        None
    }



    internal class HalfEdge
    {

    }

    /// <summary>
    /// Planar Embedding of a graph
    /// </summary>
    public class PlanarEmbedding
    {

        public Graph originalGraph;
        public List<(Node, Node)> edges;
        public Dictionary<(Node, Node), Node> cw;
        public Dictionary<(Node, Node), Node> ccw;
        public Dictionary<Node, Node> firstNeighbor;


        public PlanarEmbedding(Graph g)
        {
            originalGraph = g;
            edges = new List<(Node, Node)>();
            cw = new Dictionary<(Node, Node), Node>();
            ccw = new Dictionary<(Node, Node), Node>();
            firstNeighbor = new Dictionary<Node, Node>();
        }

        internal void AddHalfEdgeCW(Node first, Node end, Node? refN)
        {


            edges.Add((first, end));
            if (refN == null)
            {
                var edge = (first, end);
                cw[edge] = end;
                ccw[edge] = end;
                firstNeighbor[first] = end;
                return;

            }


            var cwRef = cw[(first, refN.Value)];
            cw[(first, refN.Value)] = end;
            cw[(first, end)] = cwRef;
            ccw[(first, cwRef)] = end;
            ccw[(first, end)] = refN.Value;


        }


        internal void AddHalfEdgeCCW(Node first, Node end, Node? refN)
        {

            if (refN == null)
            {
                var edge = (first, end);
                cw[edge] = end;
                ccw[edge] = end;
                firstNeighbor[first] = end;

            }
            else
            {
                var ccwRef = ccw[(first, refN.Value)];
                AddHalfEdgeCW(first, end, ccwRef);

                if (firstNeighbor.ContainsKey(first) && refN == firstNeighbor[first])
                {
                    firstNeighbor[first] = end;
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


            if (firstNeighbor.ContainsKey(start))
            {
                var r = firstNeighbor[start];
                AddHalfEdgeCCW(start, end, r);
            }
            else
            {
                AddHalfEdgeCCW(start, end, null);
            }

        }

        internal (Node, Node) NextFaceHalfEdge(Node n, Node m)
        {
            return (m, ccw[(m, n)]);
        }

        internal void TraverseFace(Node n, Node m, bool markHalfEdges = false)
        {

        }
    }

    /// <summary>
    /// Class that provides the majority of the algorithm implementation for checking
    /// planarity.
    /// </summary>
    internal class PlanarityChecker
    {

        internal Dictionary<Node, int> height;
        internal Dictionary<Node, (Node, Node)?> parentEdge;
        internal Dictionary<Node, List<Node>> orderedAdjacentNodes;
        internal Dictionary<(Node, Node), int> lowPt;
        internal Dictionary<(Node, Node), int> lowPt2;
        internal Dictionary<(Node, Node), int> side;
        internal Dictionary<(Node, Node), (Node, Node)> lowPtEdge;
        internal Dictionary<(Node, Node), (Node, Node)?> refx;
        internal List<Node> roots;
        internal Dictionary<(Node, Node), ConflictPair> stackBottom;
        internal List<ConflictPair> S;
        internal Dictionary<(Node, Node), int> nestingDepth;
        internal List<(Node, Node)> orientedEdges;
        internal Dictionary<Node, Node> rightRef;
        internal Dictionary<Node, Node> leftRef;

        internal PlanarityChecker()
        {
            height = new Dictionary<Node, int>();
            lowPtEdge = new Dictionary<(Node, Node), (Node, Node)>();
            parentEdge = new Dictionary<Node, (Node, Node)?>();
            lowPt = new Dictionary<(Node, Node), int>();
            lowPt2 = new Dictionary<(Node, Node), int>();
            side = new Dictionary<(Node, Node), int>();
            nestingDepth = new Dictionary<(Node, Node), int>();
            refx = new Dictionary<(Node, Node), (Node, Node)?>();
            roots = new List<Node>();
            orientedEdges = new List<(Node, Node)>();
            rightRef = new Dictionary<Node, Node>();
            leftRef = new Dictionary<Node, Node>();
            stackBottom = new Dictionary<(Node, Node), ConflictPair>();
            S = new List<ConflictPair>();

        }

        internal ConflictPair TopOfStack()
        {
            if (this.S.Count == 0) return null;
            else return this.S[this.S.Count - 1];
        }

        internal bool AddConstraints((Node, Node) ei, (Node, Node) e)
        {
            var P = new ConflictPair(null, null);
            while (true)
            {
                var Q = this.PopS();

                if (!Q.left.Empty())
                {
                    Q.Swap();
                }

                if (!Q.left.Empty())
                {
                    return false; //not planar
                }
                if (lowPt[Q.right.low.Value] > lowPt[e])
                {

                    if (P.right.Empty())
                    { // topmost interval 
                        P.right = Q.right.Copy();
                    }
                    else
                    {
                        refx[P.right.low.Value] = Q.right.high.Value;
                    }
                    P.right.low = Q.right.low;
                }
                else
                {
                    refx[Q.right.low.Value] = lowPtEdge[e];
                }
                var E = this.S[this.S.Count - 1];
                var F = stackBottom[ei];
                if (this.S[this.S.Count - 1].Equals(stackBottom[ei]))
                {
                    break;
                }
            }
            while (this.S[this.S.Count - 1].left.Conflicting(ei, this) ||
                this.S[this.S.Count - 1].right.Conflicting(ei, this))
            {
                var Q = PopS();
                if (Q.right.Conflicting(ei, this))
                {
                    Q.Swap();
                }
                if (Q.right.Conflicting(ei, this))
                {
                    return false;
                }

                refx[P.right.low.Value] = Q.right.high;
                if (Q.right.low != null)
                {
                    P.right.low = Q.right.low;
                }

                if (P.left.Empty())
                {
                    P.left = Q.left.Copy();
                }
                else
                {
                    refx[P.left.low.Value] = Q.left.high;
                }
                P.left.low = Q.left.low;


            }
            if (!(P.left.Empty() && P.right.Empty()))
            {
                this.S.Add(P);
            }
            return true;
        }

        internal void RemoveBackEdges(Graph g, (Node, Node) e)
        {
            var u = e.Item1;
            while (this.S.Count > 0 && this.TopOfStack().Lowest(this) == height[u])
            {
                var P = PopS();
                if (P.left.low != null)
                {
                    side[P.left.low.Value] = -1;
                }

            }
            if (this.S.Count > 0)
            {
                var P = PopS();
                while (P.left.high != null && P.left.high.Value.Item2 == u)
                {
                    P.left.high = refx[P.left.high.Value];
                }
                if (P.left.high == null && P.left.low != null)
                {
                    refx[P.left.low.Value] = P.right.low.Value;
                    side[P.left.low.Value] = -1;
                    P.left.low = null;
                }
                while (P.right.high != null && P.right.high.Value.Item2 == u)
                {
                    P.right.high = refx[P.right.high.Value];
                }
                if (P.right.high == null && P.right.low != null)
                {
                    refx[P.right.low.Value] = P.left.low.Value;
                    side[P.right.low.Value] = -1;
                    P.right.low = null;
                }
                S.Add(P);
            }
            if (lowPt[e] < height[u])
            {
                var hl = TopOfStack().left.high;
                var hr = TopOfStack().right.high;
                if (hl != null && (hr == null || lowPt[hl.Value] > lowPt[hr.Value]))
                {
                    refx[e] = hl.Value;
                }
                else
                {
                    refx[e] = hr.Value;
                }
            }
        }

        public int Sign((Node, Node) edge)
        {
            if (this.refx.ContainsKey(edge) && this.refx[edge] != null)
            {
                side[edge] *= Sign(refx[edge].Value);
                refx[edge] = null;
            }
            return side[edge];
        }



        private ConflictPair PopS()
        {
            var Q = this.S[S.Count - 1];
            this.S.RemoveAt(S.Count - 1);
            return Q;
        }
    }



    public class ConflictPair
    {
        internal Interval left;
        internal Interval right;

        public ConflictPair(Interval l, Interval r)
        {
            if (l == null) l = new Interval(null, null);
            left = l;

            if (r == null) r = new Interval(null, null);
            right = r;
        }

        internal void Swap()
        {
            var tmp = left;
            left = right;
            right = tmp;

        }

        internal int Lowest(PlanarityChecker pc)
        {
            if (left.Empty())
            {
                return pc.lowPt[right.low.Value];

            }
            if (right.Empty())
            {
                return pc.lowPt[left.low.Value];
            }
            return System.Math.Min(pc.lowPt[left.low.Value], pc.lowPt[right.low.Value]);
        }



        public override bool Equals(object obj)
        {
            if (obj is ConflictPair)
            {
                return Equals((ConflictPair)obj);
            }
            return false;
        }
        public bool Equals(ConflictPair other)
        {

            return left.Equals(other.left) && right.Equals(other.right);
        }


        public override int GetHashCode()
        {
            var h = (this.left.GetHashCode() ^ 38334421) + (this.right.GetHashCode() * 97);
            return h;
        }
    }

    public class Interval
    {
        internal (Node, Node)? low;
        internal (Node, Node)? high;

        public Interval((Node, Node)? low, (Node, Node)? high)
        {
            this.low = low;
            this.high = high;
        }

        internal bool Empty()
        {
            return this.low == null && this.high == null;
        }

        internal Interval Copy()
        {
            return new Interval(this.low, this.high); // TODO deep copy?
        }

        internal bool Conflicting((Node, Node) edge, PlanarityChecker pc)
        {
            return this.Empty() == false && pc.lowPt[high.Value] > pc.lowPt[edge];

        }

        public override bool Equals(object obj)
        {
            if (obj is Interval)
            {
                return Equals((Interval)obj);
            }
            return false;
        }
        public bool Equals(Interval other)
        {
            return low == other.low && high == other.high;
        }


        public override int GetHashCode()
        {
            var x = 38334421;
            if (low != null) x ^= low.GetHashCode();
            var y = 97;
            if (high != null) x *= this.high.GetHashCode();
            var h = (x + y);
            return h;
        }
    }

}