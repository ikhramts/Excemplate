namespace Excemplate.Language

open System
open System.Collections.Generic
open System.Reflection

open Excemplate.Language.SyntaxTree

/// <summary>A delegate that handles calls to a specific function in Excemplate expressions.
/// </summary>
type public ExcemplateHandlerDelegate = delegate of Dictionary<string, Object> -> Object

/// <summary>A delegate that handles calls to multiple functions in Excemplate expressions.
/// </summary>
type public DefaultHandlerDelegate = delegate of string * Dictionary<string, Object> -> Object

type public Evaluator() = 
    (****************** Private Fields ********************)
    let variables = new Dictionary<string, Object>()
    let handlers = new Dictionary<string, ExcemplateHandlerDelegate>()
    let mutable defaultHandler_ : DefaultHandlerDelegate = null
    
    (****************** Private Functions ********************)
    let returnNoFunctionError (name:string) (_:Dictionary<string, Object>) =
        String.Format("Function '{0}' is not implemented.", name) :> Object

    (*--------------------------------------------------*)
    let invokeHandler (functionName:string) (args:Dictionary<string, Object>) =
        if (handlers.ContainsKey(functionName)) then
            handlers.[functionName].DynamicInvoke(args)

        else
            if defaultHandler_ = null then
                returnNoFunctionError functionName args

            else
                defaultHandler_.Invoke(functionName, args)

    (*--------------------------------------------------*)
    ///<summary>Prepare arguments passed to a handler for use by a 
    /// method</summary>
    let prepareMethodParams (functionName:string) 
                  (args : Dictionary<string, Object>) 
                  (paramInfos : ParameterInfo[]) =

        let mutable skippedOptionalArgs = 0
        let mutable methodParams : Object list = []

        // Check that all arguments have correct type and all
        // required arguments are present.
        for paramInfo in paramInfos do
            let name = paramInfo.Name

            if (args.ContainsKey(name)) then
                let paramType = paramInfo.ParameterType

                if not (args.[name].GetType().IsInstanceOfType(paramType)) then
                    let message = String.Format("Function '{0}' argument '{1}' must be of type {2}.",
                                                functionName, name, paramType.ToString())
                    raise(FunctionEvaluatorException(functionName, args, message))

                methodParams <- args.[name] :: methodParams

            else if (paramInfo.IsOptional) then
                skippedOptionalArgs <- skippedOptionalArgs + 1
                methodParams <- paramInfo.DefaultValue :: methodParams

            else
                let message = String.Format("Function '{0}' argument '{1}' is required.",
                                                functionName, name)
                raise(FunctionEvaluatorException(functionName, args, message))
                
        // Check that no extra arguments were provided.    
        if (paramInfos.GetLength(0) < (args.Count + skippedOptionalArgs)) then
            let paramNames = paramInfos
                                   |> Array.map (fun p -> p.Name)
                                   |> Set.ofArray
            
            for arg in args do
                let argName = arg.Key

                if not (Set.contains argName paramNames) then
                    let message = String.Format("Function '{0}' does not have argument '{1}'.",
                                                functionName, argName)
                    raise(FunctionEvaluatorException(functionName, args, message))

        // Rearrange prepare the array of method parameters.
        methodParams
        |> List.rev
        |> Array.ofList

    (*--------------------------------------------------*)
    let makeInstanceHandler (functionName : string) 
                            (methodInfo : MethodInfo) 
                            (target : Object)= 
        let paramInfos = methodInfo.GetParameters()
        ExcemplateHandlerDelegate( 
            fun args ->
                let methodParams = prepareMethodParams functionName args paramInfos
                methodInfo.Invoke(target, methodParams)
            )

    (*--------------------------------------------------*)
    let makeStaticHandler (functionName : string)
                          (methodInfo : MethodInfo) = 
        let paramInfos = methodInfo.GetParameters()
        ExcemplateHandlerDelegate( 
            fun args ->
                let methodParams = prepareMethodParams functionName args paramInfos
                methodInfo.Invoke(null, methodParams)
            )
    
    (****************** Public Properties ********************)
    member public this.DefaultHandler 
        with get() = defaultHandler_
        and set(value) = defaultHandler_ <- value

    (****************** Public Methods ********************)
    member public this.AddHandler(functionName:string, handler:ExcemplateHandlerDelegate) =
        handlers.[functionName] <- handler

    (*--------------------------------------------------*)
    member public this.AddHandler(functionName:string, obj:Object, methodName:string) =
        let methodInfo = obj.GetType().GetMethod(methodName);
        handlers.[functionName] <- (makeInstanceHandler functionName methodInfo obj)

    (*--------------------------------------------------*)
    member public this.AddHandler(functionName:string, t : Type, methodName:string) = 
        let methodInfo = t.GetMethod(methodName);
        handlers.[functionName] <- (makeStaticHandler functionName methodInfo)

    (*--------------------------------------------------*)
    member public this.DeleteVariable name =
        variables.Remove(name) |> ignore
        ()

    (*--------------------------------------------------*)
    member public this.DeleteVariables () =
        variables.Clear()
        ()

    (*--------------------------------------------------*)
    /// <summary>Evaluates an Excemplate statement and either returns the result or 
    /// saves the result to an internal variable dictionary, depending on the type of
    /// statement </summary>
    /// <param name="statement">Statement to evaluate.</param>
    /// <returns>Null if the result was saved in a variable; otherwise, the result of
    /// evaluating the statement.</returns>
    member public this.Evaluate statement =
        // A set of functions that handle evaluation of different components of
        // the abstract syntax tree.  This implementation is not tail recursive;
        // at some point it will be good to try to rewrite this to be tail recursive
        // using continuations (if it is possible).
        let rec evaluateExpression expr =
            match expr with
            | Value(value) -> evaluateValue value
            | Var(v) -> variables.[v]
            | Function(name, args) -> 
                let evaluatedArgs = evaluateArgs args
                                      
                try invokeHandler name evaluatedArgs
                with
                | ex as Exception -> raise(new FunctionEvaluatorException(name, evaluatedArgs, ex))
        
        and evaluateValue value = 
            match value with
            | Int(i) -> i :> Object
            | Double(d) -> d :> Object
            | String(s) -> s :> Object
            | Date(d) -> d :> Object

        and evaluateArgs args =
            let dictionary = new Dictionary<string, Object>()
            for arg in args do
                match arg with
                | NamedArgument(name, value) -> dictionary.Add(name, evaluateExpression value)

            dictionary

        // Parse the line and evaluate the result.
        let lexingBuffer = Lexing.LexBuffer<char>.FromString(statement)
        let syntaxTree =
            try Parser.statement Lexer.tokenize lexingBuffer
            with
            | ex as Exception -> 
                match ex.Message with
                | "parse error" -> raise(new UnexpectedTokenException(lexingBuffer, ex, statement))
                | "unrecognized input" -> raise(new UnrecognizedInputException(lexingBuffer, ex, statement))
                | _ -> reraise()
        
        let result = evaluateExpression syntaxTree.Expression
        
        // Save or return the result.
        match syntaxTree.AssignTo with
        | Some(varName) -> variables.[varName] <- result
                           null
        | None -> result

    (*--------------------------------------------------*)
    member public this.RemoveHandler(functionName:string) =
        handlers.Remove(functionName)

    (*--------------------------------------------------*)
    member public this.SetVariable(name, value:Object) = 
        variables.[name] <- value
        ()

    
    
    
