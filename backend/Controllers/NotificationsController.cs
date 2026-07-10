using Microsoft.AspNetCore.Mvc;
using CabScheduler.Api.Agents;
using CabScheduler.Api.Services;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly SupervisorAgent _supervisorAgent;
    private readonly NotificationService _notificationService;

    public NotificationsController(SupervisorAgent supervisorAgent, NotificationService notificationService)
    {
        _supervisorAgent = supervisorAgent;
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
    {
        var notification = await _notificationService.SendAsync(request.Recipient, request.Channel, request.Message);
        return Ok(notification);
    }

    [HttpGet("fleet-summary")]
    public async Task<IActionResult> GetFleetSummary()
    {
        var summary = await _supervisorAgent.GenerateFleetSummaryAsync();
        return Ok(new { summary });
    }

    [HttpPost("delay/{routeId}")]
    public async Task<IActionResult> NotifyDelay(int routeId, [FromBody] DelayRequest request)
    {
        var result = await _supervisorAgent.NotifyDelayAsync(routeId, request.Reason);
        return Ok(new { message = result });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? channel)
    {
        var notifications = await _notificationService.GetNotificationsAsync(channel);
        return Ok(notifications);
    }
}

public class SendNotificationRequest
{
    public string Recipient { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class DelayRequest
{
    public string Reason { get; set; } = string.Empty;
}
