// <copyright file="EdgeDirection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;

    // <summary>

    /// An EdgeComponent type that is used internally by various algorithms that require
    /// directions on edges of the graph, i.e. a directed graph.
    ///
    /// Example usage:
    ///
    /// <code>
    /// EdgeComponent myComp = myGraph.AddComponent.<EdgeDirection>(myEdge);
    /// float d = myComponent.direction;
    /// </code>
    /// </summary>
    public class EdgeDirection : EdgeComponent
    {
        public EdgeDirection() { }

        public Direction Direction { get; set; }

        public override void Copy(EdgeComponent edgeComponent)
        {
            if (edgeComponent == null)
            {
                throw new Exception("Null edge component");
            }

            if (edgeComponent is EdgeDirection)
            {
                (edgeComponent as EdgeDirection).Direction = this.Direction;
            }
            else
            {
                throw new Exception("Type is not correct");
            }
        }
    }
}
