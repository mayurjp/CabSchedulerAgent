using CabScheduler.Api.Data;
using CabScheduler.Api.EventBus;
using Microsoft.EntityFrameworkCore;

namespace CabScheduler.Api.Agents;

public class EmployeeAgent
{
    private readonly CabSchedulerDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<EmployeeAgent> _logger;

    public EmployeeAgent(CabSchedulerDbContext context, IEventBus eventBus, ILogger<EmployeeAgent> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<string> ProcessChatMessageAsync(int employeeId, string message)
    {
        _logger.LogInformation("EmployeeAgent received chat from Employee {Id}: {Message}", employeeId, message);

        var lower = message.ToLowerInvariant();

        if (lower.Contains("cancel") || lower.Contains("cancellation"))
        {
            var pendingRequest = await _context.CabRequests
                .FirstOrDefaultAsync(cr => cr.EmployeeId == employeeId && cr.Status.StartsWith("Assigned"));

            if (pendingRequest is null)
                return "No active assigned request found to cancel.";

            pendingRequest.Status = "Cancelled";
            await _context.SaveChangesAsync();

            await _eventBus.PublishAsync("cab.request.cancelled", new CabRequestCancelledEvent(
                pendingRequest.Id, employeeId, "Employee cancelled via chat"));

            return $"Request #{pendingRequest.Id} has been cancelled. The scheduler will re-route if needed.";
        }

        if (lower.Contains("status") || lower.Contains("where is") || lower.Contains("eta"))
        {
            var activeRequest = await _context.CabRequests
                .Include(cr => cr.Employee)
                .FirstOrDefaultAsync(cr => cr.EmployeeId == employeeId && cr.Status.StartsWith("Assigned"));

            if (activeRequest is null)
                return "You have no active assigned request at the moment.";

            return $"Request #{activeRequest.Id} is currently {activeRequest.Status}. " +
                   $"Pickup: {activeRequest.PickupLocation}, Drop-off: {activeRequest.DropoffLocation}.";
        }

        if (lower.Contains("book") || lower.Contains("request") || lower.Contains("pickup"))
        {
            return "To book a cab, please use the Request a Cab form with your pickup and drop-off locations. " +
                   "I can help with status checks and cancellations.";
        }

        return "I can help you with cab request status, cancellations, and booking. " +
               "Try asking: 'what is my status?', 'cancel my ride', or 'book a pickup'.";
    }

    public Task<string> ProcessCabRequestAsync(int employeeId, string pickup, string dropoff)
    {
        _logger.LogInformation("EmployeeAgent processing cab request for Employee {EmployeeId}: {Pickup} -> {Dropoff}",
            employeeId, pickup, dropoff);

        return Task.FromResult("Cab request submitted for scheduling.");
    }

    public Task<string> CheckRequestStatusAsync(int requestId)
    {
        _logger.LogInformation("EmployeeAgent checking status for Request {RequestId}", requestId);

        return Task.FromResult("Request status check complete.");
    }
}
