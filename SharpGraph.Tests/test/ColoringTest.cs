// <copyright file="ColoringTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license. See LICENSE file in the samples root for full license information.
// </copyright>

using System.Linq;
using Xunit;

namespace SharpGraph
{
    public class ColoringTest
    {
        [Fact]
        public void ColoringTest1()
        {
            var g = GraphGenerator.CreateComplete(5);
            var coloring = g.GreedyColorNodes(NodeOrdering.Random);
            var colors = coloring.Values.ToHashSet();
            Assert.Equal(5, colors.Count);
        }

        [Fact]
        public void ColoringTest2()
        {
            var g = GraphGenerator.GenerateCycle(5);
            var coloring = g.GreedyColorNodes(NodeOrdering.Random);
            var colors = coloring.Values.ToHashSet();

            Assert.Equal(3, colors.Count);
        }

        [Fact]
        public void ColoringTest3()
        {
            var g = GraphGenerator.GenerateCycle(6);
            var coloring = g.GreedyColorNodes(NodeOrdering.Random);
            var colors = coloring.Values.ToHashSet();

            Assert.Equal(2, colors.Count);
        }
    }
}
