namespace Excemplate.Language

open System
open Microsoft.FSharp.Text.Lexing

type public UnrecognizedInputException(line:int, character:int, innerException:Exception) =
    inherit LanguageEvaluatorException(null, innerException)
    let mutable lineNum = line
    let mutable charNum = character

    (*************** Other Constructors *****************)
    new(lexbuf:Lexing.LexBuffer<char>, innerException) = 
        let line = lexbuf.EndPos.Line
        let character = lexbuf.EndPos.Column
        UnrecognizedInputException(line, character, innerException)

    (*************** Public Properties *****************)
    member public this.LineNum
        with get() = lineNum

    member public this.CharNum
        with get() = charNum

    
    