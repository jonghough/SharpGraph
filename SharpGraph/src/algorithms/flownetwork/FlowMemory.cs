using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{

    /// <summary>
    /// Data class for holding netowrk flow related data.
    /// </summary>
    internal class FlowMemory
    {
        public bool Visited { get; set; }
        public Node? Previous { get; set; }

        public float Distance { get; set; }

        internal FlowMemory()
        {

        }
    }

}