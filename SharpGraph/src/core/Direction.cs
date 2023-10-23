// <copyright file="Direction.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

namespace SharpGraph
{
    /// <summary>
    /// Edge Direction enum. Given an edge with an attached. <code>EdgeDirection</code>
    /// component, this enum defines the direction of the edge.
    ///
    /// Values:
    /// <list type="bullet">
    /// <item><b>Forwards</b>: Direction of edge is from the <b>From</b> edge to the <b>To</b> edge.</item>
    /// <item><b>Backwards</b>: Direction of edge is from the <b>To</b> edge to the <b>From</b> edge.</item>
    /// <item><b>Both</b>: Direction of edge goes both ways. i.e. a bidirectional edge.</item>
    /// </list>
    /// </summary>
    public enum Direction
    {
        // Forwards direction
        Forwards,

        // Backwards direction
        Backwards,

        // Both directions
        Both,
    }
}
