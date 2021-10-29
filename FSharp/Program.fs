open System

open FSharp.Lessons.Primitives
open FSharp.Lessons.BusinessEntities
open FSharp.Lessons.Proxies
open FSharp.Lessons.BusinessLogic
open FSharp.Lessons.DbData

let name = EmployeeName "John Smith"
let newName = EmployeeName "John Smith Jr"
let anotherNewName = EmployeeName "John Smith Sr"

let proxy = EmployeeProxy.create ConnectionString.defaultValue
let email = EmployeeEmail (Email "John.Smith@nowhere.gg")
let wrongEmail = EmployeeEmail (Email "Joohn.Smith@nowhere.gg")

let pet =
    {
        employeeDataType = EmployeeDataType.PetName
        emploeeDataValue = Some "Jack"
    }

let mascot =
    {
        employeeDataType = EmployeeDataType.HighSchoolMascot
        emploeeDataValue = Some "Eagle"
    }

let restaurant =
    {
        employeeDataType = EmployeeDataType.FavoriteRestaurant
        emploeeDataValue = Some "Pizza Hut"
    }

let employee =
    {
        employeeId = EmployeeId 0L
        employeeName = name
        employeeEmail = email
        managedBy = None
        dateHired = DateTime.Now
        salary = 100.0m
        description = None
        data =
            [ pet ; mascot ]
            |> List.map (fun e -> (e.employeeDataType, e))
            |> Map.ofList
    }

printfn "Trying to create employee in the database."
let result1 = proxy.saveEmployee employee
printfn $"Result: %A{result1}\n\n"
Console.ReadLine() |> ignore

printfn $"Trying to get employee with email: {email}."
let result2 = proxy.loadEmployeeByEmail email
printfn $"Result: %A{result2}\n\n"
Console.ReadLine() |> ignore

match result2 with
| Ok r ->
    printfn $"Trying to remove data with type: {pet.employeeDataType}."
    let r1 = { r with employeeName = newName; data = r.data |> Map.remove pet.employeeDataType }
    let r2 = proxy.saveEmployee r1
    printfn $"Result: %A{r2}\n\n"
    Console.ReadLine() |> ignore

    match r2 with
    | Ok r ->
        printfn $"Trying to add data with type: {restaurant.employeeDataType}."
        let r1 = { r with employeeName = anotherNewName; data = r.data |> Map.add restaurant.employeeDataType restaurant }
        let r2 = proxy.saveEmployee r1
        printfn $"Result: %A{r2}\n\n"
        Console.ReadLine() |> ignore
    | _ -> printfn "Result is invalid - cannot proceed."
| _ -> printfn "Result is invalid - cannot proceed."

printfn $"Trying to get employee with wrong email: {wrongEmail}."
let result3 = proxy.loadEmployeeByEmail wrongEmail
printfn $"Result: %A{result3}\n\n"
Console.ReadLine() |> ignore
