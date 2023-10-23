// <copyright file="Edge.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace SharpGraph
{
    public struct Edge : IEquatable<Edge>
    {
        private readonly Node from;

        private readonly Node to;

        public Edge(Node from, Node to)
        {
            if (from == to)
            {
                throw new Exception("Edge nodes must be different.");
            }

            this.from = from;
            this.to = to;
        }

        public Edge(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                throw new Exception(
                    "When using the Edge's string argument constructor, the node label strings must not be null or empty."
                );
            }

            this.from = new Node(from);
            this.to = new Node(to);
        }

        public static bool operator ==(Edge lhs, Edge rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Edge lhs, Edge rhs)
        {
            return !(lhs == rhs);
        }

        public Node From()
        {
            return this.from;
        }

        public Node To()
        {
            return this.to;
        }

        public bool IsSame(Edge edge)
        {
            if (
                (this.from == edge.from && this.to == edge.to)
                || (this.from == edge.to && this.to == edge.from)
            )
            {
                return true;
            }

            return false;
        }

        public bool IsSame(Node n1, Node n2)
        {
            if ((this.from == n1 && this.to == n2) || (this.from == n2 && this.to == n1))
            {
                return true;
            }

            return false;
        }

        public HashSet<Node> Nodes()
        {
            var nodes = new HashSet<Node>();
            nodes.Add(this.from);
            nodes.Add(this.to);
            return nodes;
        }

        public Node GetOther(Node n)
        {
            if (this.from == n)
            {
                return this.to;
            }
            else if (this.to == n)
            {
                return this.from;
            }
            else
            {
                throw new Exception("Cannot get other node to a non-incident node edge pair.");
            }
        }

        public override string ToString()
        {
            return "(" + this.from + ", " + this.to + ")";
        }

        public bool Equals(Edge other)
        {
            return this.from == other.from && this.to == other.to;
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge)
            {
                return this.Equals((Edge)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            var h = (this.from.GetHashCode() ^ 38334421) + (this.to.GetHashCode() * 11);
            return h;
        }

        internal Edge SetFrom(Node from)
        {
            return new Edge(from, this.to);
        }

        internal Edge SetTo(Node to)
        {
            return new Edge(this.from, to);
        }
    }

    public abstract class EdgeComponent
    {
        private Edge owner;

        public Edge Owner
        {
            get => this.owner;
            internal set => this.owner = value;
        }

        public abstract void Copy(EdgeComponent edgeComponent);
    }
}
