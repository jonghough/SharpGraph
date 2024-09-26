// <copyright file="Compiler.cs" company="Jonathan Hough">
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
    public class Compiler
    {
        private ASTNode rootNode;

        public Compiler(ASTNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public Graph Compile()
        {
            return this.Compile(this.rootNode);
        }

        public Graph Compile(ASTNode node)
        {
            var current = node;
            if (current is ArrowNode)
            {
                var gl = this.Compile((current as ArrowNode).Left);
                var gr = this.Compile((current as ArrowNode).Right);
                var g = gl.MergeWith(gr);

                // cross
                var glNodes = gl.GetNodes();
                var grNodes = gr.GetNodes();
                foreach (var l in glNodes)
                {
                    foreach (var r in grNodes)
                    {
                        if (l == r)
                        {
                            continue; // NO LOOPS.
                        }

                        g.AddEdge(l, r);
                    }
                }

                return g;
            }
            else if (node is RangeNode)
            {
                var r = node as RangeNode;
                if (r.Left.Value >= r.Right.Value)
                {
                    throw new NodeRangeException(
                        $"Cannot compose a range as right side is not greater than left side ({r.Right.Value} is not greater than {r.Left.Value})"
                    );
                }

                var range = Enumerable
                    .Range(r.Left.Value, r.Right.Value - r.Left.Value)
                    .Select(i => new Node(string.Empty + i))
                    .ToHashSet();
                return new Graph(range);
            }
            else if (node is NameNode)
            {
                return new Graph(new Node((node as NameNode).Value));
            }
            else if (node is CommaNode)
            {
                var comma = node as CommaNode;
                var leftG = this.Compile(comma.Left);

                var rightG = this.Compile(comma.Right);
                return leftG.MergeWith(rightG);
            }
            else if (node is CycleNode)
            {
                var cycle = node as CycleNode;
                var operand = cycle.Operand;
                if (operand is IntegerNode)
                {
                    return GraphGenerator.GenerateCycle((uint)(operand as IntegerNode).Value);
                }
                else
                {
                    var g = this.Compile(operand);
                    var nodes = g.GetNodes();
                    return GraphGenerator.GenerateCycle(nodes);
                }
            }
            else if (node is CompleteNode)
            {
                var complete = node as CompleteNode;
                var operand = complete.Operand;
                if (operand is IntegerNode)
                {
                    return GraphGenerator.CreateComplete((uint)(operand as IntegerNode).Value);
                }
                else
                {
                    var g = this.Compile(operand);
                    var nodes = g.GetNodes();
                    return GraphGenerator.CreateComplete(nodes);
                }
            }
            else if (node is IntegerNode)
            {
                return new Graph(new Node($"{(node as IntegerNode).Value}"));
            }
            else if (node is EmptyNode)
            {
                return new Graph(); // empty graph
            }
            else
            {
                throw new UnexpectedNodeException(
                    $"Unexpected node type in parsing: {node.GetType()}"
                );
            }
        }

        public Graph Compile(List<ASTNode> nodes)
        {
            var g = new Graph();
            foreach (var node in nodes)
            {
                if (node is NameNode)
                {
                    if (nodes.Count > 1)
                    {
                        throw new Exception(
                            $"A list of connections containing a solitary node must have size 1. {node}"
                        );
                    }

                    return this.Compile(node);
                }
                else if (node is RangeNode)
                {
                    if (nodes.Count > 1)
                    {
                        throw new Exception(
                            "A list of connections containing a range must have size 1."
                        );
                    }

                    return this.Compile(node);
                }
                else if (node is ArrowNode)
                {
                    var arrow = node as ArrowNode;
                    g = g.MergeWith(this.Compile(arrow));
                }
                else if (node is CommaNode)
                {
                    var cycle = node as CommaNode;
                    g = g.MergeWith(this.Compile(cycle));
                }
                else if (node is CycleNode)
                {
                    var comma = node as CommaNode;
                    g = g.MergeWith(this.Compile(comma));
                }
                else if (node is CompleteNode)
                {
                    g = g.MergeWith(this.Compile(node));
                }
                else
                {
                    throw new Exception($"Unexpected node type in parsing: {node.GetType()}");
                }
            }

            return g;
        }
    }

    [Serializable]
    public class NodeRangeException : Exception
    {
        public NodeRangeException() { }

        public NodeRangeException(string message)
            : base(message) { }

        public NodeRangeException(string message, Exception innerException)
            : base(message, innerException) { }

        protected NodeRangeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UnexpectedNodeException : Exception
    {
        public UnexpectedNodeException() { }

        public UnexpectedNodeException(string message)
            : base(message) { }

        public UnexpectedNodeException(string message, Exception innerException)
            : base(message, innerException) { }

        protected UnexpectedNodeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
