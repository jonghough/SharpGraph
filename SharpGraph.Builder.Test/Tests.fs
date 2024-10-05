module Tests

open Xunit
open FParsec
open SharpGraph.Builder.Parser
open SharpGraph.Builder.Compiler

[<Theory>]
[<InlineData("10..20", true)>]
[<InlineData("'sdA'->'B'->'V'->(0..10)", true)>]
[<InlineData("'sdA'->'B','V'->(0..10)", true)>]
[<InlineData("'sdA'->'B',('V'->(0..10))", true)>]
[<InlineData("(2..3)!", true)>]
[<InlineData("2..3!", true)>]
[<InlineData("2..3!->(2->5)", true)>]
[<InlineData("('A node'->'B node')!", true)>]
[<InlineData("(('A node'->'B node')!)->((1..4)!)", true)>]
[<InlineData("(1,2,3,4)|45", false)>]
[<InlineData(" 1 ", true)>]
[<InlineData("(1 ", false)>]
[<InlineData(" 1 ) ", false)>]
[<InlineData("(((1,2)))", true)>]
[<InlineData("1 *", true)>]
[<InlineData("1 *!|", true)>]
[<InlineData("!", false)>]
[<InlineData("", false)>]
[<InlineData("->", false)>]
[<InlineData("1->", false)>]
[<InlineData("->1", false)>]
[<InlineData("'a' 'b'", false)>]
[<InlineData("'a',->'b'", false)>]
[<InlineData("\t2\t->(\r\n5\r\n,6)", true)>]
[<InlineData("()", false)>]
[<InlineData("\n\n", false)>]
[<InlineData("", false)>]
[<InlineData("10,,20", false)>]
[<InlineData("100 'node1' 'node2'", false)>]
[<InlineData("100 ,'node1', 'node2'", true)>]
[<InlineData("node\' label'", false)>]
let ``Test Parsing Graphs`` (expression: string, expectedResult) =
    let testParser parser str =
        match run parser str with
        | Success(result, _, _) ->
            printfn "Success: %A" result
            true

        | Failure(errorMsg, _, _) ->
            printfn "Failure: %s" errorMsg
            false

    // Test the Range parser
    let parsedExpression = testParser (pExpression .>> eof) expression
    Assert.Equal(expectedResult, parsedExpression)


[<Theory>]
[<InlineData("1", 1, 0, 1)>]
[<InlineData("'a'", 1, 0, 1)>]
[<InlineData("('a')", 1, 0, 1)>]
[<InlineData("(('a'))", 1, 0, 1)>]
[<InlineData("'a','a','a','a','a'", 1, 0, 1)>]
[<InlineData("'a'->'a'", 1, 0, 1)>]
[<InlineData("1,1,1", 1, 0, 1)>]
[<InlineData("1,1,(1)", 1, 0, 1)>]
[<InlineData("'a'->'b'", 2, 1, 1)>]
[<InlineData("(1 -> 2), (2 -> 'other'), (2->'NodethreeX')", 4, 3, 1)>]
[<InlineData("(('A node'->'B node')!)->((1..4)!)", 5, 10, 1)>]
[<InlineData("('A','B','C','D','E')!", 5, 10, 1)>]
[<InlineData("(1,2,3)#(4,5,6)", 6, 9, 1)>]
[<InlineData("1,2,3,4", 4, 0, 4)>]
[<InlineData("(1,2,3,4)*", 4, 4, 1)>]
[<InlineData("(1,2,3,4)|", 4, 3, 1)>]
[<InlineData("(1,2,3,4)|,45", 5, 3, 2)>]
[<InlineData("(0..100)*", 100, 100, 1)>]
[<InlineData("0..100*", 100, 100, 1)>]
[<InlineData("0..100**", 100, 100, 1)>]
[<InlineData("'日本語の名前'->'&%$$'", 2, 1, 1)>]
[<InlineData("(1->2->3->4->5->6),7", 7, 15, 2)>]
[<InlineData("'node1'->3->4->'node1'", 3, 5, 1)>] // two way edge
[<InlineData("(0..4!),(4..10*),(10..20|)", 20, 21, 3)>]
[<InlineData(" 0..4! ,(4..10*),(10..20|)", 20, 21, 3)>]
[<InlineData(" 0..4! , 4..10* ,(10..20|)", 20, 21, 3)>]
[<InlineData(" 0..4! , 4..10* ,10..20|", 20, 21, 3)>]
[<InlineData(" (0..6!) # (6..12*)  #(12..15!)", 15, 96, 1)>]
[<InlineData(@"(0->1->2->3->4),
(5..7!),  
(4->5),(0->7)",
             8,
             13,
             1)>]
[<InlineData("0..200,(199->200)", 201, 1, 200)>]
[<InlineData(@"'A','B','C','D','E','F','G','H','I','J','K',
'L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'",
             26,
             0,
             26)>]
[<InlineData(@"('A','B','C','D','E','F','G','H','I','J','K',
'L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z')!",
             26,
             13 * 25,
             1)>]
[<InlineData(@"('A','B','C','D','E','F','G','H','I','J','K',
'L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z')*",
             26,
             26,
             1)>]

[<InlineData(@"('A','B','C','D','E','F','G','H','I','J','K',
'L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z')|",
             26,
             25,
             1)>]
let ``Test Compiling Graphs``
    (input: string, expectedNodeCount: int, expectedEdgeCount: int, expectedConnectedComponents)
    =
    let a = Compile input
    Assert.Equal(expectedNodeCount, a.GetNodes().Count)
    Assert.Equal(expectedEdgeCount, a.GetEdges().Count)
    Assert.Equal(expectedConnectedComponents, a.GetConnectedComponents().Count)
