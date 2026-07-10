import { Assignment } from './assignment';

export interface Route {
  id: number;
  name: string;
  cabId: number;
  driverId: number;
  cycle: string;
  createdAt: string;
  cab?: { plateNumber: string; model: string };
  driver?: { name: string };
  assignments: Assignment[];
}
