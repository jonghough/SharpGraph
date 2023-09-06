// <copyright file="PathsTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license. See LICENSE file in the samples root for full license information.
// </copyright>

using System.Linq;
using Xunit;

namespace SharpGraph
{
    public class PathsTest
    {
        [Fact]
        public void PathsTest1()
        {
            var g = GraphGenerator.CreateComplete(5);
            var ns = g.GetNodes().ToList();
            var paths = g.FindSimplePaths(ns[0], ns[1]);

            Assert.Equal(16, paths.Count);
        }

        [Fact]
        public void PathsTest2()
        {
            var g = GraphGenerator.CreateComplete(5);
            var ns = g.GetNodes().ToList();
            var paths = g.FindSimplePaths(ns[0], ns[1], 1);

            Assert.Single(paths);

            paths = g.FindSimplePaths(ns[0], ns[1], 2);
            Assert.Equal(4, paths.Count);
        }

        [Fact]
        public void PathsTest3()
        {
            var g = GraphGenerator.GenerateBipartiteComplete(2, 2);
            var ns = g.GetNodes().ToList();
            var paths = g.FindSimplePaths(ns[0], ns[1]);

            Assert.Equal(2, paths.Count);
        }
    }
}
