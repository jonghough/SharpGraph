// <copyright file="GraphGenerationTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace SharpGraph
{
    public class GraphGeneratorTest
    {
        [Fact]
        public void TestGeneratingTuranGraph1()
        {
            var g = GraphGenerator.GenerateTuranGraph(5, 2);
            Assert.True(g.GetNodes().Count == 5);
            Assert.True(g.GetEdges().Count == 8);
            Assert.True(g.GetEdge("1", "2") != null);
            Assert.True(g.GetEdge("1", "0") == null);
        }

        [Fact]
        public void TestGeneratingTuranGraph2()
        {
            var g = GraphGenerator.GenerateTuranGraph(10, 3);
            Assert.True(g.GetNodes().Count == 10);
            Assert.True(g.GetEdges().Count == 36);
            Assert.True(g.GetEdge("1", "2") == null);
            Assert.True(g.GetEdge("3", "4") == null);
            Assert.True(g.GetEdge("1", "8") != null);
        }

        [Fact]
        public void TestGeneratePath()
        {
            var g = GraphGenerator.GeneratePath(5, "N");
            Assert.True(g.GetNodes().Count == 5);
            foreach (var n in g.GetNodes())
            {
                if (n.GetLabel() == "N0" || n.GetLabel() == "N4")
                {
                    Assert.True(g.GetAdjacent(n, false).Count == 1);
                }
                else
                {
                    Assert.True(g.GetAdjacent(n, false).Count == 2);
                }
            }
        }

        [Theory]
        [InlineData(4, 8, 10)]
        [InlineData(5, 10, 13)]
        [InlineData(20, 40, 58)]
        public void TestGenerateLadder(
            uint nodeCountPerSide,
            int expectedNodeCount,
            int expectedEdgeCount
        )
        {
            var g = GraphGenerator.GenerateLadderGraph(nodeCountPerSide);
            Assert.True(g.GetNodes().Count == expectedNodeCount);
            Assert.True(g.GetEdges().Count == expectedEdgeCount);
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void TestGeneratePetersenGraph()
        {
            var g = GraphGenerator.GeneratePetersenGraph();
            Assert.True(g.GetNodes().Count == 10);
            Assert.True(g.GetEdges().Count == 15);
            Assert.True(g.IsConnected());
        }

        [Theory]
        [InlineData(4, false, 5, 8)]
        [InlineData(5, false, 6, 10)]
        [InlineData(10, false, 11, 20)]
        [InlineData(2, true, 0, 0)]
        public void TestGenerateWheelGraph(
            int spokesCount,
            bool throwsException,
            int expectedNodeCount,
            int expectedEdgeCount
        )
        {
            if (throwsException)
            {
                Assert.Throws<Exception>(() => GraphGenerator.GenerateWheelGraph(spokesCount));
            }
            else
            {
                var g = GraphGenerator.GenerateWheelGraph(spokesCount);
                Assert.Equal(g.GetNodes().Count, expectedNodeCount);
                Assert.Equal(g.GetEdges().Count, expectedEdgeCount);
                Assert.True(g.IsConnected());
            }
        }

        [Theory]
        [InlineData(4, 4, 16, 24)]
        [InlineData(1, 1, 1, 0)]
        [InlineData(12, 14, 12 * 14, (12 * 13) + (14 * 11))]
        [InlineData(2, 16, 32, (2 * 15) + (16 * 1))]
        public void TestGenerateGrid(
            int width,
            int height,
            int expectedNodeCount,
            int expectedEdgeCount
        )
        {
            var g = GraphGenerator.GenerateGrid(width, height);
            Assert.Equal(g.GetNodes().Count, expectedNodeCount);
            Assert.Equal(g.GetEdges().Count, expectedEdgeCount);
            Assert.True(g.IsConnected());
        }

        [Theory]
        [InlineData(4, 4, 4, 64, (24 * 4) + (3 * 16))]
        [InlineData(3, 1, 6, 18, (2 * 6) + (5 * 3))]
        [InlineData(1, 1, 1, 1, 0)]
        public void TestGenerate3DGrid(
            int width,
            int height,
            int depth,
            int expectedNodeCount,
            int expectedEdgeCount
        )
        {
            var g = GraphGenerator.Generate3DGrid(width, height, depth);
            Assert.Equal(g.GetNodes().Count, expectedNodeCount);
            Assert.Equal(g.GetEdges().Count, expectedEdgeCount);
            Assert.True(g.IsConnected());
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 0, 1)]
        [InlineData(1, 1, 0)]
        [InlineData(-1, -6, 1)]
        public void TestGenerate3DGridFailure(int width, int height, int depth)
        {
            Assert.Throws<NonPositiveArgumentException>(
                () => GraphGenerator.Generate3DGrid(width, height, depth)
            );
        }

        [Theory]
        [InlineData(1, 1, 2, 1)]
        [InlineData(4, 3, 7, 10)]
        public void TestGenerateBarbell(
            int leftSide,
            int rightSide,
            int expectedNodeCount,
            int expectedEdgeCount
        )
        {
            var g = GraphGenerator.GenerateBarbell(leftSide, rightSide);
            Assert.Equal(g.GetNodes().Count, expectedNodeCount);
            Assert.Equal(g.GetEdges().Count, expectedEdgeCount);
            Assert.True(g.IsConnected());
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(6, 6, 15)]
        [InlineData(3, 3, 3)]
        [InlineData(9, 9, 36)]
        public void TestGenerateComplete(int size, int expectedNodeCount, int expectedEdgeCount)
        {
            var g = GraphGenerator.CreateComplete((uint)size);
            Assert.Equal(g.GetNodes().Count, expectedNodeCount);
            Assert.Equal(g.GetEdges().Count, expectedEdgeCount);
            Assert.True(g.IsConnected());
        }
    }
}
