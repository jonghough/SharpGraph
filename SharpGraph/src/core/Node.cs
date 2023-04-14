using System; 

namespace SharpGraph
{

    public abstract class NodeComponent
    {
        private Node _owner;
        public Node Owner { get { return _owner; } internal set { _owner = value; } }
        public NodeComponent()
        {

        }

        public abstract void Copy(NodeComponent nodeComponent);
    }

    public struct Node : IEquatable<Node>
    {

        private string _label;

        public Node(string label)
        {
            _label = label;
            init();
        }

        private void init()
        {

        }

        public string GetLabel()
        {
            return _label;
        }



        public override string ToString()
        {
            return " " + _label + " ";

        }

        public override int GetHashCode()
        {
            return this._label.GetHashCode();
        }

        public bool Equals(Node other)
        {
            return this._label.Equals(other._label);
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return Equals((Node)obj);
            }
            return false;
        }

        public static bool operator ==(Node lhs, Node rhs)
        {
             
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Node lhs, Node rhs)
        {
            return !(lhs == rhs);
        }
    }
}