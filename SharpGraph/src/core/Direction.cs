// <copyright file="Direction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;

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
