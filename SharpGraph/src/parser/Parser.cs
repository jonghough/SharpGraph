// <copyright file="Parser.cs" company="Jonathan Hough">
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
    public class Parser
    {
        private Lexer lexer;
        private Token currentToken;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            this.currentToken = this.lexer.GetNextToken();
        }

        private void Eat(TokenType tokenType)
        {
            if (this.currentToken.Type == tokenType)
            {
                this.currentToken = this.lexer.GetNextToken();
            }
            else
            {
                throw new UnexpectedTokenException(
                    $"Unexpected token: {this.currentToken.Type}, expected: {tokenType}"
                );
            }
        }

        public ASTNode Parse() => this.Expression();

        private ASTNode Expression()
        {
            return this.CommaExpression();
        }

        private ASTNode CommaExpression()
        {
            var node = this.ArrowExpression();

            while (this.currentToken.Type == TokenType.COMMA)
            {
                this.Eat(TokenType.COMMA);
                var rightNode = this.Expression();
                node = new CommaNode { Left = node, Right = rightNode };
            }

            return node;
        }

        private ASTNode ArrowExpression()
        {
            var node = this.CompleteExpression();
            if (this.currentToken.Type == TokenType.RARROW)
            {
                this.Eat(TokenType.RARROW);
                var rightNode = this.Expression();
                node = new ArrowNode { Left = node, Right = rightNode };
            }

            return node;
        }

        private ASTNode CompleteExpression()
        {
            var node = this.CycleExpression();

            while (this.currentToken.Type == TokenType.BANG)
            {
                this.Eat(TokenType.BANG);

                node = new CompleteNode(node);
                node = this.ExpressionChain(node);
            }

            return node;
        }

        private ASTNode ExpressionChain(ASTNode node)
        {
            while (true)
            {
                if (this.currentToken.Type == TokenType.BANG)
                {
                    this.Eat(TokenType.BANG);
                    node = new CompleteNode(node);
                }
                else if (this.currentToken.Type == TokenType.ASTERISK)
                {
                    this.Eat(TokenType.ASTERISK);
                    node = new CycleNode(node);
                }
                else
                {
                    break;
                }
            }

            return node;
        }

        private ASTNode CycleExpression()
        {
            var node = this.Term();

            while (this.currentToken.Type == TokenType.ASTERISK)
            {
                this.Eat(TokenType.ASTERISK);
                node = new CycleNode(node);
                node = this.ExpressionChain(node);
            }

            return node;
        }

        private ASTNode Term()
        {
            if (this.currentToken.Type == TokenType.RPAREN)
            {
                this.Eat(TokenType.RPAREN);
                var expr = this.Expression();
                this.Eat(TokenType.LPAREN);
                return expr;
            }
            // this happens for example in empty parentheses "()".
            // if we get to this point, we need an emptynode
            else if (this.currentToken.Type == TokenType.LPAREN)
            {
                return new EmptyNode();
            }
            else if (this.currentToken.Type == TokenType.NAME)
            {
                var nameNode = new NameNode(this.currentToken.Value);
                this.Eat(TokenType.NAME);
                return nameNode;
            }
            else if (this.currentToken.Type == TokenType.INTEGER)
            {
                var leftInt = new IntegerNode(int.Parse(this.currentToken.Value));
                this.Eat(TokenType.INTEGER);
                if (this.currentToken.Type == TokenType.DOTS)
                {
                    this.Eat(TokenType.DOTS);
                    var rightInt = new IntegerNode(int.Parse(this.currentToken.Value));
                    this.Eat(TokenType.INTEGER);
                    return new RangeNode(leftInt, rightInt);
                }

                return leftInt;
            }
            else if (this.currentToken.Type == TokenType.EOF)
            {
                return new EmptyNode();
            }

            throw new UnexpectedTokenException($"Unexpected token: {this.currentToken.Type}");
        }
    }

    [Serializable]
    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException() { }

        public UnexpectedTokenException(string message)
            : base(message) { }

        public UnexpectedTokenException(string message, Exception innerException)
            : base(message, innerException) { }

        protected UnexpectedTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    internal class IllegalParseException : Exception
    {
        public IllegalParseException() { }

        public IllegalParseException(string message)
            : base(message) { }

        public IllegalParseException(string message, Exception innerException)
            : base(message, innerException) { }

        protected IllegalParseException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
