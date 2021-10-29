using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharp.Lessons.DbData;

[Table("Employee")]
public class EFEmployee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long EmployeeId { get; set; }

    [Required]
    [StringLength(50)]
    public string EmployeeName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string EmployeeEmail { get; set; } = null!;

    public DateTime DateHired { get; set; }
    public decimal Salary { get; set; }

    [MaxLength]
    public string Description { get; set; } = null!;

    public long? ManagedByEmployeeId { get; set; }
}
