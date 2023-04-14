using System.Configuration.Assemblies;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpGraph
{
    [TestFixture]
    public class JohnsonTest
    {
        [Test]
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


            Assert.AreEqual(12, minPath.Count);
        }


    }
}