namespace SharpGraph.Builder

///<summary>
/// Compiler module to compiel expressions representing graphs into
/// graph objects.
///</summary>
module Compiler =
    open SharpGraph
    open Microsoft.FSharp.Collections
    open Parser
    open FParsec

    ///
    ///
    ///
    let rec processPairs (list: List<Node>) (graph: Graph) =
        list
        |> List.mapi (fun i item ->
            list
            |> List.iteri (fun j otherItem ->
                if j > i then
                    graph.AddEdge(list.[i], list.[j])))


    ///
    ///
    ///
    and GeneratePath (g: Graph) =
        let n1 = new System.Collections.Generic.List<Node>(g.GetNodes())
        n1 |> Seq.pairwise |> Seq.iter (fun (a, b) -> g.AddEdge(a, b)) |> ignore
        g

    ///
    ///
    ///
    and GenerateCycle (g: Graph) =
        let n1 = new System.Collections.Generic.List<Node>(g.GetNodes())

        n1
        |> Seq.pairwise
        |> Seq.iter (fun (a, b) ->
            if a <> b then
                g.AddEdge(a, b))

        g.AddEdge(n1[0], n1[n1.Count - 1])
        g

    ///
    ///
    ///
    and GenerateComplete (g: Graph) =
        let n1 = g.GetNodes()
        let nodeList = new System.Collections.Generic.List<Node>(n1)
        nodeList |> List.ofSeq |> (fun l -> processPairs l g |> ignore)
        g

    ///
    ///
    ///
    and CreateEdges (g1: Graph) (g2: Graph) =
        let n1 = g1.GetNodes()
        let n2 = g2.GetNodes()
        let g = g1.MergeWith(g2)

        n1
        |> Seq.iter (fun n ->
            n2
            |> Seq.iter (fun m ->
                if n <> m then
                    g.AddEdge(n, m)))

        g

    ///
    ///
    ///
    and BinaryOperatorExp (op: BinaryOperator) (e1: Expression) (e2: Expression) =
        match op with
        | Aggregate ->
            let g1: Graph = HandleExpression e1
            let g2: Graph = HandleExpression e2
            g1.MergeWith(g2)
        | Edge -> CreateEdges (HandleExpression(e1)) (HandleExpression(e2))
        | BipartiteComplete -> CreateEdges (HandleExpression(e1)) (HandleExpression(e2))

    ///
    ///
    ///
    and HandleUnaryExp exp op =
        match op with
        | Complete -> GenerateComplete(HandleExpression(exp))
        | Cycle -> GenerateCycle(HandleExpression(exp))
        | Path -> GeneratePath(HandleExpression(exp))

    ///
    ///
    ///
    and HandleNodeIdentifier nid =
        match nid with
        | NamedNode(namedId) ->
            match namedId with
            | IdentifierName(name) -> new Graph([| name |])
        | NumberedNode n -> new Graph([| string (n) |])

    ///
    ///
    ///
    and HandleRangeExpression (r: Range) : Graph =
        match r with
        | Range(first, last) ->
            let ints = seq {for i in first .. (last-1) -> i}
            let nodes = ints |> Seq.map ( fun i -> new Node(string(i)) )  
            new Graph(Array.ofSeq nodes)

    ///
    ///
    ///
    and HandleExpression exp =
        match exp with
        | NodeIdentifierExp(nodeIdentifier) -> HandleNodeIdentifier nodeIdentifier
        | RangeExp(range) -> HandleRangeExpression range
        | UnaryExp(e, op) -> HandleUnaryExp e op
        | BinaryOperatorExp(op, e1, e2) -> BinaryOperatorExp op e1 e2

     
    let Parse code =

        match runParserOnString (pExpression .>> eof) () "Graph compile stream" code with
        | Success(result, _, _) ->
            result
        | Failure(msg, _, _) ->
            failwith msg

    ///<summary>
    /// Compiles the given string into a Graph object.
    ///</summary>
    ///<param name="code">The input string to build the graph</param>
    ///<returns>Graph object</returns>
    let Compile (text: string) =
        HandleExpression(Parse text)
