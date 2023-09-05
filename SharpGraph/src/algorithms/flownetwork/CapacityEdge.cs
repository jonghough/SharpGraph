// <copyright file="CapacityEdge.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;

    /// <summary>
    /// EdgeCapacity class extends the EdgeComponent class, and can be
    /// attached to each edge of a graph.
    /// The class' purpose is to hold information useful for performing network flow
    /// algorithms.
    /// </summary>
    public class EdgeCapacity : EdgeComponent
    {
        protected float weight;

        private float capacity;

        private float flow;

        private float reverseFlow;

        private Direction flowDirection = Direction.Forwards;

        public float Capacity
        {
            get { return this.capacity; }
            set { this.capacity = value; }
        }

        public float Flow
        {
            get { return this.flow; }
            set { this.flow = value; }
        }

        public float ReverseFlow
        {
            get { return this.reverseFlow; }
            set { this.reverseFlow = value; }
        }

        public Direction FlowDirection
        {
            get { return this.flowDirection; }
            set { this.flowDirection = value; }
        }

        public float GetResidualFlow()
        {
            if (this.flowDirection == Direction.Backwards)
            {
                return 0 + this.flow;
            }
            else
            {
                return this.capacity - this.flow;
            }
        }

        public override void Copy(EdgeComponent edgeComponent)
        {
            if (edgeComponent == null)
            {
                throw new Exception("Null edge component");
            }

            if (edgeComponent is EdgeCapacity)
            {
                EdgeCapacity ec = edgeComponent as EdgeCapacity;
                ec.weight = this.weight;
                ec.capacity = this.capacity;
                ec.flow = this.flow;
                ec.flowDirection = this.flowDirection;
                ec.reverseFlow = this.reverseFlow;
            }
            else
            {
                throw new Exception("Type is not correct");
            }
        }
    }
}
