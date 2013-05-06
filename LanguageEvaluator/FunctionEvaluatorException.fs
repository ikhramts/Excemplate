namespace Excemplate.Language

open System
open System.Collections.Generic
open System.Linq
open System.Text

type public FunctionEvaluatorException(funcName:string, args:Dictionary<string, Object>, message:string, innerException:Exception) = 
    inherit Exception(message, innerException)

    let mutable functionName_ = funcName
    let mutable functionArgs_ = args

    (*************** Other Constructors *****************)
    new(innerException) = FunctionEvaluatorException(null, null, null, innerException)
    new(funcName, args, innerException) = FunctionEvaluatorException(funcName, args, null, innerException)

    (*************** Public Properties *****************)
    member public this.FunctionName
        with get() = functionName_
        and set value = functionName_ <- value

    member public this.FunctionArgs
        with get() = functionArgs_
        and set value = functionArgs_ <- value


    (*************** Public Methods *****************)
    override this.ToString() =
        let message = new StringBuilder()
        message.Append("Function \"").Append(functionName_).Append("(") |> ignore

        if functionArgs_ <> null then
            let mutable first = true

            for KeyValue(argName, argValue) in functionArgs_ do
                if first then 
                    first <- false
                else 
                    message.Append(", ") |> ignore

                message.Append(argName).Append('=').Append(argValue) |> ignore

        message.Append(") produced an error: ").Append(innerException.Message) |> ignore
        message.ToString()

                
                

        
        