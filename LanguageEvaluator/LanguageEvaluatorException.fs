namespace Excemplate.Language

open System

type public LanguageEvaluatorException(message:string, innerException:Exception) =
    inherit Exception(message, innerException)
