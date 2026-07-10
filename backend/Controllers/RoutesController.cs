using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Models;
using RouteEntity = CabScheduler.Api.Models.Route;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly CabSchedulerDbContext _context;

    public RoutesController(CabSchedulerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteEntity>>> GetAll()
    {
        return await _context.Routes
            .Include(r => r.Cab)
            .Include(r => r.Driver)
            .Include(r => r.Assignments)
                .ThenInclude(a => a.Employee)
            .Include(r => r.Assignments)
                .ThenInclude(a => a.CabRequest)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RouteEntity>> GetById(int id)
    {
        var route = await _context.Routes
            .Include(r => r.Cab)
            .Include(r => r.Driver)
            .Include(r => r.Assignments)
                .ThenInclude(a => a.Employee)
            .Include(r => r.Assignments)
                .ThenInclude(a => a.CabRequest)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (route is null)
            return NotFound();
        return route;
    }
}
