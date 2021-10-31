namespace FSharp.Lessons
open System

open Primitives
open BusinessEntities

module BusinessLogic =

    type IncomeRaise =
        | RaiseByPct of decimal
        | RaiseByAmount of decimal

        static member tryCreateByPctRaise p =
            if p < 0.0m || p > 1.0m then toInvalidDataError $"The percentage parameter: '{p}' is invalid."
            else Ok (RaiseByPct p)

        static member tryCreateByAmountRaise p =
            if p < 0.0m || p > 10_000.0m then toInvalidDataError $"The amount parameter: '{p}' is invalid."
            else Ok (RaiseByAmount p)


    let raiseIncome r e =
        match r with
        | RaiseByPct p -> { e with salary = e.salary * (1.0m + p) }
        | RaiseByAmount a -> { e with salary = e.salary + a }


    let raiseAll r e = e |> List.map (raiseIncome r)


    let tryRaiseAllByPct p e =
        IncomeRaise.tryCreateByPctRaise p
        |> Result.map raiseAll
        |> Result.bind (fun f -> f e |> Ok)
