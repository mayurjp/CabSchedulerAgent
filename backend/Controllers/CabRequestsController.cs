using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Models;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CabRequestsController : ControllerBase
{
    private readonly CabSchedulerDbContext _context;

    public CabRequestsController(CabSchedulerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CabRequest>>> GetAll()
    {
        return await _context.CabRequests.Include(cr => cr.Employee).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CabRequest>> GetById(int id)
    {
        var cabRequest = await _context.CabRequests.Include(cr => cr.Employee).FirstOrDefaultAsync(cr => cr.Id == id);
        if (cabRequest is null)
            return NotFound();
        return cabRequest;
    }

    [HttpPost]
    public async Task<ActionResult<CabRequest>> Create(CabRequest cabRequest)
    {
        cabRequest.RequestedTime = DateTime.UtcNow;
        cabRequest.Status = "Pending";
        _context.CabRequests.Add(cabRequest);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cabRequest.Id }, cabRequest);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var cabRequest = await _context.CabRequests.FindAsync(id);
        if (cabRequest is null)
            return NotFound();

        cabRequest.Status = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
