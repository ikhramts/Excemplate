namespace Excemplate.Language

open System

type public InvalidDateException(failedDate:string, innerException:Exception) = 
    inherit Exception("Invalid date/time: " + failedDate, innerException)
    let badDate = failedDate

    member public this.FailedDate
        with get() = badDate