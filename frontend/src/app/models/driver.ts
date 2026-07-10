import { Cab } from './cab';

export interface Driver {
  id: number;
  name: string;
  licenseNumber: string;
  cabId: number | null;
  assignedCab?: Cab;
}
