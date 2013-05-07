namespace Excemplate.Language

open System
open Microsoft.FSharp.Text.Lexing

type public UnexpectedTokenException(line:int, character:int, token:string, message:string, innerException:Exception) =
    inherit LanguageEvaluatorException(message, innerException)

    let lineNum_ = line
    let charNum_ = character
    let token_ = token

    (*************** Other Constructors *****************)
    new(lexbuf:Lexing.LexBuffer<char>, innerException) = 
        let token = new String(lexbuf.Lexeme)
        let character = lexbuf.EndPos.Column - token.Length
        let line = lexbuf.EndPos.Line
        let message:string = "Invalid element '" + token + "' at line " + (line + 1).ToString() + " column " + (character + 1).ToString() + "."
        UnexpectedTokenException(line, character, token, message, innerException)
    
    (*************** Public Properties *****************)
    member public this.LineNum 
        with get() = lineNum_

    member public this.CharNum 
        with get() = charNum_

    member public this.Token 
        with get() = token_

    (*************** Public Properties *****************)
    