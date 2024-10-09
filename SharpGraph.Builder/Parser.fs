namespace SharpGraph.Builder

module Parser =
    open FParsec 

    
    type NodeIdentifier =
        | NamedNode of IdentifierName
        | NumberedNode of int

    and IdentifierName = IdentifierName of string
    and Range = Range of int * int

    and UnaryOperator =
        | Complete
        | Cycle
        | Path

    and BinaryOperator =
        | BipartiteComplete
        | Aggregate
        | Edge

    and Expression =
        | NodeIdentifierExp of NodeIdentifier
        | RangeExp of Range
        | UnaryExp of Expression * UnaryOperator
        | BinaryOperatorExp of BinaryOperator * Expression * Expression


    let ws = spaces 

    let stringLiteral: Parser<IdentifierName, unit> =
        between (pchar ''') (pchar ''') (manyChars (noneOf "'")) |>> IdentifierName

    let pNamedNode: Parser<NodeIdentifier, unit> = stringLiteral |>> NamedNode

    let pNumberedNode: Parser<NodeIdentifier, unit> =
        pint32 .>> spaces .>> notFollowedByString ".." |>> NumberedNode

    let pNodeIdentifier: Parser<NodeIdentifier, unit> =
        attempt pNamedNode <|> pNumberedNode

    let pRange: Parser<Range, unit> =
        let rangeContent =
            pipe2 (pint32 .>> pstring "..") pint32 (fun start end' -> Range(start, end'))

        attempt (between (pchar '(') (pchar ')') rangeContent) <|> rangeContent


    let pExpression, pExpressionRef = createParserForwardedToRef<Expression, unit> ()

    let pSimpleExpression: Parser<Expression, unit> =
        choice
            [ attempt (
                  pRange <|> (between (pchar '(' >>. spaces) (spaces >>. pchar ')') pRange)
                  |>> RangeExp
              )
              attempt (
                  pNodeIdentifier
                  <|> (between (pchar '(' >>. spaces) (spaces >>. pchar ')') pNodeIdentifier)
                  |>> NodeIdentifierExp
              ) ]
 

    let opp = new OperatorPrecedenceParser<Expression, unit, unit>()

    opp.TermParser <-
        pSimpleExpression
        <|> (between (pchar '(' .>> spaces) (pchar ')' .>> spaces) pExpression)

    opp.AddOperator(InfixOperator("->", ws, 1, Associativity.Left, fun x y -> BinaryOperatorExp(Edge, x, y)))
    opp.AddOperator(InfixOperator(",", ws, 2, Associativity.Left, fun x y -> BinaryOperatorExp(Aggregate, x, y)))

    opp.AddOperator(
        InfixOperator("#", ws, 3, Associativity.Left, fun x y -> BinaryOperatorExp(BipartiteComplete, x, y))
    )

    opp.AddOperator(PostfixOperator("!", ws, 4, true, fun x -> UnaryExp(x, Complete)))
    opp.AddOperator(PostfixOperator("*", ws, 4, true, fun x -> UnaryExp(x, Cycle)))
    opp.AddOperator(PostfixOperator("|", ws, 4, true, fun x -> UnaryExp(x, Path)))

    let pOperatorExpression = ws >>. opp.ExpressionParser .>> ws 

    let expressionParser =
        let c = choice [ (attempt pOperatorExpression .>> notFollowedBy pExpression ); attempt pSimpleExpression ]
        attempt c 

    pExpressionRef.Value <- expressionParser
