namespace Excemplate.Language

open System

type public ParserException(line:int, character:int, message:string, innerException:Exception) =
    inherit Exception(message, innerException)
    let mutable lineNum = line
    let mutable charNum = character

    (*************** Other Constructors *****************)
    new() = ParserException(0, 0, "", null)
    new(innerException) = ParserException(0, 0, "", innerException);
    new(message, innerException) = ParserException(0, 0, message, innerException)
    new(line, character, innerException) = ParserException(line, character, "", innerException)

    (*************** Public Properties *****************)
    member public this.LineNum
        with get() = lineNum
        and set value = 
            lineNum <- value
            ()

    member public this.CharNum
        with get() = charNum
        and set value = 
            charNum <- value
            ()

    
    