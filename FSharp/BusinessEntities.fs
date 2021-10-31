namespace FSharp.Lessons
open System
open Primitives

module BusinessEntities =

    type EmployeeData =
        {
            employeeDataType : EmployeeDataType
            emploeeDataValue : string option
        }


    type Employee =
        {
            employeeId : EmployeeId
            employeeName : EmployeeName
            employeeEmail : EmployeeEmail
            managedBy : EmployeeId option
            dateHired : DateTime
            salary : decimal
            description : string option
            data : Map<EmployeeDataType, EmployeeData>
        }
