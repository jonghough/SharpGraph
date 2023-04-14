using System; 

namespace SharpGraph
{

    /// <summary>
    /// EdgeCapacity class extends the EdgeComponent class, and can be
    /// attached to each edge of a graph.
    /// The class' purpose is to hold information useful for performing network flow 
    /// algorithms.
    /// </summary>
    public class EdgeCapacity : EdgeComponent
    {
        protected float _weight;

        private float _capacity;

        public float Capacity { get { return _capacity; } set { _capacity = value; } }

        private float _flow;

        public float Flow
        {
            get
            {
                return _flow;
            }
            set
            {
                _flow = value;
            }
        }

        private float _reverseFlow;

        public float ReverseFlow
        {
            get
            {
                return _reverseFlow;
            }
            set
            {
                _reverseFlow = value;
            }
        }

        private Direction _flowDirection = Direction.Forwards;
        public Direction FlowDirection
        {
            get { return _flowDirection; }
            set { _flowDirection = value; }
        }




        public float GetResidualFlow()
        {
            if (_flowDirection == Direction.Backwards)
                return 0 + _flow;
            else return (_capacity - _flow);
        }

          public override void Copy (EdgeComponent edgeComponent) {
            if (edgeComponent == null)
                throw new Exception ("Null edge component");
            if (edgeComponent is EdgeCapacity) {
                EdgeCapacity ec = edgeComponent as EdgeCapacity;
                ec._weight = _weight;
                ec._capacity = _capacity;
                ec._flow = _flow;
                ec._flowDirection = _flowDirection;
                ec._reverseFlow = _reverseFlow;
            } else throw new Exception ("Type is not correct");
        }
    }
}

