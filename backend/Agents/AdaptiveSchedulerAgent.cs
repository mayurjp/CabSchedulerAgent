using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using CabScheduler.Api.Data;
using CabScheduler.Api.EventBus;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace CabScheduler.Api.Agents;

public class AdaptiveSchedulerAgent : AIAgent
{
    private readonly CabSchedulerDbContext _context;
    private readonly IEventBus _eventBus;

    public AdaptiveSchedulerAgent(CabSchedulerDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task InitializeAsync()
    {
        await _eventBus.SubscribeAsync<CabRequestCancelledEvent>(
            "cab.request.cancelled", HandleCancellationAsync);
    }

    private async Task HandleCancellationAsync(CabRequestCancelledEvent evt)
    {
        var assignment = await _context.Assignments
            .Include(a => a.Route)
            .FirstOrDefaultAsync(a => a.CabRequestId == evt.RequestId);

        if (assignment is null)
            return;

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();

        var route = assignment.Route;
        var remainingCount = await _context.Assignments.CountAsync(a => a.RouteId == route!.Id);

        if (remainingCount == 0)
        {
            _context.Routes.Remove(route!);
            await _context.SaveChangesAsync();
            await _eventBus.PublishAsync("route.updated",
                new RouteUpdatedEvent(route!.Id, route.Cycle, 0));
        }
        else
        {
            await _eventBus.PublishAsync("route.updated",
                new RouteUpdatedEvent(route!.Id, route.Cycle, remainingCount));
        }
    }

    protected override async Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        CancellationToken cancellationToken)
    {
        var userMessages = messages.Where(m => m.Role == ChatRole.User).ToList();
        var command = userMessages.Any() ? (userMessages.Last().Text ?? "").ToLowerInvariant() : "";

        if (command.Contains("cancel") || command.Contains("reroute"))
            return await HandleCancelRequestAsync(command);

        if (command.Contains("status") || command.Contains("listening"))
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "AdaptiveSchedulerAgent is active and listening for cab.request.cancelled events. " +
                "Use 'cancel request {id}' to trigger a re-route."));

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            "I handle dynamic re-routing when cancellations occur. " +
            "I'm subscribed to cancellation events from the EmployeeAgent and SchedulerController."));
    }

    private async Task<AgentResponse> HandleCancelRequestAsync(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int requestId = 0;
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
                "Please specify a request ID to cancel, e.g. 'cancel request 7'."));

        var request = await _context.CabRequests.FindAsync(requestId);
        if (request is null)
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                $"Request #{requestId} not found."));

        request.Status = "Cancelled";
        await _context.SaveChangesAsync();

        await _eventBus.PublishAsync("cab.request.cancelled",
            new CabRequestCancelledEvent(requestId, request.EmployeeId, "Cancelled by adaptive scheduler"));

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            $"Request #{requestId} cancelled. Dynamic re-routing initiated."));
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
