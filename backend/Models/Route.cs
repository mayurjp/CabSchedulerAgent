using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CabScheduler.Api.Models;

public class Route
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int CabId { get; set; }

    [ForeignKey("CabId")]
    public Cab? Cab { get; set; }

    public int DriverId { get; set; }

    [ForeignKey("DriverId")]
    public Driver? Driver { get; set; }

    [Required]
    [MaxLength(20)]
    public string Cycle { get; set; } = "Morning";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Assignment> Assignments { get; set; } = new();
}
