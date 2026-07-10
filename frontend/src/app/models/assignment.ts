export interface Assignment {
  id: number;
  routeId: number;
  employeeId: number;
  cabRequestId: number;
  employee?: { name: string; department: string };
  cabRequest?: { pickupLocation: string; dropoffLocation: string };
}
