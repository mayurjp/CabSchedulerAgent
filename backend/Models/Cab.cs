using System.ComponentModel.DataAnnotations;

namespace CabScheduler.Api.Models;

public class Cab
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    public int Capacity { get; set; }
}
