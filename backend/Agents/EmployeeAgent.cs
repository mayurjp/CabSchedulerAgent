namespace CabScheduler.Api.Agents;

public class EmployeeAgent
{
    private readonly ILogger<EmployeeAgent> _logger;

    public EmployeeAgent(ILogger<EmployeeAgent> logger)
    {
        _logger = logger;
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
