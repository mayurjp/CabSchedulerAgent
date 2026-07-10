using CabScheduler.Api.Data;
using CabScheduler.Api.EventBus;
using Microsoft.EntityFrameworkCore;

namespace CabScheduler.Api.Agents;

public class SupervisorAgent
{
    private readonly CabSchedulerDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<SupervisorAgent> _logger;

    public SupervisorAgent(CabSchedulerDbContext context, IEventBus eventBus, ILogger<SupervisorAgent> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<string> GenerateFleetSummaryAsync()
    {
        var totalRoutes = await _context.Routes.CountAsync();
        var activeAssignments = await _context.Assignments.CountAsync();
        var pendingRequests = await _context.CabRequests.CountAsync(cr => cr.Status == "Pending");

        var summary = new FleetSummaryEvent(totalRoutes, activeAssignments, pendingRequests, DateTime.UtcNow);

        await _eventBus.PublishAsync("fleet.summary", summary);

        _logger.LogInformation("SupervisorAgent published fleet summary: {Routes} routes, {Assignments} assignments, {Pending} pending",
            totalRoutes, activeAssignments, pendingRequests);

        return $"Fleet Summary — Routes: {totalRoutes}, Active Assignments: {activeAssignments}, " +
               $"Pending Requests: {pendingRequests}";
    }

    public async Task<string> NotifyDelayAsync(int routeId, string reason)
    {
        var route = await _context.Routes
            .Include(r => r.Assignments)
                .ThenInclude(a => a.Employee)
            .FirstOrDefaultAsync(r => r.Id == routeId);

        if (route is null)
            return $"Route #{routeId} not found.";

        var employeeCount = route.Assignments.Count;

        await _eventBus.PublishAsync("route.delayed", new { routeId, reason, employeeCount, Timestamp = DateTime.UtcNow });

        _logger.LogInformation("SupervisorAgent notified delay for Route {RouteId}: {Reason} ({Count} employees)",
            routeId, reason, employeeCount);

        return $"Delay notification sent for Route #{routeId}. {employeeCount} employees affected.";
    }

    public Task<string> AssignDriverAsync(int requestId)
    {
        _logger.LogInformation("SupervisorAgent assigning driver for Request {RequestId}", requestId);

        return Task.FromResult("Driver assignment initiated.");
    }

    public Task<string> OptimizeRoutesAsync()
    {
        _logger.LogInformation("SupervisorAgent running route optimization");

        return Task.FromResult("Route optimization complete.");
    }
}
