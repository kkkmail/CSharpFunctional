namespace CSharpTests
{
    public class EmployeeIdTests
    {
        [Fact]
        void ZeroValueCanCreate()
        {
            var result = new EmployeeId(0);
            result.Should().NotBeNull();
        }
    }
}
