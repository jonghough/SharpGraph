# SharpGraph.Builder

Builder library for `SharpGraphLib` SharpGraph library. This allwos the use of a DSL to quickly and easily construct graphs.

## Building Graphs

### GraphGenerator

The `GraphGenerator` static class contians multiple methods for building common graphs.
Example

```csharp
// generate a barbell graph
var g1 = GraphGenerator.GenerateBarbell(10,10);
// generate complete graph on 5 vertices
var g2 = GraphGenerator.CreateComplete(5);
```

### DSL Syntax

There is a second method to generate graphs, which is to use the inbuilt simple DSL. The syntax is terse, to allow easy building of graphs.
An example which builds the complete graph on 5 vertices, with nodes labelled 0 to 4. The `!` character builds a complete graph of everything to the left of it.

```
var g = GraphCompiler.Build("5!")
```

Syntax

| term | meaning                                                                                    | example  |
| ---- | ------------------------------------------------------------------------------------------ | -------- |
| 'a'    | named node, will produce a grpah of single node, labeled "a"                               | 'a'        |
| 1..4 | builds a graph of nodes labeled 1,2,3, with no edges                                       | 3..5
| !    | build complete graph                                                                       | 4!       |
| \*   | build cyclic graph                                                                         | (1..6)\* |
| ->   | builds edges by connecting all nodes on the left graph, with all nodes on the right graph. |
| ,    | combines left graph and right graph with no edge connections                               |

Below we have some examples of using the DSL to easily create graphs.

#### Complete graph

There are multiple ways to generate a complete graph.
Below is the simplest, with nodes labeled 0 to 4 in this case.

```
GraphCompiler.Build("5!")
```

We could also use `->` syntax, as below, where we generate the complete graph on 5 vertices with labels "A" to "E".

```
GraphCompiler.Build("(((A->B)->C)->D)->E")
```

Note the excessive parentheses. We don't actually need them and could use

```
GraphCompiler.Build("A->B->C->D->E")
```

To generate a complete graph, we could also use the range syntax, as in the example below.
`10..20` generates a graph of 10 nodes labeled from 10 to 19, with no edges. Then `!` builds a complete graph
of this to generate the complete graph on 10 vertices, with labels from 10 to 19.

```
GraphCompiler.Build("10..20!")
```

### Complete Bipartite Graph

Similar to the complete graph, we could build complete bipartite graphs.
The right arrow `->` connects all nodes on the left side of it to all nodes on the right.
So, for example

```
GraphCompiler.Build("0..3->3..6")
```

will create an edgeless graph with nodes 0,1,2, with `0..3`, and similarly with `3..6`, we get an edgeless graph with nodes 3,4,5. Then the `->` right arrow connects all nodes on the left, with all nodes on the right to get a bipartite complete grpah "k(3,3)".

If we wanted non-integer labels on our nodes we could do

```
GraphCompiler.Build("(Node1, Node2, Node3)->(Node4,Node5,Node6)")
```

### Cyclic graphs

We have a primitive operation to generate cyclic graphs, `*`. This works muc the same as `!` for the generation of complete graphs.
Note that in terms of operator precedence `!` is higher than `*`.

Example. The below example will create the cyclic graph "C(5)".

```
GraphCompiler.Build("5*")
```

We can also do the same with

```
GraphCompiler.Build("0..4*")
```

### Paths

To create paths we can combine `,` and `->` as below. THe below example creates a path `A0-A1-A2-A3-A4`.

```
GraphCompiler.Build("A0->A1,A1->A2,A2->A3,A3->A4");
```

### More complex constructions

As an example, we can construct a Petersen Graph using the DSL syntax.

```
"5*,0->5,1->6,2->7,3->8,4->9,5->7,7->9,9->6,6->8,8->5"
```

## Node naming

Creating node names (labels) with the DSL requires following a few rules. Node labels can be stringified integers, have we have seen. Or can be given names, e.g. "Node1". Naming rules are that the first character must be an alphabet character (`[a-zA-Z]`), subsequent characters can be any alphanumeric character, or the characters `_` and `-`. The name must end with an alphanumeric character.

The following are allowed
"a", "A", "This_is_a_NoDe1", "x--2--4".

The following are forbidden
"\_a", "-A", "M--", "ThiS-Is-a-nOdE10-", "A()!+B"