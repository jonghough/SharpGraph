// <copyright file="RouteMemory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class RouteMemory
    {
        public RouteMemory() { }

        public bool Visited { get; set; }

        public Node? Previous { get; set; }

        public float Distance { get; set; }
    }
}
