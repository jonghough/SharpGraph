// <copyright file="EdgeDirection.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;

namespace SharpGraph
{
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
