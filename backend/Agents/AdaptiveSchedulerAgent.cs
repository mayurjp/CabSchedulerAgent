using CabScheduler.Api.Data;
using CabScheduler.Api.EventBus;
using CabScheduler.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CabScheduler.Api.Agents;

public class AdaptiveSchedulerAgent
{
    private readonly CabSchedulerDbContext _context;
    private readonly SchedulingEngine _schedulingEngine;
    private readonly IEventBus _eventBus;
    private readonly ILogger<AdaptiveSchedulerAgent> _logger;

    public AdaptiveSchedulerAgent(
        CabSchedulerDbContext context,
        SchedulingEngine schedulingEngine,
        IEventBus eventBus,
        ILogger<AdaptiveSchedulerAgent> logger)
    {
        _context = context;
        _schedulingEngine = schedulingEngine;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        await _eventBus.SubscribeAsync<CabRequestCancelledEvent>("cab.request.cancelled", HandleCancellationAsync);
        _logger.LogInformation("AdaptiveSchedulerAgent initialized and listening for events");
    }

    private async Task HandleCancellationAsync(CabRequestCancelledEvent evt)
    {
        _logger.LogInformation("AdaptiveSchedulerAgent handling cancellation for Request {RequestId}", evt.RequestId);

        var assignment = await _context.Assignments
            .Include(a => a.Route)
            .FirstOrDefaultAsync(a => a.CabRequestId == evt.RequestId);

        if (assignment is null)
        {
            _logger.LogWarning("No assignment found for cancelled request {RequestId}", evt.RequestId);
            return;
        }

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();

        var route = assignment.Route;
        var remainingCount = await _context.Assignments.CountAsync(a => a.RouteId == route!.Id);

        if (remainingCount == 0)
        {
            _context.Routes.Remove(route!);
            _logger.LogInformation("Route {RouteId} removed — no remaining assignments", route!.Id);
        }
        else
        {
            _logger.LogInformation("Route {RouteId} now has {Count} remaining assignments after cancellation",
                route!.Id, remainingCount);
        }

        await _context.SaveChangesAsync();

        await _eventBus.PublishAsync("route.updated", new RouteUpdatedEvent(
            route!.Id, route.Cycle, remainingCount));

        _logger.LogInformation("AdaptiveSchedulerAgent re-routing complete for Request {RequestId}", evt.RequestId);
    }

    public async Task<string> CancelAndRerouteAsync(int requestId, string reason)
    {
        var request = await _context.CabRequests.FindAsync(requestId);
        if (request is null)
            return $"Request #{requestId} not found.";

        request.Status = "Cancelled";
        await _context.SaveChangesAsync();

        await _eventBus.PublishAsync("cab.request.cancelled", new CabRequestCancelledEvent(
            requestId, request.EmployeeId, reason));

        return $"Request #{requestId} cancelled. Dynamic re-routing initiated.";
    }
}
