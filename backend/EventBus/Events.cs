namespace CabScheduler.Api.EventBus;

public record CabRequestCancelledEvent(int RequestId, int EmployeeId, string Reason);

public record RouteUpdatedEvent(int RouteId, string Cycle, int EmployeeCount);

public record FleetSummaryEvent(int TotalRoutes, int ActiveAssignments, int PendingRequests, DateTime Timestamp);
