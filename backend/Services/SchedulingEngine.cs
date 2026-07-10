using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Models;
using RouteEntity = CabScheduler.Api.Models.Route;

namespace CabScheduler.Api.Services;

public class SchedulingEngine
{
    private readonly CabSchedulerDbContext _context;
    private readonly ILogger<SchedulingEngine> _logger;
    private const double ProximityThresholdKm = 5.0;

    public SchedulingEngine(CabSchedulerDbContext context, ILogger<SchedulingEngine> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RouteEntity>> RunMorningPickupAsync()
    {
        _logger.LogInformation("Starting morning pickup cycle");
        return await ScheduleAsync("Morning");
    }

    public async Task<List<RouteEntity>> RunEveningDropAsync()
    {
        _logger.LogInformation("Starting evening drop cycle");
        return await ScheduleAsync("Evening");
    }

    private async Task<List<RouteEntity>> ScheduleAsync(string cycle)
    {
        var pendingRequests = await _context.CabRequests
            .Include(cr => cr.Employee)
            .Where(cr => cr.Status == "Pending")
            .ToListAsync();

        if (!pendingRequests.Any())
        {
            _logger.LogInformation("No pending cab requests for {Cycle} cycle", cycle);
            return new List<RouteEntity>();
        }

        var availableCabs = await _context.Cabs.ToListAsync();
        var availableDrivers = await _context.Drivers.ToListAsync();

        if (!availableCabs.Any() || !availableDrivers.Any())
        {
            _logger.LogWarning("No available cabs or drivers for {Cycle} cycle", cycle);
            return new List<RouteEntity>();
        }

        var groups = GroupByProximity(pendingRequests);

        var routes = new List<RouteEntity>();
        int cabIndex = 0;
        int driverIndex = 0;

        foreach (var group in groups)
        {
            var cab = availableCabs[cabIndex % availableCabs.Count];
            var driver = availableDrivers[driverIndex % availableDrivers.Count];

            if (group.Count > cab.Capacity)
            {
                _logger.LogWarning("Group size {Size} exceeds cab {Cab} capacity {Capacity}. Splitting.",
                    group.Count, cab.PlateNumber, cab.Capacity);
            }

            var route = new RouteEntity
            {
                Name = $"{cycle}-Route-{routes.Count + 1}",
                CabId = cab.Id,
                DriverId = driver.Id,
                Cycle = cycle,
                CreatedAt = DateTime.UtcNow
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            foreach (var request in group)
            {
                var assignment = new Assignment
                {
                    RouteId = route.Id,
                    EmployeeId = request.EmployeeId,
                    CabRequestId = request.Id
                };

                _context.Assignments.Add(assignment);

                request.Status = cycle == "Morning" ? "Assigned-Pickup" : "Assigned-Drop";
                _context.Entry(request).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            route.Assignments = await _context.Assignments
                .Include(a => a.Employee)
                .Include(a => a.CabRequest)
                .Where(a => a.RouteId == route.Id)
                .ToListAsync();

            routes.Add(route);

            cabIndex++;
            driverIndex++;
        }

        _logger.LogInformation("{Cycle} cycle complete. Created {Count} routes for {Total} requests",
            cycle, routes.Count, pendingRequests.Count);

        return routes;
    }

    private List<List<CabRequest>> GroupByProximity(List<CabRequest> requests)
    {
        var groups = new List<List<CabRequest>>();
        var remaining = new HashSet<CabRequest>(requests);

        while (remaining.Any())
        {
            var seed = remaining.First();
            remaining.Remove(seed);

            var group = new List<CabRequest> { seed };

            var nearby = remaining
                .Where(r => DistanceKm(
                    seed.Employee!.PickupLat, seed.Employee.PickupLng,
                    r.Employee!.PickupLat, r.Employee.PickupLng) <= ProximityThresholdKm)
                .ToList();

            foreach (var n in nearby)
            {
                group.Add(n);
                remaining.Remove(n);
            }

            groups.Add(group);
        }

        return groups;
    }

    private static double DistanceKm(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLng = ToRad(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double degrees) => degrees * Math.PI / 180;
}
