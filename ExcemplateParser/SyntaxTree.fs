module SyntaxTree
open System

// Definition of the abstract syntax tree elements.
type Expression = 
    | Literal
    | Variable
    | Function
    
and Literal = 
    | Int of int
    | Double of double
    | String of string
    | DateTime

and Variable = string
and Function = string * Argument list
and Argument = NamedArgument | OrderedArgument
and OrderedArgument = Expression
and NamedArgument = string * Expression

type Statement = 
    { AssignTo : string option;
      Expression : Expression; }

