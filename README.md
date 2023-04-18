 
# SharpGraph
Graphs and algorithms

![unit test workflow](https://github.com/jonghough/SharpGraph/actions/workflows/unit-test.yaml/badge.svg?branch=master)
 
## Project
This is a .NET library containing an assortment of `Graph` algorithms.

#### Some currently implemented algorithms:


| Algorithm | Usage |
|---------------------------------|----------------------------------------------------|
|Dijkstra | for finding shortest path between two nodes on a connected weighted graph, with non-negative weights|
|A-Star | for finding shortest path between two nodes on a connected weighted graph, with non-negative weights - usually faster than Dijkstra|
|Cycle finding | find all cycles on a given graph. Find Hamiltonian cycles.|
|Bellman-Ford | for finding shortest path between two nodes on a connected, weighted graph with positive or negative weights, and no negative weight cycles|
|Spanning Tree | finds the minimum spanning tree for a connected, weighted graph|
|Connected subgraphs | find all maximally connected subgraphs of a graph|
|General DFS and BFS search algorithms | For graph searching |
|Find minimum cut of a graph | Necessary edges to keep the graph connected|
|Maximum Flow | Find max flow in a flow network |
|Planarity testing | Checks if a graph is planar |

 
## Example usage

We can create a graph by specifying the edges and labeling nodes as below.

```csharp
Edge eAB = new Edge ("A","B");
Edge eAC = new Edge ("A","C");
Edge eCD = new Edge ("C","D");
Edge eDE = new Edge ("D","E"); 
Edge eAF = new Edge ("A","F"); 
Edge eBF = new Edge ("B","F"); 
Edge eDF = new Edge ("D","F"); 

List<Edge> list = new List<Edge> ();
list.Add (eAB);
list.Add (eAC);
list.Add (eCD);
list.Add (eDE);
list.Add (eAF);
list.Add (eBF);
list.Add (eDF);

Graph g = new Graph (list); 
```

### create a complete graph (graph where all nodes are connected to each other)

```csharp
//first create  10 nodes
var nodes = NodeGenerator.GenerateNodes(10);

// this will create a complete graph on 10 nodes, so there are 45 edges.
var graph = GraphGenerator.Create(nodes);
```

### is the graph connected?

```csharp
//test whether the graph is connected, which means it is possible to move from any point to any other point.
var g = new Graph():
for(int i = 0; i < 10;i++){
    g.AddEdge(new Edge(""+i, ""+(i+1)));
}
g.IsConnected(); //true
```
As another exampler, we can generate a complete graph on 10 nodes and check if it is connected. We can also
find biconnected components of the graph.
```csharp
var g = GraphGenerator.CreateComplete(NodeGenerator.GenerateNodes(10)); // generate complete graph
g.IsConnected(); //true
//find biconnected components
g.FindBiconnectedComponents (); // complete graph so only 1 biconnected component, the graph itself.
```

### find minimum spanning tree of a connected weighted graph

```csharp
Node b1 = new Node ("1"); 
Node b2 = new Node ("2"); 
Node b3 = new Node ("3"); 

Edge wedge1 = new Edge (b1, b2);
Edge wedge2 = new Edge (b1, b3);
Edge wedge3 = new Edge (b2, b3);
var edges = new List<Edge> ();
edges.Add (wedge1);
edges.Add (wedge2);
edges.Add (wedge3);

var g = new Graph (edges);

g.AddComponent<EdgeWeight> (wedge1).Weight = -5.1f;
g.AddComponent<EdgeWeight> (wedge2).Weight = 1.0f;
g.AddComponent<EdgeWeight> (wedge3).Weight = 3.5f;
var span = g.GenerateMinimumSpanningTree ();

```

### find ALL cycles in a graph (this algorithm is very slow, don't use for big graphs)

#### example
![cycles](/graph_cycles.png)

In the above graph there are 3 basic cycles. This is easily calculated. For more complicated graphs the
calculation can be quite complex. For example, for the complete graph on 8 nodes, there are 8011 cycles to find.

```csharp
HashSet<Node> nodes7 = NodeGenerator.GenerateNodes(7);
var graph7 = GraphGenerator.CreateComplete(nodes7); 
var cycles = graph7.FindAllCyclesInGraph();
// number of cycles should be 1172
```

### find shortest path between two nodes on a *weighted graph*

We can find the minimum distance path between any two nodes on a connected graph using `Dijkstra's Algorithm`. 

```csharp 
Edge eAB = new Edge ("A", "B");
Edge eAG = new Edge ("A", "G");
Edge eCD = new Edge ("C", "D");
Edge eDH = new Edge ("D", "H");
Edge eAF = new Edge ("A", "F");
Edge eBF = new Edge ("B", "F");
Edge eDF = new Edge ("D", "F");
Edge eCG = new Edge ("C", "G");
Edge eBC = new Edge ("B", "C");
Edge eCE = new Edge ("C", "E");

List<Edge> list = new List<Edge> ();
list.Add (eAB);
list.Add (eAG);
list.Add (eCD);
list.Add (eDH);
list.Add (eAF);
list.Add (eBF);
list.Add (eDF);
list.Add (eCG);
list.Add (eBC);
list.Add (eCE);

Graph g = new Graph (list);

g.AddComponent<EdgeWeight> (eAB).Weight = 10;
g.AddComponent<EdgeWeight> (eAG).Weight = 14;
g.AddComponent<EdgeWeight> (eCD).Weight = 5;
g.AddComponent<EdgeWeight> (eDH).Weight = 5.5f;
g.AddComponent<EdgeWeight> (eAF).Weight = 12;
g.AddComponent<EdgeWeight> (eBF).Weight = 3.5f;
g.AddComponent<EdgeWeight> (eDF).Weight = 1.5f;
g.AddComponent<EdgeWeight> (eCG).Weight = 11.5f;
g.AddComponent<EdgeWeight> (eBC).Weight = 6.5f;
g.AddComponent<EdgeWeight> (eCE).Weight = 6.5f;

List<Node> path = g.FindMinPath (b1, b8);

```

### find shortest path between two nodes on a *weighted graph* with A-Star

For a faster algorithm, we can use the `A-star algorithm` to find the minimum path on the same graph as above.

```csharp 
List<Node> path = g.FindMinPath (b1, b6, new TestHeuristic (5));

```
 
## Design

There are four main objects:

### Graph 
an object which contains collections of *edges* and *nodes*. Most algorithms and functionality are Graph methods in this design. Graphs can be copied and merged.

### Edge 
contains pairs of *Nodes*. Can be associated with any number of *EdgeComponent*s. Comparison of Edges is based on whether their nodes are identical.
### Node
In a given graph, a node (or *vertex*) must have a unique name, a label, that defines it.  

### Components
Edges and Nodes share a similar *Component* architecture, where a given edge may have attached to it multiple `EdgeComponent` subclass instances. Similarly, Nodes can have multiple
`NodeComponent` subclass instances attached to them.

In  this way, we can define weight edges as a new component `MyEdgeWeight`, which can contain a `float` value (the weight), and attach this component to each edge of our graph. This is simpler than having a subclass of `Edge`, `WeightedEdge` and having  web of generics in the library. It is also very flexible and allows pretty much any type of `Node` or `Edge` type without creating subclasses. Just define our component and attach it to each edge / node. 

For example, a *Directed Graph* is just a graph with a *DirectionComponent* attached to each edge in the graph. 

## testing

In the base directory
```
dotnet test ./
```


## building
see [dotnet link](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build)
```
dotnet build --configuration Release
```