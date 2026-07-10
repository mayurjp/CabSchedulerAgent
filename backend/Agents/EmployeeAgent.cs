using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using CabScheduler.Api.Data;
using CabScheduler.Api.EventBus;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace CabScheduler.Api.Agents;

public class EmployeeAgent : AIAgent
{
    private readonly CabSchedulerDbContext _context;
    private readonly IEventBus _eventBus;

    public EmployeeAgent(CabSchedulerDbContext context, IEventBus eventBus)
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
        var userMessages = messages
            .Where(m => m.Role == ChatRole.User).ToList();

        if (!userMessages.Any())
            return new AgentResponse(new ChatMessage(
                ChatRole.Assistant, "I'm ready to help with your cab requests. How can I assist you?"));

        var lastMessage = userMessages.Last().Text ?? string.Empty;
        var lower = lastMessage.ToLowerInvariant();

        if (lower.Contains("cancel") || lower.Contains("cancellation"))
            return await HandleCancellationAsync(session, lower);

        if (lower.Contains("status") || lower.Contains("where is") || lower.Contains("eta"))
            return await HandleStatusCheckAsync(session, lower);

        if (lower.Contains("book") || lower.Contains("request") || lower.Contains("pickup"))
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "To book a cab, please use the Request a Cab form with your pickup and drop-off locations. " +
                "I can help with status checks and cancellations."));

        if (lower.Contains("register") || lower.Contains("sign up"))
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "To register as an employee, please use the Register Employee form. " +
                "You'll need your name, email, department, and pickup location coordinates."));

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            "I can help you with cab request status, cancellations, and booking. " +
            "Try asking: 'what is my status?', 'cancel my ride', or 'book a pickup'."));
    }

    private async Task<AgentResponse> HandleCancellationAsync(AgentSession? session, string message)
    {
        var employeeIdStr = ExtractEmployeeId(message);
        if (!int.TryParse(employeeIdStr, out var employeeId))
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "To cancel a ride, I need your Employee ID. Please include it in your message, e.g. 'cancel ride for employee 5'."));

        var pendingRequest = await _context.CabRequests
            .FirstOrDefaultAsync(cr => cr.EmployeeId == employeeId && cr.Status.StartsWith("Assigned"));

        if (pendingRequest is null)
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "No active assigned request found to cancel for your account."));

        pendingRequest.Status = "Cancelled";
        await _context.SaveChangesAsync();

        await _eventBus.PublishAsync("cab.request.cancelled", new CabRequestCancelledEvent(
            pendingRequest.Id, employeeId, "Employee cancelled via chat agent"));

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            $"Request #{pendingRequest.Id} has been cancelled. The adaptive scheduler will re-route if needed."));
    }

    private async Task<AgentResponse> HandleStatusCheckAsync(AgentSession? session, string message)
    {
        var employeeIdStr = ExtractEmployeeId(message);
        if (!int.TryParse(employeeIdStr, out var employeeId))
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "I need your Employee ID to check your status. Please include it, e.g. 'status for employee 5'."));

        var activeRequest = await _context.CabRequests
            .Include(cr => cr.Employee)
            .FirstOrDefaultAsync(cr => cr.EmployeeId == employeeId && cr.Status.StartsWith("Assigned"));

        if (activeRequest is null)
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                "You have no active assigned request at the moment."));

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            $"Request #{activeRequest.Id} — Status: {activeRequest.Status}. " +
            $"Pickup: {activeRequest.PickupLocation}, Drop-off: {activeRequest.DropoffLocation}."));
    }

    private static string ExtractEmployeeId(string message)
    {
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (int.TryParse(words[i], out _) && i + 1 < words.Length)
                return words[i];
        }
        return string.Empty;
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
