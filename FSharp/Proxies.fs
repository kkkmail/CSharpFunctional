namespace FSharp.Lessons

open Primitives
open BusinessEntities

module Proxies =

    type LoggerProxy =
        {
            logError : ErrorData -> unit
        }


    type EmployeeProxy =
        {
            loadEmployee : EmployeeId -> ResultData<Employee>
            loadEmployeeByEmail : EmployeeEmail -> ResultData<Employee>
            saveEmployee : Employee -> ResultData<Employee>
            loadSubordinates : EmployeeId -> ListResult<Employee>
        }
