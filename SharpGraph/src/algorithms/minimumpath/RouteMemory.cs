using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{

    public class RouteMemory
    {
        public bool Visited { get; set; }
        public Node? Previous { get; set; }

        public float Distance { get; set; }

        public RouteMemory()
        {

        }
    }

}