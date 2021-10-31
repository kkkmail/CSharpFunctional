using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharp.Lessons.DbData;

[Table("EmployeeDataType")]
public class EFEmployeeDataType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long EmployeeDataTypeId { get; set; }

    [Required]
    [StringLength(50)]
    public string EmployeeDataTypeName { get; set; } = null!;
}
