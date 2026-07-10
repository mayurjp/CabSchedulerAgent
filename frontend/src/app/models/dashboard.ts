export interface SupervisorDashboard {
  summary: {
    totalEmployees: number;
    totalCabs: number;
    totalDrivers: number;
    pendingRequests: number;
    activeRoutes: number;
    todayAssignments: number;
  };
  recentRoutes: {
    id: number;
    name: string;
    cycle: string;
    cabPlate: string;
    driverName: string;
    employeeCount: number;
    createdAt: string;
  }[];
}
