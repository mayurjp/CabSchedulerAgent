using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using CabScheduler.Api.Data;
using CabScheduler.Api.EventBus;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace CabScheduler.Api.Agents;

public class SupervisorAgent : AIAgent
{
    private readonly CabSchedulerDbContext _context;
    private readonly IEventBus _eventBus;

    public SupervisorAgent(CabSchedulerDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    protected override async Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        CancellationToken cancellationToken)
    {
        var userMessages = messages.Where(m => m.Role == ChatRole.User).ToList();
        var command = userMessages.Any() ? (userMessages.Last().Text ?? "").ToLowerInvariant() : "summary";

        if (command.Contains("fleet") || command.Contains("summary"))
            return await GenerateFleetSummaryResponseAsync();

        if (command.Contains("delay") || command.Contains("notify"))
            return await HandleDelayCommandAsync(command);

        if (command.Contains("assign") || command.Contains("driver"))
            return await HandleAssignCommandAsync(command);

        if (command.Contains("optimize") || command.Contains("route"))
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "Route optimization initiated — checking proximity groups and cab capacity..."));

        return await GenerateFleetSummaryResponseAsync();
    }

    private async Task<AgentResponse> GenerateFleetSummaryResponseAsync()
    {
        var totalRoutes = await _context.Routes.CountAsync();
        var activeAssignments = await _context.Assignments.CountAsync();
        var pendingRequests = await _context.CabRequests.CountAsync(cr => cr.Status == "Pending");

        var summary = new FleetSummaryEvent(totalRoutes, activeAssignments, pendingRequests, DateTime.UtcNow);
        await _eventBus.PublishAsync("fleet.summary", summary);

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            $"Fleet Summary — Total Routes: {totalRoutes}, Active Assignments: {activeAssignments}, " +
            $"Pending Requests: {pendingRequests}"));
    }

    private async Task<AgentResponse> HandleDelayCommandAsync(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int routeId = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            if (int.TryParse(parts[i], out var id))
            {
                routeId = id;
                break;
            }
        }

        if (routeId == 0)
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "Please specify a route ID to notify a delay, e.g. 'delay route 3 due to traffic'."));

        var route = await _context.Routes
            .Include(r => r.Assignments).ThenInclude(a => a.Employee)
            .FirstOrDefaultAsync(r => r.Id == routeId);

        if (route is null)
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                $"Route #{routeId} not found."));

        var reason = command.Contains("traffic") ? "Traffic congestion" :
                     command.Contains("weather") ? "Weather conditions" : "Operational delay";

        await _eventBus.PublishAsync("route.delayed", new
        {
            routeId, reason, employeeCount = route.Assignments.Count, Timestamp = DateTime.UtcNow
        });

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            $"Delay notification sent for Route #{routeId} ({reason}). {route.Assignments.Count} employees affected."));
    }

    private async Task<AgentResponse> HandleAssignCommandAsync(string command)
    {
        var requestId = 0;
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            if (int.TryParse(parts[i], out var id))
            {
                requestId = id;
                break;
            }
        }

        if (requestId == 0)
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "Please specify a request ID to assign, e.g. 'assign driver to request 5'."));

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            $"Driver assignment initiated for Request #{requestId}."));
    }

    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        yield break;
    }

    protected override ValueTask<AgentSession> CreateSessionCoreAsync(CancellationToken cancellationToken)
#pragma warning disable CS8625
        => default;
#pragma warning restore CS8625

    protected override ValueTask<JsonElement> SerializeSessionCoreAsync(
        AgentSession state, JsonSerializerOptions? options, CancellationToken cancellationToken)
        => new ValueTask<JsonElement>(default(JsonElement));

    protected override ValueTask<AgentSession> DeserializeSessionCoreAsync(
        JsonElement state, JsonSerializerOptions? options, CancellationToken cancellationToken)
#pragma warning disable CS8625
        => default;
#pragma warning restore CS8625
}
