namespace CabScheduler.Api.Agents;

public class SupervisorAgent
{
    private readonly ILogger<SupervisorAgent> _logger;

    public SupervisorAgent(ILogger<SupervisorAgent> logger)
    {
        _logger = logger;
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
