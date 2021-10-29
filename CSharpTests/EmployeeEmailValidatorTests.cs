global using Xunit;
global using System;
global using System.Linq;
global using MoreLinq.Extensions;
global using System.Collections.Immutable;
global using CSharp.Lessons;
global using CSharp.Lessons.Functional;
global using CSharp.Lessons.Functional.Validation;
global using CSharp.Lessons.Primitives;
global using CSharp.Lessons.Primitives.Validators;
global using Unit = System.ValueTuple;
global using static CSharp.Lessons.Functional.F;
global using FluentAssertions;
global using System.Threading.Tasks;

namespace CSharpTests
{
    public class EmployeeEmailValidatorTests
    {
        [Fact]
        public void NullStringMustFail()
        {
            string? email = null;
            var result = EmployeeEmail.TryCreate(email!);
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            $"{result}".Should().Contain("cannot be null");
        }

        [Fact]
        public void WhiteSpaceStringMustFail()
        {
            var result = EmployeeEmail.TryCreate("    ");
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            $"{result}".Should().Contain("must not be null or empty");
        }

        [Fact]
        public void MoreThanOneAtMustFail()
        {
            var result = EmployeeEmail.TryCreate("abc@def@gh");
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            $"{result}".Should().Contain("must have exactly one '@'");
        }

        [Fact]
        public void NonCorporateDomainMustFail()
        {
            var result = EmployeeEmail.TryCreate("john@doe.com");
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            $"{result}".Should().Contain("Employee email must end with corporate domain name.");
        }
    }
}
