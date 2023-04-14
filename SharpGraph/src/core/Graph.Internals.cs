using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {

        /// <summary>
        /// Gets the given component defined by the T parameter for the given node. If such a 
        /// component does not exist, one will be created and attached to the node and then returned.
        /// </summary>
        /// <param name="n">node of graph</param>
        /// <typeparam name="T">Type name of NodeComponent</typeparam>
        /// <returns>The node component</returns>
        public T GetComponent<T>(Node n) where T : NodeComponent, new()
        {
            if (!_nodeComponents.ContainsKey(n))
            {
                return null;
            }
            foreach (var kvp in _nodeComponents[n])
            {
                if (kvp.Key.Equals(typeof(T).FullName))
                {
                    return kvp.Value as T;
                }
            }
            return null;
        }

        private KeyValuePair<String, NodeComponent> GetComponentKVP<T>(Node n) where T : NodeComponent, new()
        {
            if (!_nodeComponents.ContainsKey(n))
            {
                return default(KeyValuePair<String, NodeComponent>);
            }
            foreach (var kvp in _nodeComponents[n])
            {
                if (kvp.Value is T)
                {
                    return kvp;
                }
            }
            return new KeyValuePair<string, NodeComponent>();
        }

        public T AddComponent<T>(Node node) where T : NodeComponent, new()
        {
            T t = GetComponent<T>(node);
            if (t == null)
            {
                t = new T();
                t.Owner = node;
                string nm = typeof(T).FullName;
                _nodeComponents[node].Add(nm, t);
                return t;
            }
            else
            {
                return t;
            }
        }

        private T AddNodeComponent<T>(Node node) where T : NodeComponent, new()
        {
            return AddComponent<T>(node);
        }
        private T AddEdgeComponent<T>(Edge edge) where T : EdgeComponent, new()
        {
            return AddComponent<T>(edge);
        }

        public bool RemoveComponent<T>(Node node) where T : NodeComponent, new()
        {
            var kvp = GetComponentKVP<T>(node);
            if (kvp.Key.Equals(""))
            {
                return false;
            }
            else
            {
                _nodeComponents[node].Remove(kvp.Key);
                return true;
            }
        }

        public T GetComponent<T>(Edge e) where T : EdgeComponent, new()
        {
            if (!_edgeComponents.ContainsKey(e))
            {
                return null;
            }
            foreach (var kvp in _edgeComponents[e])
            {
                if (kvp.Value is T)
                {
                    return kvp.Value as T;
                }
            }
            return null;
        }

        public bool HasComponent<T>(Edge e) where T : EdgeComponent, new()
        {
            if (!_edgeComponents.ContainsKey(e))
            {
                return false;
            }
            foreach (var kvp in _edgeComponents[e])
            {
                if (kvp.Value is T)
                {
                    return true;
                }
            }
            return false;
        }

        private KeyValuePair<String, EdgeComponent> GetComponentKVP<T>(Edge e) where T : EdgeComponent, new()
        {
            if (!_edgeComponents.ContainsKey(e))
            {
                return default(KeyValuePair<String, EdgeComponent>);
            }
            foreach (var kvp in _edgeComponents[e])
            {
                if (kvp.Value is T)
                {
                    return kvp;
                }
            }
            return new KeyValuePair<string, EdgeComponent>();
        }

        public T AddComponent<T>(Edge e) where T : EdgeComponent, new()
        {
            T t = GetComponent<T>(e);
            if (t == null)
            {
                t = new T();
                string nm = typeof(T).FullName;
                _edgeComponents[e].Add(nm, t);
                return t;
            }
            else
            {
                return t;
            }
        }

        public bool RemoveComponent<T>(Edge edge) where T : EdgeComponent, new()
        {
            var kvp = GetComponentKVP<T>(edge);
            if (kvp.Key.Equals(""))
            {
                return false;
            }
            else
            {
                _edgeComponents[edge].Remove(kvp.Key);
                return true;
            }
        }

        public T GetComponent<T>() where T : GraphComponent, new()
        {
            if (!_graphComponents.ContainsKey(typeof(T).FullName))
            {
                return null;
            }

            return _graphComponents[typeof(T).FullName] as T;
        }

        public T AddComponent<T>() where T : GraphComponent, new()
        {
            T t = GetComponent<T>();
            if (t == null)
            {
                t = new T();
                string nm = typeof(T).FullName;
                _graphComponents[nm] = t;
                return t;
            }
            else
            {
                return t;
            }
        }

        public bool RemoveComponent<T>() where T : GraphComponent, new()
        {
            T t = GetComponent<T>();
            if (t == null)
            {
                return false;
            }
            else
            {
                _graphComponents.Remove(typeof(T).FullName);
                return true;
            }
        }

        public Graph Copy()
        {
            var ns = GetNodes();
            var es = GetEdges();
            var g = new Graph(es, ns);
            foreach (var dic in _nodeComponents)
            {
                foreach (var kvp in dic.Value)
                {
                    var v = kvp.Value;
                    Type t = v.GetType();
                    var m = typeof(Graph).GetMethod(nameof(Graph.AddNodeComponent),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).MakeGenericMethod(t);
                    object componentObject = m.Invoke(g, new object[] { dic.Key });

                    v.Copy(componentObject as NodeComponent);
                }
            }

            foreach (var dic in _edgeComponents)
            {
                foreach (var kvp in dic.Value)
                {
                    var v = kvp.Value;
                    Type t = v.GetType();
                    object componentObject = typeof(Graph)
                        .GetMethod(nameof(Graph.AddEdgeComponent),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .MakeGenericMethod(t)
                        .Invoke(g, new object[] { dic.Key });

                    v.Copy(componentObject as EdgeComponent);
                }
            }
            return g;
        }
    }
}