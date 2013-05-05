namespace Excemplate.Language

open System
open System.Collections.Generic

open Excemplate.Language.SyntaxTree

/// <summary>A delegate that handles function calls in the Excemplate expressions.
/// </summary>
type public FunctionCallHandlerDelegate = delegate of string * Dictionary<string, Object> -> Object

type public Evaluator(evalFunc:FunctionCallHandlerDelegate) = 
    (****************** Private Fields ********************)
    let variables = new Dictionary<string, Object>()
    
    (****************** Public Properties ********************)
    member val public EvalFunc:FunctionCallHandlerDelegate = evalFunc with get, set
        
    (****************** Public Methods ********************)
    member public this.DeleteVariable name =
        variables.Remove(name) |> ignore
        ()

    member public this.DeleteVariables () =
        variables.Clear()
        ()

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
            | Function(name, args) -> this.EvalFunc.Invoke(name, (evaluateArgs args))
        
        and evaluateValue value = 
            match value with
            | Int(i) -> i :> Object
            | Double(d) -> d :> Object
            | String(s) -> s :> Object

        and evaluateArgs args =
            let dictionary = new Dictionary<string, Object>()
            for arg in args do
                match arg with
                | NamedArgument(name, value) -> dictionary.Add(name, evaluateExpression value)

            dictionary

        // Parse the line and evaluate the result.
        let lexingBuffer = Lexing.LexBuffer<char>.FromString(statement)
        let syntaxTree = Parser.statement Lexer.tokenize lexingBuffer
        let result = evaluateExpression syntaxTree.Expression
        
        // Save or return the result.
        match syntaxTree.AssignTo with
        | Some(varName) -> variables.[varName] <- result
                           null
        | None -> result

    member public this.SetVariable(name, value:Object) = 
        variables.[name] <- value
        ()

    

