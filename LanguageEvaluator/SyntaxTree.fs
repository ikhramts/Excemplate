module internal Excemplate.Language.SyntaxTree
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
    | Date of DateTime

and Argument = 
    | NamedArgument of string * Expression

type Statement = 
    { AssignTo : string option;
      Expression : Expression; }
