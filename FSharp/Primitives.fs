namespace FSharp.Lessons
open Microsoft.FSharp.Reflection

module Primitives =

    let getAllUnionCases<'T>() =
        FSharpType.GetUnionCases(typeof<'T>)
        |> Seq.map (fun x -> FSharpValue.MakeUnion(x, Array.zeroCreate(x.GetFields().Length)) :?> 'T)
        |> List.ofSeq


    /// Splits the list of results into list of successes and list of failures.
    let unzip r =
        let success e =
            match e with
            | Ok v -> Some v
            | Error _ -> None

        let failure e =
            match e with
            | Ok _ -> None
            | Error e -> Some e

        let sf e = success e, failure e
        let s, f = r |> List.map sf |> List.unzip
        s |> List.choose id, f |> List.choose id


    type ErrorData =
        | DatabaseError of string
        | InvalidDataError of string
        | SomeOtherError of string


    let toInvalidDataError s = $"%A{s}" |> InvalidDataError |> Error


    type ResultData<'T> = Result<'T, ErrorData>
    type UnitResult = ResultData<unit>
    type ListResult<'T> = List<ResultData<'T>>
    type ResultList<'T> = ResultData<List<'T>>


    type EmployeeId =
        | EmployeeId of int64

        member e.value = let (EmployeeId v) = e in v


    type EmployeeName =
        | EmployeeName of string

        member e.value = let (EmployeeName v) = e in v

        static member tryCreate n =
            // Add some validation and/or other logic here.
            n |> EmployeeName |> Ok


    type Email =
        | Email of string

        member e.value = let (Email v) = e in v

        static member tryCreate e =
            // Add some validation and/or other logic here.
            e |> Email |> Ok


    type EmployeeEmail =
        | EmployeeEmail of Email

        member e.value = let (EmployeeEmail (Email v)) = e in v
        static member tryCreate e = Email.tryCreate e |> Result.map EmployeeEmail


    type EmployeeDataType =
        | FavoriteRestaurant
        | HighSchoolMascot
        | PetName

        member t.name = $"%A{t}"

        member t.id =
            match t with
            | FavoriteRestaurant -> 1L
            | HighSchoolMascot -> 2L
            | PetName -> 3L

        static member allValues =
            getAllUnionCases<EmployeeDataType>()
            |> List.map (fun e -> (e.id, e))
            |> Map.ofList

        static member tryCreate i =
            match EmployeeDataType.allValues |> Map.tryFind i with
            | Some e -> Ok e
            | None -> toInvalidDataError $"Invalid EmployeeDataTypeId: i{i}i."
