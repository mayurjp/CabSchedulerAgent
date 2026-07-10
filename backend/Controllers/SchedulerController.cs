using Microsoft.AspNetCore.Mvc;
using CabScheduler.Api.Agents;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulerController : ControllerBase
{
    private readonly SchedulerAgent _schedulerAgent;

    public SchedulerController(SchedulerAgent schedulerAgent)
    {
        _schedulerAgent = schedulerAgent;
    }

    [HttpPost("run-morning")]
    public async Task<IActionResult> RunMorning()
    {
        var result = await _schedulerAgent.OrchestrateMorningCycleAsync();
        return Ok(new { message = result });
    }

    [HttpPost("run-evening")]
    public async Task<IActionResult> RunEvening()
    {
        var result = await _schedulerAgent.OrchestrateEveningCycleAsync();
        return Ok(new { message = result });
    }
}
