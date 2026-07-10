using Microsoft.AspNetCore.Mvc;
using CabScheduler.Api.Agents;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulerController : ControllerBase
{
    private readonly SchedulerAgent _schedulerAgent;
    private readonly AdaptiveSchedulerAgent _adaptiveSchedulerAgent;

    public SchedulerController(SchedulerAgent schedulerAgent, AdaptiveSchedulerAgent adaptiveSchedulerAgent)
    {
        _schedulerAgent = schedulerAgent;
        _adaptiveSchedulerAgent = adaptiveSchedulerAgent;
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

    [HttpPost("cancel/{requestId}")]
    public async Task<IActionResult> CancelAndReroute(int requestId, [FromBody] CancelRequest? body)
    {
        var reason = body?.Reason ?? "Cancelled by supervisor";
        var result = await _adaptiveSchedulerAgent.CancelAndRerouteAsync(requestId, reason);
        return Ok(new { message = result });
    }
}

public class CancelRequest
{
    public string Reason { get; set; } = string.Empty;
}
