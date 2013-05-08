namespace Excemplate.Language

open System
open System.Text
open Microsoft.FSharp.Text.Lexing

type public UnrecognizedInputException(line:int, column:int, message:string, innerException:Exception) =
    inherit LanguageEvaluatorException(message, innerException)
    let mutable lineNum = line
    let mutable charNum = column

    (*************** Other Constructors *****************)
    new(lexbuf:Lexing.LexBuffer<char>, innerException, command:string) = 
        let line = lexbuf.EndPos.Line
        let column = lexbuf.EndPos.Column
        let message = 
            "Unexpected character at line " + (line + 1).ToString() + 
            " column " + (column + 1).ToString() + 
            ".  Input was: \"" + command + "\"."

        UnrecognizedInputException(line, column, message, innerException)

                

    (*************** Public Properties *****************)
    member public this.LineNum
        with get() = lineNum

    member public this.CharNum
        with get() = charNum

    
    