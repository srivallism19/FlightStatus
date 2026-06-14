export interface FlightStatusResult {
  flightNumber: string;
  date: Date;
  status: string;
  scheduledDepartureUtc?: Date | null;
  actualDepartureUtc?: Date | null;
  scheduledArrivalUtc?: Date | null;
  actualArrivalUtc?: Date | null;
  terminal?: string | null;
  gate?: string | null;
  delayReason?: string | null;
  lastUpdatedUtc?: Date | null;
  sourceProvider?: string | null;
  message?: string | null;
}
