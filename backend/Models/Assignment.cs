using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CabScheduler.Api.Models;

public class Assignment
{
    [Key]
    public int Id { get; set; }

    public int RouteId { get; set; }

    [ForeignKey("RouteId")]
    public Route? Route { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }

    public int CabRequestId { get; set; }

    [ForeignKey("CabRequestId")]
    public CabRequest? CabRequest { get; set; }
}
