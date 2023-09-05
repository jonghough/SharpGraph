using System.Configuration.Assemblies;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class JohnsonTest
    {
        [Fact]
        public void JohnsonTest1()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(4);
            var g = GraphGenerator.CreateComplete(nodes);

            foreach (Edge e in g.GetEdges())
            {
                var a = g.AddComponent<EdgeWeight>(e);
                a.Weight = 2;
                var b = g.AddComponent<EdgeDirection>(e);
                b.Direction = Direction.Forwards;
            }

            var minPath = g.FindShortestPaths();

            Assert.Equal(12, minPath.Count);
        }

        [Fact]
        public void JohnsonTest2()
        {
            var g = GraphGenerator.GenerateCycle(7);

            var r = new Random();
            foreach (Edge e in g.GetEdges())
            {
                var a = g.AddComponent<EdgeWeight>(e);
                a.Weight = r.NextSingle();
                var b = g.AddComponent<EdgeDirection>(e);
                b.Direction = Direction.Both;
            }

            var minPath = g.FindShortestPaths();

            // 7*6 all possible ordered pairs
            Assert.Equal(7 * 6, minPath.Count);
        }

        [Fact]
        public void JohnsonExceptionTest1()
        {
            var g = GraphGenerator.CreateComplete(5);

            var r = new Random();
            // do not add direcitons
            foreach (Edge e in g.GetEdges())
            {
                var a = g.AddComponent<EdgeWeight>(e);
                a.Weight = r.NextSingle();
            }
            // exception because no direction
            Assert.Throws<Exception>(() => g.FindShortestPaths());
        }

        [Fact]
        public void JohnsonExceptionTest2()
        {
            var g = GraphGenerator.CreateComplete(5);

            var r = new Random();
            // do not add weights
            foreach (Edge e in g.GetEdges())
            {
                var b = g.AddComponent<EdgeDirection>(e);
                b.Direction = Direction.Both;
            }

            // exception because no weights
            Assert.Throws<Exception>(() => g.FindShortestPaths());
        }
    }
}
