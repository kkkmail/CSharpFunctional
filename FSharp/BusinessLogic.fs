namespace FSharp.Lessons
open System
open System.Linq

open Primitives
open BusinessEntities

module BusinessLogic =

    type SalaryRaise =
        | RaiseByPct of decimal
        | RaiseByAmount of decimal

        static member tryCreateByPctRaise p =
            if p < 0.0m || p > 1.0m then toInvalidDataError $"The percentage parameter: '{p}' is invalid."
            else Ok (RaiseByPct p)

        static member tryCreateByAmountRaise p =
            if p < 0.0m || p > 10_000.0m then toInvalidDataError $"The amount parameter: '{p}' is invalid."
            else Ok (RaiseByAmount p)

        member r.raiseIncome e =
            match r with
            | RaiseByPct p -> { e with salary = e.salary * (1.0m + p) }
            | RaiseByAmount a -> { e with salary = e.salary + a }

        member r.raiseAll e = e |> List.map r.raiseIncome


    let tryRaiseAllByPct p e =
        SalaryRaise.tryCreateByPctRaise p
        |> Result.map (fun r -> r.raiseAll)
        |> Result.bind (fun f -> f e |> Ok)


    let emailRules =
        [
            fun (email : string) ->
                if String.IsNullOrWhiteSpace email
                then Some $"Email: '{email}' must not be null or empty."
                else None

            fun email ->
                if email.Count(fun c -> c = '@') <> 1
                then Some $"Email: '{email}' must have exactly one '@'."
                else None
        ]


    let employeeEmailRules =
        emailRules
        @
        [
            fun email ->
                if email.ToLower().Trim().EndsWith EmployeeEmail.corporateDomain |> not
                then Some "Employee email must end with corporate domain name."
                else None
        ]
