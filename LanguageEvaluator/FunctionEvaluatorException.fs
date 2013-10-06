namespace Excemplate.Language

open System
open System.Collections.Generic
open System.Linq
open System.Text

type public FunctionEvaluatorException(funcName:string, args:Dictionary<string, Object>, message:string, innerException:Exception) = 
    inherit LanguageEvaluatorException(message, innerException)

    let mutable functionName_ = funcName
    let mutable functionArgs_ = args

    (*************** Other Constructors *****************)
    new(innerException) = FunctionEvaluatorException(null, null, null, innerException)

    new(funcName:string, args:Dictionary<string, Object>, innerException:Exception) = 
        let message = new StringBuilder()
        message.Append("Function \"").Append(funcName).Append("(") |> ignore

        if args <> null then
            // We have to iterate over the dictionary in a more convoluted manner
            // due to restrictions on the types of expressions that may appear
            // in the constructor.
            let mutable first = true
            let argsArray = args.ToArray()
            let numArgs = argsArray.Length

            for i = 0 to (numArgs - 1) do
                if first then 
                    first <- false
                else 
                    message.Append(", ") |> ignore
                
                let argName = argsArray.[i].Key
                let argValue = 
                    match argsArray.[i].Value with
                    | :? DateTime as date -> date.ToString("yyyy-MM-dd'T'HH':'mm':'ss")
                    | obj -> obj.ToString()
                                
                message.Append(argName).Append('=').Append(argValue) |> ignore

        message.Append(")\" produced an error: ").Append(innerException.Message) |> ignore
        FunctionEvaluatorException(funcName, args, message.ToString(), innerException)

    new(funcName:string, args:Dictionary<string, Object>, message:string) =
        FunctionEvaluatorException(funcName, args, message, null)

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

                
                

        
        