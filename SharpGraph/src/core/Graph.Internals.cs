// <copyright file="Graph.Internals.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Graph
    {
        /// <summary>
        /// Gets the given component defined by the T parameter for the given node. If such a
        /// component does not exist, one will be created and attached to the node and then returned.
        /// </summary>
        /// <param name="n">node of graph.</param>
        /// <typeparam name="T">Type name of NodeComponent.</typeparam>
        /// <returns>The node component.</returns>
        public T GetComponent<T>(Node n)
            where T : NodeComponent, new()
        {
            if (!this.nodeComponents.ContainsKey(n))
            {
                return null;
            }

            foreach (var kvp in this.nodeComponents[n])
            {
                if (kvp.Key.Equals(typeof(T).FullName))
                {
                    return kvp.Value as T;
                }
            }

            return null;
        }

        public T AddComponent<T>(Node node)
            where T : NodeComponent, new()
        {
            T t = this.GetComponent<T>(node);
            if (t == null)
            {
                t = new T();
                t.Owner = node;
                string nm = typeof(T).FullName;
                this.nodeComponents[node].Add(nm, t);
                return t;
            }
            else
            {
                return t;
            }
        }

        public bool RemoveComponent<T>(Node node)
            where T : NodeComponent, new()
        {
            var kvp = this.GetComponentKVP<T>(node);
            if (kvp.Key.Equals(string.Empty))
            {
                return false;
            }
            else
            {
                this.nodeComponents[node].Remove(kvp.Key);
                return true;
            }
        }

        public T GetComponent<T>(Edge e)
            where T : EdgeComponent, new()
        {
            if (!this.edgeComponents.ContainsKey(e))
            {
                return null;
            }

            foreach (var kvp in this.edgeComponents[e])
            {
                if (kvp.Value is T)
                {
                    return kvp.Value as T;
                }
            }

            return null;
        }

        private KeyValuePair<string, NodeComponent> GetComponentKVP<T>(Node n)
            where T : NodeComponent, new()
        {
            if (!this.nodeComponents.ContainsKey(n))
            {
                return default(KeyValuePair<string, NodeComponent>);
            }

            foreach (var kvp in this.nodeComponents[n])
            {
                if (kvp.Value is T)
                {
                    return kvp;
                }
            }

            return default(KeyValuePair<string, NodeComponent>);
        }

        private T AddNodeComponent<T>(Node node)
            where T : NodeComponent, new()
        {
            return this.AddComponent<T>(node);
        }

        private T AddEdgeComponent<T>(Edge edge)
            where T : EdgeComponent, new()
        {
            return this.AddComponent<T>(edge);
        }

        public bool HasComponent<T>(Edge e)
            where T : EdgeComponent, new()
        {
            if (!this.edgeComponents.ContainsKey(e))
            {
                return false;
            }

            foreach (var kvp in this.edgeComponents[e])
            {
                if (kvp.Value is T)
                {
                    return true;
                }
            }

            return false;
        }

        public T AddComponent<T>(Edge e)
            where T : EdgeComponent, new()
        {
            T t = this.GetComponent<T>(e);
            if (t == null)
            {
                t = new T();
                string nm = typeof(T).FullName;
                this.edgeComponents[e].Add(nm, t);
                return t;
            }
            else
            {
                return t;
            }
        }

        public bool RemoveComponent<T>(Edge edge)
            where T : EdgeComponent, new()
        {
            var kvp = this.GetComponentKVP<T>(edge);
            if (kvp.Key.Equals(string.Empty))
            {
                return false;
            }
            else
            {
                this.edgeComponents[edge].Remove(kvp.Key);
                return true;
            }
        }

        private KeyValuePair<string, EdgeComponent> GetComponentKVP<T>(Edge e)
            where T : EdgeComponent, new()
        {
            if (!this.edgeComponents.ContainsKey(e))
            {
                return default(KeyValuePair<string, EdgeComponent>);
            }

            foreach (var kvp in this.edgeComponents[e])
            {
                if (kvp.Value is T)
                {
                    return kvp;
                }
            }

            return default(KeyValuePair<string, EdgeComponent>);
        }

        public T GetComponent<T>()
            where T : GraphComponent, new()
        {
            if (!this.graphComponents.ContainsKey(typeof(T).FullName))
            {
                return null;
            }

            return this.graphComponents[typeof(T).FullName] as T;
        }

        public T AddComponent<T>()
            where T : GraphComponent, new()
        {
            T t = this.GetComponent<T>();
            if (t == null)
            {
                t = new T();
                string nm = typeof(T).FullName;
                this.graphComponents[nm] = t;
                return t;
            }
            else
            {
                return t;
            }
        }

        public bool RemoveComponent<T>()
            where T : GraphComponent, new()
        {
            T t = this.GetComponent<T>();
            if (t == null)
            {
                return false;
            }
            else
            {
                this.graphComponents.Remove(typeof(T).FullName);
                return true;
            }
        }

        public Graph Copy()
        {
            var ns = this.GetNodes();
            var es = this.GetEdges();
            var g = new Graph(es, ns);
            foreach (var dic in this.nodeComponents)
            {
                foreach (var kvp in dic.Value)
                {
                    var v = kvp.Value;
                    Type t = v.GetType();
                    var m = typeof(Graph)
                        .GetMethod(
                            nameof(Graph.AddNodeComponent),
                            System.Reflection.BindingFlags.NonPublic
                                | System.Reflection.BindingFlags.Instance
                        )
                        .MakeGenericMethod(t);
                    object componentObject = m.Invoke(g, new object[] { dic.Key });

                    v.Copy(componentObject as NodeComponent);
                }
            }

            foreach (var dic in this.edgeComponents)
            {
                foreach (var kvp in dic.Value)
                {
                    var v = kvp.Value;
                    Type t = v.GetType();
                    object componentObject = typeof(Graph)
                        .GetMethod(
                            nameof(Graph.AddEdgeComponent),
                            System.Reflection.BindingFlags.NonPublic
                                | System.Reflection.BindingFlags.Instance
                        )
                        .MakeGenericMethod(t)
                        .Invoke(g, new object[] { dic.Key });

                    v.Copy(componentObject as EdgeComponent);
                }
            }

            return g;
        }
    }
}
