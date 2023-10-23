// <copyright file="Node.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;

namespace SharpGraph
{
    public struct Node : IEquatable<Node>
    {
        private readonly string label;

        public Node(string label)
        {
            this.label = label;
            Init();
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

        private static void Init() { }
    }

    public abstract class NodeComponent
    {
        private Node owner;

        protected NodeComponent() { }

        public Node Owner
        {
            get => this.owner;
            internal set => this.owner = value;
        }

        public abstract void Copy(NodeComponent nodeComponent);
    }
}
