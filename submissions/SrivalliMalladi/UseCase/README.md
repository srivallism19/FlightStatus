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

## 5. Test cases

- The FlightStatus.Tests project contains unit and integration tests:
  - Endpoints: FlightStatusEndpointTests (missing params, invalid date, valid request), HealthEndpointTests
  - Providers: MapStatus normalization tests for provider vocabularies
  - Services: FlightStatusServiceTests verifying selection logic and Unknown fallback
  - Middleware: ErrorHandlingMiddlewareTests verifying exception -> 500 responses
- Moq is used to mock IFlightStatusService and IFlightStatusProvider in tests.
- System.Text.Json is configured with JsonStringEnumConverter (and a custom case-insensitive converter) to support enum serialization from/to strings.

## 6. Testing and running the API

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

### Local setup and run

1. Open a terminal and navigate to the UI folder:
   - `cd submissions\SrivalliMalladi\UseCase\FlightStatus.UI`
2. Install dependencies:
   - `npm install`
3. Build the production app:
   - `npm run build`
4. Start the local production server:
   - `npm start`
5. Open the browser:
   - `http://localhost:8080`

The local production server uses `server.js` to serve the built SPA from `dist/FlightStatus.UI/browser`.

### Tests and build

- Run unit tests:
  - `npm run test -- --watch=false`
- Produce a production build:
  - `npm run build`

### Azure App Service deployment

Deploy the entire `FlightStatus.UI` folder, not only `dist`.
The deployed folder must include:

- `package.json`
- `server.js`
- `angular.json`
- `src/`
- `dist/FlightStatus.UI/browser`

Azure settings:

- OS: **Linux**
- Runtime stack: **Node 22 LTS**
- Startup command: `npm start`

After deployment completes, open the app URL and verify that the Angular SPA loads. If it does not load, confirm the startup command and ensure `dist/FlightStatus.UI/browser` exists.

> Note: The UI expects the backend API to be available at `https://localhost:44380` by default when running locally. If you are running the backend locally, start the API first with `dotnet run` from `FlightStatus.Api`.
Currently the backend is deployed to Azure at: `https://flightstatus-backend.azurewebsites.net/` and the UI can be configured to use that endpoint in `src/environments/environment.ts`.


## 8. Azure Deployments -  FrontEnd and BackEnd

- UI application is deployed to Azure App Service at: `https://flightstatus-app.azurewebsites.net/` by doing code deployment from the `FlightStatus.UI` folder.
- Backend API is deployed to Azure App Service at: `https://flightstatus-backend.azurewebsites.net/` by doing code publish from visual studio from the `FlightStatus.Api` folder.

- For test data, refer to the `Data/` folder which contains JSON stubs simulating provider responses. These can be used for local testing or extended for integration tests. 
- File names and structures in the `Data/` folder correspond to the expected provider responses and are read by the respective provider implementations of AeroTrack and QuickFlight.
---

This README provides a concise SDLC view for the FlightStatus application. See the src/ folder for code and tests and the Data/ folder for JSON stubs.
