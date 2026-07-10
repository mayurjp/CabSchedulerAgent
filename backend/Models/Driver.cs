using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CabScheduler.Api.Models;

public class Driver
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    public int? CabId { get; set; }

    [ForeignKey("CabId")]
    public Cab? AssignedCab { get; set; }
}
