using CSharp.Lessons.DbData;

namespace CSharpTests;

public class DbDataTests
{
    [Fact]
    void DatabaseAccessShouldWork()
    {
        using var ctx = new DatabaseContext();
        var _ = ctx.EmployeeSet.FirstOrDefault();
    }
}
