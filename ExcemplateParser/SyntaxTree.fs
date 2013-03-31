module internal ExcemplateEvaluator.SyntaxTree
open System

// Definition of the abstract syntax tree elements.
type Expression = 
    | Value of Literal
    | Var of string
    | Function of string * Argument list
    
and Literal = 
    | Int of int
    | Double of double
    | String of string

and Variable = string
and Function = string * Argument list
and Argument = 
    | NamedArgument of string * Expression
    | OrderedArgument of Expression

type Statement = 
    { AssignTo : string option;
      Expression : Expression; }

