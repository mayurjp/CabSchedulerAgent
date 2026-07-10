using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Models;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly CabSchedulerDbContext _context;

    public DriversController(CabSchedulerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Driver>>> GetAll()
    {
        return await _context.Drivers.Include(d => d.AssignedCab).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Driver>> GetById(int id)
    {
        var driver = await _context.Drivers.Include(d => d.AssignedCab).FirstOrDefaultAsync(d => d.Id == id);
        if (driver is null)
            return NotFound();
        return driver;
    }

    [HttpPost]
    public async Task<ActionResult<Driver>> Create(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Driver driver)
    {
        if (id != driver.Id)
            return BadRequest();

        _context.Entry(driver).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Drivers.AnyAsync(d => d.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver is null)
            return NotFound();

        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
