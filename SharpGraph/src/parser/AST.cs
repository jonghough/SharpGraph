// <copyright file="AST.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SharpGraph
{
    // AST Node classes
    public abstract class ASTNode { }

    public class CommaNode : ASTNode
    {
        public ASTNode Left { get; set; }

        public ASTNode Right { get; set; }

        public override string ToString() =>
            $"Comma({string.Join(", ", this.Left)} , {string.Join(", ", this.Right)})";
    }

    public class EmptyNode : ASTNode { }

    public class ArrowNode : ASTNode
    {
        public ASTNode Left { get; set; }

        public ASTNode Right { get; set; }

        public override string ToString() =>
            $"Arrow({string.Join(", ", this.Left)} -> {string.Join(", ", this.Right)})";
    }

    public class NameNode : ASTNode
    {
        public string Value { get; }

        public NameNode(string value) => this.Value = value;

        public override string ToString() => $"Name({this.Value})";
    }

    public class IntegerNode : ASTNode
    {
        public int Value { get; }

        public IntegerNode(int value) => this.Value = value;

        public override string ToString() => $"Integer({this.Value})";
    }

    public class RangeNode : ASTNode
    {
        public IntegerNode Left { get; }

        public IntegerNode Right { get; }

        public RangeNode(IntegerNode left, IntegerNode right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override string ToString() => $"Range({this.Left}..{this.Right})";
    }

    public class CycleNode : ASTNode
    {
        public ASTNode Operand { get; }

        public CycleNode(ASTNode operand)
        {
            this.Operand = operand;
        }

        public override string ToString() => $"Cyle({this.Operand})";
    }

    public class CompleteNode : ASTNode
    {
        public ASTNode Operand { get; }

        public CompleteNode(ASTNode operand)
        {
            this.Operand = operand;
        }

        public override string ToString() => $"Complete({this.Operand})";
    }
}
