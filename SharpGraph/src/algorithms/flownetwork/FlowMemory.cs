// <copyright file="FlowMemory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

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
