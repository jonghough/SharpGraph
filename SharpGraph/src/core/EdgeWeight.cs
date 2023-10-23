// <copyright file="EdgeWeight.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;

namespace SharpGraph
{
    /// <summary>
    /// An EdgeComponent type that is used internally by various algorithms that require
    /// weights on edges of the graph.
    ///
    /// Example usage:
    ///
    /// <code>
    /// EdgeComponent myComp = myGraph.AddComponent<EdgeComponent>(myEdge);
    /// float w = myComponent.Weight;
    /// </code>
    /// </summary>
    public class EdgeWeight : EdgeComponent
    {
        public float Weight { get; set; }

        public override void Copy(EdgeComponent edgeComponent)
        {
            if (edgeComponent == null)
            {
                throw new Exception("Null edge component");
            }

            if (edgeComponent is EdgeWeight)
            {
                (edgeComponent as EdgeWeight).Weight = this.Weight;
            }
            else
            {
                throw new Exception("Type is not correct");
            }
        }
    }
}
