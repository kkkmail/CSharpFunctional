global using System;
global using System.Linq;
global using MoreLinq.Extensions;
global using System.Collections.Immutable;
global using System.Runtime.CompilerServices;
global using System.Threading.Tasks;

global using CSharp.Lessons;
global using CSharp.Lessons.Functional;
global using CSharp.Lessons.Functional.Validation;
global using CSharp.Lessons.Primitives;
global using CSharp.Lessons.Primitives.Validators;
global using CSharp.Lessons.BusinessEntities;
global using CSharp.Lessons.Proxies;
global using Unit = System.ValueTuple;
global using static CSharp.Lessons.Functional.F;

// This does not compile. So, you can't bind one generic parameter.
//global using ResultData<T> = CSharp.Lessons.Functional.Result<T, ErrorData>

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
