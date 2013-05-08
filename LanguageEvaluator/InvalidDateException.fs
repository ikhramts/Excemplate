namespace Excemplate.Language

open System
open Microsoft.FSharp.Text.Lexing

type public InvalidDateException(failedDate:string, message:string, innerException:Exception) = 
    inherit LanguageEvaluatorException(message, innerException)
    let badDate = failedDate

    new(lexbuf:Lexing.LexBuffer<char>, innerException) =
        let badDate = new String(lexbuf.Lexeme)
        let message = "Invalid date/time: " + badDate
        InvalidDateException(badDate, message, innerException)

    member public this.FailedDate
        with get() = badDate

