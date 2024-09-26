// <copyright file="Lexer.cs" company="Jonathan Hough">
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
    public enum TokenType
    {
        DOTS,
        RARROW,
        NAME,
        RPAREN,
        LPAREN,
        COMMA,
        INTEGER,
        ASTERISK,
        BANG,
        EOF,
    }

    public class Token
    {
        public TokenType Type { get; }

        public string Value { get; }

        public Token(TokenType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }
    }

    public class Lexer
    {
        private string input;
        private int position;

        public Lexer(string input)
        {
            this.input = input;
            this.position = 0;
        }

        private char CurrentChar =>
            this.position < this.input.Length ? this.input[this.position] : '\0';

        private void Advance()
        {
            this.position++;
        }

        public bool HasNextToken => this.CurrentChar != '\0';

        public Token GetNextToken()
        {
            while (this.CurrentChar != '\0')
            {
                if (char.IsWhiteSpace(this.CurrentChar))
                {
                    this.SkipWhitespace();
                    continue;
                }

                if (this.CurrentChar == '.' && this.PeekNextNonWhitespace() == '.')
                {
                    this.Advance();
                    this.SkipWhitespace();
                    this.Advance();
                    return new Token(TokenType.DOTS, "..");
                }

                if (this.CurrentChar == '-' && this.PeekNextNonWhitespace() == '>')
                {
                    this.Advance();
                    this.SkipWhitespace();
                    this.Advance();
                    return new Token(TokenType.RARROW, "->");
                }

                if (this.CurrentChar == '(')
                {
                    this.Advance();
                    return new Token(TokenType.RPAREN, "(");
                }

                if (this.CurrentChar == ')')
                {
                    this.Advance();
                    return new Token(TokenType.LPAREN, ")");
                }

                if (this.CurrentChar == ',')
                {
                    this.Advance();
                    return new Token(TokenType.COMMA, ",");
                }

                if (this.CurrentChar == '!')
                {
                    this.Advance();
                    return new Token(TokenType.BANG, "!");
                }

                if (this.CurrentChar == '*')
                {
                    this.Advance();
                    return new Token(TokenType.ASTERISK, "!");
                }

                if (char.IsLetter(this.CurrentChar))
                {
                    return new Token(TokenType.NAME, this.ReadName());
                }

                if (char.IsDigit(this.CurrentChar))
                {
                    return new Token(TokenType.INTEGER, this.ReadInteger());
                }

                throw new UnexpectedCharacterException($"Unexpected character: {this.CurrentChar}");
            }

            return new Token(TokenType.EOF, null);
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(this.CurrentChar))
            {
                this.Advance();
            }
        }

        private char PeekNextNonWhitespace()
        {
            int peekPos = this.position + 1;
            while (peekPos < this.input.Length && char.IsWhiteSpace(this.input[peekPos]))
            {
                peekPos++;
            }

            return peekPos < this.input.Length ? this.input[peekPos] : '\0';
        }

        private string ReadName()
        {
            int start = this.position;
            while (
                char.IsLetterOrDigit(this.CurrentChar)
                || this.CurrentChar == '_'
                || this.CurrentChar == '-'
            )
            {
                if (this.CurrentChar == '-' && this.PeekNextNonWhitespace() == '>')
                {
                    break;
                }

                this.Advance();
            }

            string name = this.input.Substring(start, this.position - start);
            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9]([a-zA-Z0-9_-]*[a-zA-Z0-9])*$"))
            {
                throw new IllegalTokenException($"Invalid NAME token: {name}");
            }

            return name;
        }

        private string ReadInteger()
        {
            int start = this.position;
            while (char.IsDigit(this.CurrentChar))
            {
                this.Advance();
            }

            return this.input.Substring(start, this.position - start);
        }
    }

    [Serializable]
    public class IllegalTokenException : Exception
    {
        public IllegalTokenException() { }

        public IllegalTokenException(string message)
            : base(message) { }

        public IllegalTokenException(string message, Exception innerException)
            : base(message, innerException) { }

        protected IllegalTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UnexpectedCharacterException : Exception
    {
        public UnexpectedCharacterException() { }

        public UnexpectedCharacterException(string message)
            : base(message) { }

        public UnexpectedCharacterException(string message, Exception innerException)
            : base(message, innerException) { }

        protected UnexpectedCharacterException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
