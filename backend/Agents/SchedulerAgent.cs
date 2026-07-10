using CabScheduler.Api.Services;

namespace CabScheduler.Api.Agents;

public class SchedulerAgent
{
    private readonly SchedulingEngine _engine;
    private readonly ILogger<SchedulerAgent> _logger;

    public SchedulerAgent(SchedulingEngine engine, ILogger<SchedulerAgent> logger)
    {
        _engine = engine;
        _logger = logger;
    }

    public async Task<string> OrchestrateMorningCycleAsync()
    {
        _logger.LogInformation("SchedulerAgent orchestrating morning pickup cycle");

        var routes = await _engine.RunMorningPickupAsync();

        if (!routes.Any())
            return "No routes created. No pending requests or no available cabs/drivers.";

        return $"Morning cycle complete. Created {routes.Count} routes " +
               $"covering {routes.Sum(r => r.Assignments.Count)} employees.";
    }

    public async Task<string> OrchestrateEveningCycleAsync()
    {
        _logger.LogInformation("SchedulerAgent orchestrating evening drop cycle");

        var routes = await _engine.RunEveningDropAsync();

        if (!routes.Any())
            return "No routes created. No pending requests or no available cabs/drivers.";

        return $"Evening cycle complete. Created {routes.Count} routes " +
               $"covering {routes.Sum(r => r.Assignments.Count)} employees.";
    }
}
