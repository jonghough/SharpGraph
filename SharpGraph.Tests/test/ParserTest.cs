// <copyright file="ParserTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using Xunit;

namespace SharpGraph
{
    public class ParserTest
    {
        [Theory]
        [InlineData("(a->b) -> (1..4)", 5, 7, 1, null)]
        [InlineData("((a->b)->(c->d))->(e->f)", 6, 15, 1, null)]
        [InlineData("(0..5)", 5, 0, 5, null)]
        [InlineData("0..7", 7, 0, 7, null)]
        [InlineData("0..7->7..9", 9, 14, 1, null)] // bipartite k7,2
        [InlineData("(0..5)->(xy3-z->rtu)", 7, 11, 1, null)]
        [InlineData("(a->v,b->c)->(d->e,e- > x)", 7, 19, 1, null)]
        [InlineData("s->t,t->s,(2..6)", 6, 10, 1, null)]
        [InlineData("(1..5)->(5..10)", 9, 20, 1, null)]
        [InlineData("(0..4)->(0..4)", 4, 12, 1, null)]
        [InlineData("(((a->b)->c)->d)->e", 5, 10, 1, null)]
        [InlineData("5!", 5, 10, 1, null)]
        [InlineData("5*", 5, 5, 1, null)]
        [InlineData("5*->5*", 5, 20, 1, null)] // this is the same as 5*, but back edges included
        [InlineData("5*->(5..10)*", 10, 35, 1, null)]
        [InlineData("(a->b,b->c)!->(a->d)", 4, 6, 1, null)]
        [InlineData("(0..4!!)", 4, 6, 1, null)]
        [InlineData("((0..4)*)", 4, 4, 1, null)]
        [InlineData("", 0, 0, 0, null)]
        [InlineData("()", 0, 0, 0, null)]
        [InlineData("(())", 0, 0, 0, null)]
        [InlineData("((()), (()),())", 0, 0, 0, null)]
        [InlineData("x1->x2!", 2, 1, 1, null)]
        [InlineData("(x1->x2)!", 2, 1, 1, null)]
        [InlineData("(100..105)*", 5, 5, 1, null)]
        [InlineData("(0..5)->a!", 6, 5, 1, null)]
        [InlineData("(0..5)!->a!", 6, 15, 1, null)]
        [InlineData("(0..5)!*->a!", 6, 10, 1, null)]
        [InlineData("a!", 1, 0, 1, null)]
        [InlineData("(Aaaa->Bbbb,Bbbb->Cccc)!!!!!", 3, 3, 1, null)]
        [InlineData("((2..4)!)*", 2, 2, 1, null)]
        [InlineData("10!->10..14!", 14, 91, 1, null)]
        [InlineData("10!,10..14!,a->b", 16, 52, 3, null)]
        [InlineData("0..3->3..6", 6, 9, 1, null)]
        [InlineData("(0..3)->(3..6)", 6, 9, 1, null)]
        [InlineData("0..3,3..6,6..9,9..12", 12, 0, 12, null)]
        [InlineData("10*->10..14*", 14, 54, 1, null)]
        [InlineData("A", 1, 0, 1, null)]
        [InlineData("100", 1, 0, 1, null)]
        [InlineData("1->2, 4->10", 4, 4, 1, null)]
        [InlineData("(1->2), 4->10", 4, 2, 2, null)]
        [InlineData(
            "5*,(0->5),(1->6),(2->7),(3->8),(4->9),(5->7),(7->9),(9->6),(6->8),(8->5)",
            10,
            15,
            1,
            null
        )] // petersen graph
        [InlineData("0*", 0, 0, 0, typeof(ArgumentOutOfRangeException))]
        [InlineData("0!", 0, 0, 0, typeof(ArgumentException))]
        [InlineData("-5!", 0, 0, 0, typeof(UnexpectedCharacterException))]
        [InlineData("A%->B", 0, 0, 0, typeof(UnexpectedCharacterException))]
        [InlineData("node1->node2_", 0, 0, 0, typeof(IllegalTokenException))]
        [InlineData(" ( ) ! ", 0, 0, 0, typeof(ArgumentException))]
        [InlineData("|!khff%&'", 0, 0, 0, typeof(UnexpectedCharacterException))] // line noise
        [InlineData("****", 0, 0, 0, typeof(UnexpectedTokenException))]
        [InlineData("->->", 0, 0, 0, typeof(UnexpectedTokenException))]
        [InlineData("1..1", 0, 0, 0, typeof(NodeRangeException))]
        [InlineData("2..0", 0, 0, 0, typeof(NodeRangeException))]
        [InlineData("(1..1)->(1..5)", 0, 0, 0, typeof(NodeRangeException))]
        public void TestParserWorks(
            string generatorString,
            int expectedNodeCount,
            int expectedEdgeCount,
            int expectedComponentCount,
            Type exceptionThrown
        )
        {
            if (exceptionThrown == null)
            {
                Lexer lexer = new Lexer(generatorString);
                Parser parser = new Parser(lexer);
                ASTNode ast = parser.Parse();
                Compiler compiler = new Compiler(ast);
                Graph g = compiler.Compile();
                Assert.Equal(expectedNodeCount, g.GetNodes().Count);
                Assert.Equal(expectedEdgeCount, g.GetEdges().Count);
                Assert.Equal(expectedComponentCount, g.GetConnectedComponents().Count);
            }
            else
            {
                Action runTest = () =>
                {
                    Lexer lexer = new Lexer(generatorString);
                    Parser parser = new Parser(lexer);
                    ASTNode ast = parser.Parse();
                    Compiler compiler = new Compiler(ast);
                    Graph g = compiler.Compile();
                };
                var ex = Record.Exception(runTest);
                Assert.NotNull(ex);
                Assert.Equal(exceptionThrown, ex.GetType());
            }
        }
    }
}
