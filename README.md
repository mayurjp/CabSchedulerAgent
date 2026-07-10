# Cab Scheduler

Employee cab booking and scheduling system with agentic AI orchestration.

---

## Architecture Overview

```
+---------------------------+     +---------------------------+
|     Angular 18 (Frontend) |     |  ASP.NET Core 10 (Backend)|
|                           |     |                           |
|  /                  Home  |     |  Controllers (10)         |
|  /register   Employee Reg |     |  Agents      (4)          |
|  /request-cab   Cab Req   |--> |  Services    (2)          |
|  /chat         Chat Agent |     |  EventBus    (3)          |
|  /dashboard  Supv Dash    |     |  Models      (7)          |
|                           |     |  DbContext   (1)          |
+---------------------------+     +------------+--------------+
            |   proxy /api                |
            +-----------------------------+---> SQL Express
                                               (EF Core)
```

The system follows a **multi-agent architecture**. Four AI agents coordinate via an event bus to handle scheduling, rerouting, notifications, and employee communication. The Angular frontend communicates with the ASP.NET Core Web API through REST endpoints, with an API proxy during development.

---

## Version History

| Version | Theme | Highlights |
|---------|-------|------------|
| **v0.1** | Foundation | Employee registration, cab requests, CRUD APIs, agent stubs |
| **v0.2** | Scheduling Engine | Proximity-based route grouping, morning/evening cycles, supervisor dashboard |
| **v0.3** | Agentic AI + Events | Chat agent, adaptive re-routing, event bus, notification service |

---

## Feature Matrix

| Feature | v0.1 | v0.2 | v0.3 |
|---------|:----:|:----:|:----:|
| Employee Registration | &check; | &check; | &check; |
| Cab Request Submission | &check; | &check; | &check; |
| REST API (CRUD: Employees, Cabs, Drivers) | &check; | &check; | &check; |
| SQL Express / EF Core Persistence | &check; | &check; | &check; |
| Agent Framework Stubs (Employee, Supervisor) | &check; | &check; | &check; |
| Automated Scheduling Engine | | &check; | &check; |
| Proximity-Based Route Grouping (5km) | | &check; | &check; |
| Morning Pickup & Evening Drop Cycles | | &check; | &check; |
| Supervisor Dashboard | | &check; | &check; |
| SchedulerAgent for Route Orchestration | | &check; | &check; |
| Chat Agent for Employee Interaction | | | &check; |
| Fleet Summary Notifications | | | &check; |
| Adaptive Re-routing (Cancellations) | | | &check; |
| Event Bus (Pub/Sub, Kafka/RabbitMQ stub) | | | &check; |
| Notification Service | | | &check; |

---

## Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 18 | Frontend SPA with standalone components |
| ASP.NET Core | 10 | Backend Web API |
| Entity Framework Core | 10 | ORM for SQL Express |
| SQL Server Express | Latest | Relational database |
| Microsoft Agent Framework | Latest | Agent-based AI orchestration |
| Kafka / RabbitMQ | Latest | Event-driven messaging (stub in v0.3) |

---

## Project Structure

```
CabSchedulerAgent/
├── README.md
├── .gitignore
│
├── backend/                              # ASP.NET Core 10 Web API
│   ├── CabScheduler.Api.csproj
│   ├── Program.cs                        # App entry point & DI
│   ├── appsettings.json                  # Connection strings & config
│   ├── Properties/
│   │   └── launchSettings.json
│   │
│   ├── Models/                           # Entity Framework entities
│   │   ├── Employee.cs
│   │   ├── Cab.cs
│   │   ├── Driver.cs
│   │   ├── CabRequest.cs
│   │   ├── Route.cs
│   │   ├── Assignment.cs
│   │   └── Notification.cs
│   │
│   ├── Data/
│   │   └── CabSchedulerDbContext.cs      # EF Core DbContext
│   │
│   ├── Controllers/                      # REST API endpoints
│   │   ├── EmployeesController.cs        # CRUD for employees
│   │   ├── CabsController.cs             # CRUD for cabs
│   │   ├── DriversController.cs          # CRUD for drivers
│   │   ├── CabRequestsController.cs      # Submit & manage cab requests
│   │   ├── RoutesController.cs           # View route details
│   │   ├── AssignmentsController.cs      # View assignments
│   │   ├── SchedulerController.cs        # Run morning/evening cycles, cancel
│   │   ├── ChatController.cs             # Employee chat agent endpoint
│   │   ├── NotificationsController.cs    # Send & query notifications
│   │   └── SupervisorController.cs       # Dashboard summary data
│   │
│   ├── Agents/                           # Microsoft Agent Framework agents
│   │   ├── EmployeeAgent.cs              # Employee-facing chat & requests
│   │   ├── SupervisorAgent.cs            # Fleet summary & delay notifications
│   │   ├── SchedulerAgent.cs             # Schedule orchestration
│   │   └── AdaptiveSchedulerAgent.cs     # Dynamic re-routing on cancellations
│   │
│   ├── Services/                         # Business logic
│   │   ├── SchedulingEngine.cs           # Proximity grouping, cab assignment
│   │   └── NotificationService.cs        # Persist & query notifications
│   │
│   └── EventBus/                         # Event-driven messaging
│       ├── IEventBus.cs                  # Pub/Sub interface
│       ├── StubEventBus.cs               # In-memory stub (swap for Kafka/RabbitMQ)
│       └── Events.cs                     # Event record types
│
└── frontend/                             # Angular 18 Application
    ├── angular.json
    ├── package.json
    ├── proxy.conf.json                   # Dev API proxy (4200 -> 5001)
    ├── tsconfig.json
    ├── public/
    │   └── favicon.ico
    └── src/
        ├── index.html
        ├── main.ts
        ├── styles.css                    # Global styles
        └── app/
            ├── app.component.ts          # Shell with nav + router-outlet
            ├── app.component.html
            ├── app.component.css
            ├── app.config.ts             # Providers: router, http, zone.js
            ├── app.routes.ts             # Route definitions
            │
            ├── models/                   # TypeScript interfaces
            │   ├── employee.ts
            │   ├── cab.ts
            │   ├── driver.ts
            │   ├── cab-request.ts
            │   ├── route.ts
            │   ├── assignment.ts
            │   └── dashboard.ts
            │
            ├── services/
            │   └── api.service.ts         # HTTP client for all API endpoints
            │
            └── components/
                ├── home/                  # Landing page with navigation cards
                ├── employee-registration/ # Register & list employees
                ├── cab-request/           # Submit cab requests
                ├── employee-chat/         # Chat UI for employee agent
                └── supervisor-dashboard/  # Summary cards + scheduling controls
```

---

## Architecture & Design

### Agent Architecture

The system uses **four specialized agents** that communicate via an event bus:

```
+------------------+     +-------------------+
|  EmployeeAgent   |     |  SupervisorAgent  |
|                  |     |                   |
| - Chat processing|     | - Fleet summaries |
| - Status checks  |     | - Delay alerts    |
| - Cancellations  |     | - Route oversight |
+--------+---------+     +---------+---------+
         |                          |
         |     +-----------+       |
         +---> | EventBus  | <-----+
               | (Pub/Sub) |
               +-----+-----+
                     |
         +-----------+-----------+
         |                       |
+--------+---------+   +---------+-----------+
| SchedulerAgent   |   | AdaptiveScheduler   |
|                  |   |      Agent          |
| - Morning cycle  |   | - Cancel handling   |
| - Evening cycle  |   | - Dynamic re-route  |
| - Route creation |   | - Empty route prune |
+------------------+   +---------------------+
```

- **EmployeeAgent** — Processes natural language chat messages from employees. Handles intents: status checks, cancellations, and booking guidance. Communicates cancellations via the event bus.
- **SupervisorAgent** — Generates fleet summaries and publishes them to the event bus. Sends delay notifications for specific routes affecting employees.
- **SchedulerAgent** — Orchestrates the SchedulingEngine for morning pickup and evening drop cycles. Creates routes and assignments automatically.
- **AdaptiveSchedulerAgent** — Subscribes to `cab.request.cancelled` events. Removes cancelled assignments, prunes empty routes, and publishes `route.updated` events for downstream consumers.

### Event-Driven Architecture

| Event Type | Publisher | Subscriber | Purpose |
|------------|-----------|------------|---------|
| `cab.request.cancelled` | EmployeeAgent, SchedulerController | AdaptiveSchedulerAgent | Trigger re-routing |
| `route.updated` | AdaptiveSchedulerAgent | (extensible) | Propagate route changes |
| `fleet.summary` | SupervisorAgent | (extensible) | Fleet status broadcast |
| `route.delayed` | SupervisorAgent | (extensible) | Delay alert broadcast |

The `IEventBus` abstraction supports swapping the in-memory `StubEventBus` for Kafka or RabbitMQ by implementing the same interface.

### Scheduling Engine

The `SchedulingEngine` service:

1. Queries all **pending** `CabRequest` records with their associated `Employee` entities.
2. Groups employees by **geographic proximity** (Haversine formula, 5 km threshold) using pickup coordinates (`PickupLat`, `PickupLng`).
3. Iterates through available `Cab` and `Driver` records in round-robin fashion.
4. Creates `Route` entities with `Assignment` child records for each employee in the group.
5. Updates request statuses to `Assigned-Pickup` (morning) or `Assigned-Drop` (evening).
6. Respects cab capacity with a warning when groups exceed capacity.

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) v18+
- [Angular CLI](https://angular.io/cli) v18 (`npm install -g @angular/cli@18`)
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### 1. Clone & Setup

```bash
git clone <repo-url>
cd CabSchedulerAgent
```

### 2. Backend

```bash
cd backend

# Restore packages
dotnet restore

# Configure connection string in appsettings.json (default: local SQLEXPRESS with Windows auth)

# Create database via EF Core migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the API
dotnet run
```

The API starts at `https://localhost:5001`.

### 3. Frontend

```bash
cd frontend

# Install dependencies
npm install

# Start dev server (with proxy to backend)
ng serve
```

The Angular app starts at `http://localhost:4200`. All `/api` requests are proxied to `https://localhost:5001`.

---

## Configuration

### Connection String

Update `backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CabScheduler;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### CORS

Configured in `backend/Program.cs` to allow `http://localhost:4200`. Update the origin for different environments.

### API Proxy

The Angular dev server proxies `/api` requests to the backend. Configured in `frontend/proxy.conf.json`:

```json
{
  "/api": {
    "target": "https://localhost:5001",
    "secure": false
  }
}
```

---

## API Reference

### Employees

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/employees` | List all employees |
| `GET` | `/api/employees/{id}` | Get employee by ID |
| `POST` | `/api/employees` | Register a new employee |
| `PUT` | `/api/employees/{id}` | Update an employee |
| `DELETE` | `/api/employees/{id}` | Delete an employee |

**POST Request Body:**
```json
{
  "name": "string",
  "email": "string",
  "department": "string",
  "pickupLat": 0.0,
  "pickupLng": 0.0
}
```

### Cabs

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/cabs` | List all cabs |
| `GET` | `/api/cabs/{id}` | Get cab by ID |
| `POST` | `/api/cabs` | Add a cab |
| `PUT` | `/api/cabs/{id}` | Update a cab |
| `DELETE` | `/api/cabs/{id}` | Delete a cab |

### Drivers

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/drivers` | List all drivers |
| `GET` | `/api/drivers/{id}` | Get driver by ID |
| `POST` | `/api/drivers` | Add a driver |
| `PUT` | `/api/drivers/{id}` | Update a driver |
| `DELETE` | `/api/drivers/{id}` | Delete a driver |

### Cab Requests

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/cabrequests` | List all cab requests |
| `GET` | `/api/cabrequests/{id}` | Get request by ID |
| `POST` | `/api/cabrequests` | Submit a new cab request |
| `PUT` | `/api/cabrequests/{id}/status` | Update request status |

**POST Request Body:**
```json
{
  "employeeId": 0,
  "pickupLocation": "string",
  "dropoffLocation": "string"
}
```

### Scheduling

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/scheduler/run-morning` | Run morning pickup cycle |
| `POST` | `/api/scheduler/run-evening` | Run evening drop cycle |
| `POST` | `/api/scheduler/cancel/{requestId}` | Cancel a request & trigger re-routing |

**Cancel Request Body:**
```json
{
  "reason": "string"
}
```

### Routes

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/routes` | List all routes with assignments |
| `GET` | `/api/routes/{id}` | Get route details by ID |

### Assignments

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/assignments` | List all assignments |

### Chat

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/chat/employee/message` | Send a chat message to EmployeeAgent |

**Request Body:**
```json
{
  "employeeId": 0,
  "message": "string"
}
```

**Response:**
```json
{
  "reply": "string"
}
```

**Supported Intents:**
- "what is my status?" / "where is my cab?" / "eta" — returns request status and location
- "cancel my ride" / "cancellation" — cancels the assigned request and triggers re-routing
- "book a pickup" / "request" — redirects to the Cab Request form

### Notifications

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/notifications/send` | Send a notification |
| `GET` | `/api/notifications/fleet-summary` | Generate fleet summary |
| `POST` | `/api/notifications/delay/{routeId}` | Notify delay on a route |
| `GET` | `/api/notifications` | List recent notifications (optional `?channel=` filter) |

### Supervisor

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/supervisor/dashboard` | Get dashboard summary and recent routes |

---

## Database Schema

### Employees
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| Name | string(100) | required |
| Email | string(200) | required, unique index |
| Department | string(100) | |
| PickupLat | double | |
| PickupLng | double | |

### Cabs
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| PlateNumber | string(20) | required |
| Model | string(100) | required |
| Capacity | int | |

### Drivers
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| Name | string(100) | required |
| LicenseNumber | string(50) | required |
| CabId | int? | FK -> Cabs, SET NULL on delete |

### CabRequests
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| EmployeeId | int | FK -> Employees, cascade delete |
| PickupLocation | string(250) | required |
| DropoffLocation | string(250) | required |
| RequestedTime | DateTime | default UTC now |
| Status | string(20) | required, default "Pending" |

**Status values:** `Pending`, `Assigned-Pickup`, `Assigned-Drop`, `Cancelled`

### Routes
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| Name | string(100) | required |
| CabId | int | FK -> Cabs, restrict delete |
| DriverId | int | FK -> Drivers, restrict delete |
| Cycle | string(20) | required, "Morning" or "Evening" |
| CreatedAt | DateTime | default UTC now |

### Assignments
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| RouteId | int | FK -> Routes, cascade delete |
| EmployeeId | int | FK -> Employees, restrict delete |
| CabRequestId | int | FK -> CabRequests, restrict delete |

### Notifications
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | PK, auto-increment |
| Recipient | string(100) | required |
| Channel | string(50) | required (e.g. "Email", "SMS", "Kafka") |
| Message | string(500) | required |
| SentAt | DateTime | default UTC now |
| Status | string(20) | required, default "Sent" |

**Entity Relationship Diagram:**
```
Employees 1---* CabRequests
Cabs      1---* Routes
Drivers   1---* Routes
Routes    1---* Assignments
Employees 1---* Assignments
CabRequests 1---* Assignments
Drivers   ?---1 Cabs (optional)
```

---

## Frontend Routes & Components

| Route | Component | Description |
|-------|-----------|-------------|
| `/` | `HomeComponent` | Landing page with navigation cards |
| `/register` | `EmployeeRegistrationComponent` | Register employees with location coordinates; list all employees |
| `/request-cab` | `CabRequestComponent` | Submit cab requests; view existing requests |
| `/chat` | `EmployeeChatComponent` | Chat interface with EmployeeAgent for status, cancellation, booking |
| `/dashboard` | `SupervisorDashboardComponent` | Fleet summary cards, run scheduling cycles, view recent routes |

All components are standalone (Angular 18 `standalone: true`). The shell component (`AppComponent`) provides a navigation bar and `<router-outlet>`.

---

## Development

### Running Locally

Start the backend and frontend in separate terminals:

```bash
# Terminal 1: Backend
cd backend
dotnet run

# Terminal 2: Frontend
cd frontend
ng serve
```

### Database Migrations

After making model changes:

```bash
cd backend
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Replacing the Stub Event Bus

To swap the in-memory `StubEventBus` for a real message broker, implement `IEventBus`:

```csharp
public class KafkaEventBus : IEventBus
{
    public Task PublishAsync<T>(string topic, T @event) { /* Kafka produce */ }
    public Task SubscribeAsync<T>(string topic, Func<T, Task> handler) { /* Kafka consume */ }
}
```

Then register it in `Program.cs`:

```csharp
// builder.Services.AddSingleton<IEventBus, StubEventBus>();
builder.Services.AddSingleton<IEventBus, KafkaEventBus>();
```

### Adding a New Agent Intent (EmployeeAgent)

Add a new keyword branch in `EmployeeAgent.ProcessChatMessageAsync`:

```csharp
if (lower.Contains("your-keyword"))
{
    // Handle the intent
    return "Response message";
}
```

---

## Roadmap

- [ ] Replace `StubEventBus` with Kafka or RabbitMQ for production readiness
- [ ] Add traffic data integration for real-time route optimization
- [ ] Expand to multiple office destinations with hub-and-spoke routing
- [ ] Implement SignalR for real-time UI notifications
- [ ] Add authentication (JWT) and role-based access control (Employee vs Supervisor)
- [ ] Build agent conversation memory and context persistence
- [ ] Add unit tests (xUnit for backend, Jasmine/Karma for frontend)
- [ ] Containerize with Docker Compose (backend, frontend, SQL Server, Kafka)

---

## License

This project is for learning and demonstration purposes.
