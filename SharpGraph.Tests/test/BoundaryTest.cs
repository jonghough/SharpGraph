// <copyright file="BoundaryTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharpGraph
{
    public class BoundaryTest
    {
        [Fact]
        public void NodeBoundaryTest1()
        {
            var g = GraphGenerator.CreateComplete(6);
            var nodes = g.GetNodes().ToList();
            var hs = new HashSet<Node>();
            hs.Add(nodes[0]);
            var bdry = g.NodeBoundary(hs);
            Assert.Equal(5, bdry.Count);
            Assert.DoesNotContain(nodes[0], bdry);

            hs.Add(nodes[1]);
            hs.Add(nodes[2]);
            bdry = g.NodeBoundary(hs);
            Assert.Equal(3, bdry.Count);
            Assert.DoesNotContain(nodes[0], bdry);
            Assert.DoesNotContain(nodes[1], bdry);
            Assert.DoesNotContain(nodes[2], bdry);
        }

        [Fact]
        public void EdgeBoundaryTest1()
        {
            var g = GraphGenerator.CreateComplete(6);
            var nodes = g.GetNodes().ToList();
            var hs = new HashSet<Node>();
            hs.Add(nodes[0]);
            var bdry = g.EdgeBoundary(hs);
            Assert.Equal(5, bdry.Count);

            hs.Add(nodes[1]);
            hs.Add(nodes[2]);
            hs.Add(nodes[3]);
            bdry = g.EdgeBoundary(hs);

            // all edges to nodes 4,5. So 8 edges.
            Assert.Equal(8, bdry.Count);
        }
    }
}
