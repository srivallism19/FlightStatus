# FlightStatus
to fetch the flight status based on Flight number and Date

## 1. Analysis

- Problem statement: unify flight status from multiple providers into a single canonical API response.
- Requirements: Minimal API with query parameters (flightNumber, date), normalized statuses across providers, Swagger UI for exploration, JSON stubs for provider data, and test coverage.

## 2. Architecture

- Solution layout:
  - FlightStatus.Api (Minimal API backend)
  - FlightStatus.Tests (xUnit test project)
  - FlightStatus.UI (Angular frontend app)
- Folders inside FlightStatus.Api: Models, Interfaces, Providers, Services, Endpoints, Middleware, Data
- Frontend: Angular UI is included under `FlightStatus.UI` and provides the standalone app, Material-based search form, result card, service, and tests.

## 3. Design

- DTOs and enums:
  - FlightStatusResult: canonical response model containing flight number, date, scheduled/actual times, terminal, gate, delay reason, lastUpdatedUtc, sourceProvider, message, status
  - UnifiedStatus: enum { OnTime, Delayed, Cancelled, Diverted, Unknown }
- Interfaces:
  - IFlightStatusProvider: provider abstraction that returns FlightStatusResult or null
  - IFlightStatusService: service performing provider calls and selection
- Providers:
  - AeroTrackProvider and QuickFlightProvider (stubbed, read from JSON files under Data/)
- Service selection logic: prefer provider response with latest LastUpdatedUtc; tie-breaker uses provider priority.
- Middleware: global error handling middleware to convert exceptions to friendly 500 responses.

## 4. Develop

- Implemented endpoints: GET /flights/status?flightNumber={code}&date={yyyy-MM-dd} (see src/FlightStatus.Api/Endpoints)
- Providers read stub JSON from src/FlightStatus.Api/Data and normalize provider-specific status strings to UnifiedStatus.
- FlightStatusService performs normalization, parallel provider calls, selection and fallback to Unknown when no provider data available.
- LoggingExtensions helper logs provider selection decisions.
- Swagger/OpenAPI integration is configured for easy testing in development.

## 5. Test

- The FlightStatus.Tests project contains unit and integration tests:
  - Endpoints: FlightStatusEndpointTests (missing params, invalid date, valid request), HealthEndpointTests
  - Providers: MapStatus normalization tests for provider vocabularies
  - Services: FlightStatusServiceTests verifying selection logic and Unknown fallback
  - Middleware: ErrorHandlingMiddlewareTests verifying exception -> 500 responses
- Moq is used to mock IFlightStatusService and IFlightStatusProvider in tests.
- System.Text.Json is configured with JsonStringEnumConverter (and a custom case-insensitive converter) to support enum serialization from/to strings.

## 6. Deploy / Document

- Run API locally:
  - cd submissions\SrivalliMalladi\UseCase\FlightStatus.Api
  - dotnet run
- Run tests:
  - dotnet test FlightStatus.Tests/FlightStatus.Tests.csproj
- Example request:
  - GET /flights/status?flightNumber=AB123&date=2026-06-13
- Example response:
  - { "flightNumber": "AB123", "date": "2026-06-13", "status": "OnTime", "sourceProvider": "AeroTrack" }

## 7. Frontend UI Setup

The Angular UI is located in `FlightStatus.UI` and can be started locally after cloning the repository.

1. Open a terminal and navigate to the UI folder:
   - `cd submissions\SrivalliMalladi\UseCase\FlightStatus.UI`
2. Install dependencies:
   - `npm install`
3. Start the Angular dev server:
   - `npm run start`
4. Open the browser:
   - `http://localhost:4200`

If you want to verify tests for the UI project:

- `npm run test -- --watch=false`

If you want to produce a production build:

- `npm run build`

> Note: The UI expects the backend API to be available at `https://localhost:44380` by default. If you are running the backend locally, start the API first with `dotnet run` from `FlightStatus.Api`.

---
This README provides a concise SDLC view for the FlightStatus application. See the src/ folder for code and tests and the Data/ folder for JSON stubs.
