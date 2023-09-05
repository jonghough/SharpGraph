// <copyright file="Node.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;

    public struct Node : IEquatable<Node>
    {
        private string label;

        public Node(string label)
        {
            this.label = label;
            this.Init();
        }

        public static bool operator ==(Node lhs, Node rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Node lhs, Node rhs)
        {
            return !(lhs == rhs);
        }

        public string GetLabel()
        {
            return this.label;
        }

        public override string ToString()
        {
            return " " + this.label + " ";
        }

        private void Init() { }

        public override int GetHashCode()
        {
            return this.label.GetHashCode();
        }

        public bool Equals(Node other)
        {
            return this.label.Equals(other.label);
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return this.Equals((Node)obj);
            }

            return false;
        }
    }

    public abstract class NodeComponent
    {
        private Node owner;

        public NodeComponent() { }

        public Node Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }

        public abstract void Copy(NodeComponent nodeComponent);
    }
}
