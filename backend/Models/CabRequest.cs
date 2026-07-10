using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CabScheduler.Api.Models;

public class CabRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }

    [Required]
    [MaxLength(250)]
    public string PickupLocation { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string DropoffLocation { get; set; } = string.Empty;

    public DateTime RequestedTime { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
}
