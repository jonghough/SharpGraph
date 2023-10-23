// <copyright file="FlowMemory.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

namespace SharpGraph
{
    /// <summary>
    /// Data class for holding netowrk flow related data.
    /// </summary>
    internal class FlowMemory
    {
        internal FlowMemory() { }

        public bool Visited { get; set; }

        public Node? Previous { get; set; }

        public float Distance { get; set; }
    }
}
