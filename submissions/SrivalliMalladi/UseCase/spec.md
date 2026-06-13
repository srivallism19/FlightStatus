# Flight Status Specification

## Assumptions
- Two providers exist: **AeroTrack** and **QuickFlight**.
- Providers are stubbed with deterministic, hardcoded responses (no real API calls).
- Unified status enum must normalize provider vocabularies.
- When both providers respond, prefer the one with the later `lastUpdatedUtc`.
- When only one provider responds, use that result.
- When neither responds, return `Unknown` with an appropriate message.
- Input validation: `flightNumber` and `date` are required; return 400 if missing.
- No real secrets or credentials are committed.

---

## Unified Status Enum
public enum FlightStatus
{
    OnTime,     // Departing/arrived within 15 minutes of schedule
    Delayed,    // Departure or arrival pushed beyond 15 minutes
    Cancelled,  // Flight will not operate
    Diverted,   // Flight landed at a different airport
    Unknown     // Provider returned no usable status
}

## Data Model
public class FlightStatusResult
{
    public string FlightNumber { get; set; }
    public DateTime Date { get; set; }
    public FlightStatus Status { get; set; }
    public DateTime ScheduledDeparture { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public DateTime ScheduledArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public string? Terminal { get; set; }
    public string? Gate { get; set; }
    public string? DelayReason { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
    public string SourceProvider { get; set; }
    public string? Message { get; set; }
}



## Provider Abstraction
public interface IFlightStatusProvider
{
    Task<FlightStatusResult> GetStatusAsync(string flightNumber, DateTime date);
}



## Stub Implementations
AeroTrackProvider

- Returns verbose details: status, scheduled/actual times, terminal, gate, delay reason.

- Uses AeroTrack’s vocabulary → mapped to unified enum.

QuickFlightProvider

- Returns minimal details: status + scheduled times only.

- Faster, but less detail.

- Vocabulary mapped to unified enum.


## API Contract
Endpoint: GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}

Responses:

- 200 OK → Unified FlightStatusResult JSON.

- 400 Bad Request → Missing/invalid input.

- 500 Internal Server Error → Provider failure or unexpected error.


## Frontend Requirements
Search form: flight number input + date picker.

Result card: unified status with color coding:

Green = OnTime

Amber = Delayed

Red = Cancelled/Diverted

Grey = Unknown

AeroTrack-only fields (gate, terminal, delay reason) shown when present, hidden when absent.

Basic error state when API returns an error.


## Testing Scope
- Normalisation rules (mapping provider vocabularies → unified enum).

- Provider selection logic (latest lastUpdatedUtc).

- Input validation (missing flight number/date).

- Fallback to Unknown when no provider responds.


## Deployment & Documentation
- Application must start from a clean clone using README instructions.

- README includes setup steps, assumptions, and Copilot usage summary.

- prompts.md captures all AI prompts used, with notes on accepted/rejected outputs.

- reflection.md documents evaluator feedback, fixes, and improvements.




