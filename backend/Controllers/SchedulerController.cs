using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
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
        var session = await _schedulerAgent.CreateSessionAsync();
        var response = await _schedulerAgent.RunAsync(
            new List<ChatMessage> { new(ChatRole.User, "run morning") }, session);
        var text = response.Messages.LastOrDefault()?.Text ?? "Scheduler completed.";
        return Ok(new { message = text });
    }

    [HttpPost("run-evening")]
    public async Task<IActionResult> RunEvening()
    {
        var session = await _schedulerAgent.CreateSessionAsync();
        var response = await _schedulerAgent.RunAsync(
            new List<ChatMessage> { new(ChatRole.User, "run evening") }, session);
        var text = response.Messages.LastOrDefault()?.Text ?? "Scheduler completed.";
        return Ok(new { message = text });
    }

    [HttpPost("cancel/{requestId}")]
    public async Task<IActionResult> CancelAndReroute(int requestId, [FromBody] CancelRequest? body)
    {
        var reason = body?.Reason ?? "Cancelled by supervisor";
        var session = await _adaptiveSchedulerAgent.CreateSessionAsync();
        var response = await _adaptiveSchedulerAgent.RunAsync(
            new List<ChatMessage> { new(ChatRole.User, $"cancel request {requestId} reason {reason}") }, session);
        var text = response.Messages.LastOrDefault()?.Text ?? "Cancellation processed.";
        return Ok(new { message = text });
    }
}

public class CancelRequest
{
    public string Reason { get; set; } = string.Empty;
}
