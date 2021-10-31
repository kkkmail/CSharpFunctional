using Microsoft.EntityFrameworkCore;

namespace CSharp.Lessons.DbData;

public class DatabaseContext : DbContext
{
    private static ConnectionString ConnectionString => ConnectionString.DefaultValue;

    public DbSet<EFEmployee> EmployeeSet { get; set; } = null!;
    public DbSet<EFEmployeeData> EmployeeDataSet { get; set; } = null!;
    public DbSet<EFEmployeeDataType> EmployeeDataTypeSet { get; set; } = null!;

    public static DbContextOptions<DatabaseContext> GetDbContextOptions(string connectionString) =>
        new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString)
            .Options;

    public DatabaseContext() : base(GetDbContextOptions(ConnectionString.Value))
    {
    }

    public DatabaseContext(ConnectionString connectionString) : base(GetDbContextOptions(connectionString.Value))
    {
    }
}
