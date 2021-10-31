namespace FSharp.Lessons
open System
open FSharp.Data.Sql
open FSharp.Data.Sql.Transactions

open Primitives
open BusinessEntities
open Proxies

module DbData =

    [<Literal>]
    let DbConnectionString = "Data Source=localhost;Initial Catalog=Example;Integrated Security=SSPI"


    type ConnectionString =
        | ConnectionString of string

        member this.value = let (ConnectionString v) = this in v
        static member defaultValue() = ConnectionString DbConnectionString


    type private Database = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, DbConnectionString, UseOptionTypes = true>
    type private DatabaseContext = Database.dataContext
    let private getContext (c : unit -> ConnectionString) = c().value |> Database.GetDataContext

    let private getContext2 (c : unit -> ConnectionString) =
        Database.GetDataContext(c().value, { Timeout = TimeSpan.FromSeconds(60); IsolationLevel = IsolationLevel.Chaos })

    let private mapExceptionToError e = $"Exception: '%A{e}'." |> DatabaseError |> Error


    type private EmployeeTable = Database.dataContext.``dbo.EmployeeEntity``
    type private EmployeeDataTable = Database.dataContext.``dbo.EmployeeDataEntity``
    type private EmployeeDataTypeTable = Database.dataContext.``dbo.EmployeeDataTypeEntity``


    let private tryDbFun f =
        try
            f()
        with
        | e -> mapExceptionToError e


    let private mapEmployee (e : EmployeeTable) =
        match EmployeeName.tryCreate e.EmployeeName, EmployeeEmail.tryCreate e.EmployeeEmail with
        | Ok name, Ok email ->
            {
                employeeId = EmployeeId e.EmployeeId
                employeeName = name
                employeeEmail = email
                managedBy = e.ManagedByEmployeeId |> Option.map EmployeeId
                dateHired = e.DateHired
                salary = e.Salary
                description = e.Description
                data = Map.empty
            }
            |> Ok
        | _ -> toInvalidDataError $"Name: '%A{e.EmployeeName}' and/or email: '%A{e.EmployeeEmail}' are invalid."


    let private mapEmployeeData (d : EmployeeDataTable) =
        EmployeeDataType.tryCreate d.EmployeeDataTypeId
        |> Result.map (fun t ->
                            {
                                employeeDataType = t
                                emploeeDataValue = d.EmployeeDataValue
                            })


    let private mapEmployeeResult i (e, d) =
        match e |> Seq.map mapEmployee |> Seq.tryHead with
        | Some (Ok employee) ->
            let (data, errors) = d |> Seq.map mapEmployeeData |> List.ofSeq |> unzip
            match errors with
            | [] -> Ok { employee with data = data |> List.map (fun e -> (e.employeeDataType, e)) |> Map.ofList }
            | _ ->
                let m = errors |> List.map (fun e -> $"'%A{e}'") |> String.concat ", "
                toInvalidDataError $"Some data is invalid: {m}."
        | Some (Error e) -> Error e
        | None -> toInvalidDataError $"Cannot find employee with ID: '%A{i}'."


    let private loadEmployee c (EmployeeId i) =
        let g () =
            let ctx = getContext c

            let e =
                query {
                    for e in ctx.Dbo.Employee do
                    where (e.EmployeeId = i)
                    select e
                }

            let d =
                query {
                    for d in ctx.Dbo.EmployeeData do
                    where (d.EmployeeId = i)
                    select d
                }

            mapEmployeeResult i (e, d)

        tryDbFun g


    let private loadEmployeeByEmail c (EmployeeEmail (Email email)) =
        let g() =
            let ctx = getContext c

            let x =
                query {
                    for e in ctx.Dbo.Employee do
                    where (e.EmployeeEmail = email)
                    select (Some e.EmployeeId)
                    exactlyOneOrDefault
                }

            match x with
            | Some v -> loadEmployee c (EmployeeId v)
            | None -> toInvalidDataError $"Cannot find employee with email: '{email}'."

        tryDbFun g


    let private deleteEmployeeData (ctx : DatabaseContext) (EmployeeId i) =
        let g() =
            query {
                for e in ctx.Dbo.EmployeeData do
                where (e.EmployeeId = i)
            }
            |> Seq.``delete all items from single table``
            |> Async.RunSynchronously
            |> Ok

        tryDbFun g

    //let mutable private failUpdateEmployeeData = false


    let private saveEmployeeData (ctx : DatabaseContext) (EmployeeId i) (d : EmployeeData) =
        let g() =
            let x =
                query {
                    for e in ctx.Dbo.EmployeeData do
                    where (e.EmployeeDataTypeId = d.employeeDataType.id)
                    select (Some e)
                    exactlyOneOrDefault
                }

            match x with
            | Some v ->
                v.EmployeeDataValue <- d.emploeeDataValue
                ctx.SubmitUpdates()
            | None ->
                //if failUpdateEmployeeData
                //then failwith "Simulating database error in the middle of lengthy update."

                let t =
                    query {
                        for e in ctx.Dbo.EmployeeDataType do
                        where (e.EmployeeDataTypeId = d.employeeDataType.id)
                        select (Some e)
                        exactlyOneOrDefault
                    }

                match t with
                | None ->
                    let dt = ctx.Dbo.EmployeeDataType.``Create(EmployeeDataTypeId, EmployeeDataTypeName)``(
                        EmployeeDataTypeId = d.employeeDataType.id,
                        EmployeeDataTypeName = d.employeeDataType.name)

                    ctx.SubmitUpdates()
                | Some _ -> ()

                let employeeData = ctx.Dbo.EmployeeData.``Create(EmployeeDataTypeId, EmployeeId)``(
                    EmployeeDataTypeId = d.employeeDataType.id,
                    EmployeeId = i)

                employeeData.EmployeeDataValue <- d.emploeeDataValue
                ctx.SubmitUpdates()
            |> Ok

        tryDbFun g


    let private saveEmployee c e =
        let g() =
            let logIfErr r =
                match r with
                | Error e ->
                    printfn $"Error: '%A{e}'."
                    //failwith $"%A{e}"
                | Ok _ -> ()

            let ctx = getContext c

            let x =
                query {
                    for d in ctx.Dbo.Employee do
                    where (d.EmployeeId = e.employeeId.value)
                    select (Some d)
                    exactlyOneOrDefault
                }

            let updateData i =
                deleteEmployeeData ctx i
                |> logIfErr

                e.data
                |> Map.values
                |> List.ofSeq
                |> List.map (saveEmployeeData ctx i)
                |> List.map logIfErr
                |> ignore

            match x with
            | Some v ->
                //failUpdateEmployeeData <- not failUpdateEmployeeData

                v.EmployeeName <- e.employeeName.value
                v.EmployeeEmail <- e.employeeEmail.value
                v.ManagedByEmployeeId <- e.managedBy |> Option.map (fun a -> a.value)
                v.DateHired <- e.dateHired
                v.Salary <- e.salary
                v.Description <- e.description

                updateData e.employeeId
                ctx.SubmitUpdates()

                loadEmployee c e.employeeId
            | None ->
                let employee = ctx.Dbo.Employee.Create(
                    DateHired = e.dateHired,
                    EmployeeEmail = e.employeeEmail.value,
                    //EmployeeName = e.employeeName.value,
                    ManagedByEmployeeId = (e.managedBy |> Option.map (fun a -> a.value)),
                    Salary = e.salary,
                    Description = e.description)

                // Glitch in the library.
                employee.EmployeeName <- e.employeeName.value

                ctx.SubmitUpdates()
                let i = EmployeeId employee.EmployeeId
                updateData i
                ctx.SubmitUpdates()

                loadEmployee c i

        tryDbFun g


    let private loadSubordinates c (EmployeeId i) =
        let g() =
            try
                let ctx = getContext c

                let e =
                    query {
                        for e in ctx.Dbo.Employee do
                        where (e.ManagedByEmployeeId = Some i)
                        select e.EmployeeId
                    }

                e
                |> List.ofSeq
                |> List.map (fun i -> loadEmployee c (EmployeeId i))
            with
            | e -> [ mapExceptionToError e ]

        g()


    type EmployeeProxy
        with
        static member create c =
            {
                loadEmployee = loadEmployee c
                loadEmployeeByEmail = loadEmployeeByEmail c
                saveEmployee = saveEmployee c
                loadSubordinates = loadSubordinates c
            }
