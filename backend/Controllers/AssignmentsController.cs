using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Models;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly CabSchedulerDbContext _context;

    public AssignmentsController(CabSchedulerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Assignment>>> GetAll()
    {
        return await _context.Assignments
            .Include(a => a.Route)
            .Include(a => a.Employee)
            .Include(a => a.CabRequest)
            .ToListAsync();
    }
}
