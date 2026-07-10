using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupervisorController : ControllerBase
{
    private readonly CabSchedulerDbContext _context;

    public SupervisorController(CabSchedulerDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var totalEmployees = await _context.Employees.CountAsync();
        var totalCabs = await _context.Cabs.CountAsync();
        var totalDrivers = await _context.Drivers.CountAsync();
        var pendingRequests = await _context.CabRequests.CountAsync(cr => cr.Status == "Pending");
        var activeRoutes = await _context.Routes.CountAsync();
        var todayAssignments = await _context.Assignments.CountAsync();

        var recentRoutes = await _context.Routes
            .Include(r => r.Cab)
            .Include(r => r.Driver)
            .Include(r => r.Assignments)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Cycle,
                CabPlate = r.Cab != null ? r.Cab.PlateNumber : "N/A",
                DriverName = r.Driver != null ? r.Driver.Name : "N/A",
                EmployeeCount = r.Assignments.Count,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            summary = new
            {
                totalEmployees,
                totalCabs,
                totalDrivers,
                pendingRequests,
                activeRoutes,
                todayAssignments
            },
            recentRoutes
        });
    }
}
