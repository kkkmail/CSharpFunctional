using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharp.Lessons.DbData;

[Table("EmployeeData")]
public class EFEmployeeData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long EmployeeDataId { get; set; }

    public long EmployeeId { get; set; }
    public long EmployeeDataTypeId { get; set; }

    [MaxLength]
    public string? EmployeeDataValue { get; set; }
}
