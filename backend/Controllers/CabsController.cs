using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Models;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CabsController : ControllerBase
{
    private readonly CabSchedulerDbContext _context;

    public CabsController(CabSchedulerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cab>>> GetAll()
    {
        return await _context.Cabs.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cab>> GetById(int id)
    {
        var cab = await _context.Cabs.FindAsync(id);
        if (cab is null)
            return NotFound();
        return cab;
    }

    [HttpPost]
    public async Task<ActionResult<Cab>> Create(Cab cab)
    {
        _context.Cabs.Add(cab);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cab.Id }, cab);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Cab cab)
    {
        if (id != cab.Id)
            return BadRequest();

        _context.Entry(cab).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Cabs.AnyAsync(c => c.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cab = await _context.Cabs.FindAsync(id);
        if (cab is null)
            return NotFound();

        _context.Cabs.Remove(cab);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
