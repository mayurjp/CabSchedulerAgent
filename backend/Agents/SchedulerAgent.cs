using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using CabScheduler.Api.Services;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace CabScheduler.Api.Agents;

public class SchedulerAgent : AIAgent
{
    private readonly SchedulingEngine _engine;

    public SchedulerAgent(SchedulingEngine engine)
    {
        _engine = engine;
    }

    protected override async Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        CancellationToken cancellationToken)
    {
        var userMessages = messages.Where(m => m.Role == ChatRole.User).ToList();
        var command = userMessages.Any() ? (userMessages.Last().Text ?? "").ToLowerInvariant() : "";

        if (command.Contains("morning") || command.Contains("pickup"))
            return await OrchestrateCycleAsync("Morning");

        if (command.Contains("evening") || command.Contains("drop"))
            return await OrchestrateCycleAsync("Evening");

        if (command.Contains("both") || command.Contains("all"))
        {
            var morningResult = await _engine.RunMorningPickupAsync();
            var eveningResult = await _engine.RunEveningDropAsync();
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                $"Scheduler: Morning cycle — {morningResult.Count} routes created. " +
                $"Evening cycle — {eveningResult.Count} routes created."));
        }

        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            "I orchestrate the scheduling engine. Use 'run morning', 'run evening', or 'run both' to execute cycles."));
    }

    private async Task<AgentResponse> OrchestrateCycleAsync(string cycle)
    {
        var routes = cycle == "Morning"
            ? await _engine.RunMorningPickupAsync()
            : await _engine.RunEveningDropAsync();

        if (!routes.Any())
            return new AgentResponse(new ChatMessage(ChatRole.Assistant,
                $"No routes created for {cycle} cycle. No pending requests or no available cabs/drivers."));

        var totalEmployees = routes.Sum(r => r.Assignments.Count);
        return new AgentResponse(new ChatMessage(ChatRole.Assistant,
            $"{cycle} cycle complete. Created {routes.Count} routes covering {totalEmployees} employees."));
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
