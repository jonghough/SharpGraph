using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{

    public abstract class EdgeComponent
    {
        private Edge _owner;
        public Edge Owner { get { return _owner; } internal set { _owner = value; } }

        public abstract void Copy(EdgeComponent edgeComponent);
    }

    public struct Edge : IEquatable<Edge>
    {

        private Node _from,
        _to;

        public Edge(Node from, Node to)
        {
            if (from == to) throw new Exception("Edge nodes must be different.");
            _from = from;
            _to = to;
        }

        public Edge(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                throw new Exception("When using the Edge's string argument constructor, the node label strings must not be null or empty.");
            }
            _from = new Node(from);
            _to = new Node(to);
        }

        internal Edge SetFrom(Node from)
        {
            return new Edge(from, _to);
        }

        internal Edge SetTo(Node to)
        {
            return new Edge(_from, to);
        }

        public Node From()
        {
            return _from;
        }

        public Node To()
        {
            return _to;
        }

        public bool IsSame(Edge edge)
        {
            if ((_from == edge._from && _to == edge._to) ||
                (_from == edge._to && _to == edge._from))
            {
                return true;
            }
            return false;
        }

        public bool IsSame(Node n1, Node n2)
        {
            if ((_from == n1 && _to == n2) ||
                (_from == n2 && _to == n1))
            {
                return true;
            }
            return false;
        }

        public HashSet<Node> Nodes()
        {
            var nodes = new HashSet<Node>();
            nodes.Add(_from);
            nodes.Add(_to);
            return nodes;
        }

        public Node GetOther(Node n)
        {
            if (_from == n) return _to;
            else if (_to == n) return _from;
            else throw new Exception("Cannot get other node to a non-incident node edge pair.");
        }

        public override string ToString()
        {
            return "(" + _from + ", " + _to + ")";

        }

        public bool Equals(Edge other)
        {
            return _from == other._from && _to == other._to;
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge)
            {
                return Equals((Edge)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var h = (this._from.GetHashCode() ^ 38334421) + (this._to.GetHashCode() * 11);
            return h;
        }

        public static bool operator ==(Edge lhs, Edge rhs)
        {

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Edge lhs, Edge rhs)
        {
            return !(lhs == rhs);
        }
    }
}