using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public class ColoringTest {
        [Test]
        public void ColoringTest1 () {
            var g = GraphGenerator.CreateComplete (5);
            var coloring = g.GreedyColorNodes (NodeOrdering.Random);
            var colors = coloring.Values.ToHashSet ();
            Assert.AreEqual (5, colors.Count);
        }

        [Test]
        public void ColoringTest2 () {
            var g = GraphGenerator.GenerateCycle (5);
            var coloring = g.GreedyColorNodes (NodeOrdering.Random);
            var colors = coloring.Values.ToHashSet ();

            Assert.AreEqual (3, colors.Count);
        }

        [Test]
        public void ColoringTest3 () {
            var g = GraphGenerator.GenerateCycle (6);
            var coloring = g.GreedyColorNodes (NodeOrdering.Random);
            var colors = coloring.Values.ToHashSet ();

            Assert.AreEqual (2, colors.Count);
        }
    }
}