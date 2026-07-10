# Cab Scheduler v0.2

## Overview

This version introduces the Scheduling Engine. Employees are grouped by pickup proximity, and cabs are assigned automatically with respect to capacity. The SchedulerAgent implemented using Microsoft Agent Framework orchestrates assignments. A supervisor dashboard has been added to the Angular frontend.

## Features

| Feature                        | Status |
| ------------------------------ | ------ |
| Employee Registration & Cab Request | v0.1   |
| REST API (Employees, Cabs, Drivers, CabRequests) | v0.1   |
| SQL Express persistence via EF Core | v0.1   |
| Agent Framework Stubs (EmployeeAgent, SupervisorAgent) | v0.1   |
| **Automated Scheduling Engine**     | **v0.2** |
| **Morning pickup & evening drop cycle** | **v0.2** |
| **Route grouping by employee location proximity** | **v0.2** |
| **Angular Supervisor Dashboard**    | **v0.2** |
| **SchedulerAgent (orchestrates assignments)** | **v0.2** |

## Tech Stack

| Technology                 | Version |
| -------------------------- | ------- |
| Angular                    | 18      |
| ASP.NET Core               | 10      |
| SQL Express                | Latest  |
| Entity Framework Core      | 10      |
| Microsoft Agent Framework  | Latest  |

## Project Structure

```
CabSchedulerAgent/
├── frontend/                  # Angular 18 application
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/    # Reusable UI components
│   │   │   ├── services/      # API service layer
│   │   │   └── models/        # TypeScript interfaces
│   │   └── assets/
│   └── angular.json
├── backend/                   # ASP.NET Core 10 Web API
│   ├── Controllers/
│   ├── Models/
│   ├── Data/                  # DbContext & migrations
│   ├── Agents/                # Agent stubs & SchedulerAgent
│   └── Services/              # Scheduling engine & route grouping
├── database/                  # SQL Express schema & migrations
├── docs/                      # Additional documentation
└── README.md
```

## Prerequisites

- [Node.js](https://nodejs.org/) (v18+)
- [Angular CLI](https://angular.io/cli) (v18)
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repo-url>
cd CabSchedulerAgent
```

### 2. Backend Setup

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

The API will be available at `https://localhost:5001`.

### 3. Frontend Setup

```bash
cd frontend
npm install
ng serve
```

The Angular app will be available at `http://localhost:4200`.

### 4. Database Setup

Ensure SQL Express is running. Update the connection string in `backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CabScheduler;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## API Endpoints

| Method | Endpoint                    | Description                      |
| ------ | --------------------------- | -------------------------------- |
| GET    | `/api/employees`            | List all employees               |
| POST   | `/api/employees`            | Register an employee             |
| GET    | `/api/cabs`                 | List all cabs                    |
| POST   | `/api/cabs`                 | Add a cab                        |
| GET    | `/api/drivers`              | List all drivers                 |
| POST   | `/api/drivers`              | Add a driver                     |
| GET    | `/api/cabrequests`          | List all cab requests            |
| POST   | `/api/cabrequests`          | Submit a cab request             |
| PUT    | `/api/cabrequests/{id}/status` | Update request status         |
| POST   | `/api/scheduler/run-morning`   | **v0.2** Run morning pickup cycle   |
| POST   | `/api/scheduler/run-evening`   | **v0.2** Run evening drop cycle     |
| GET    | `/api/routes`               | **v0.2** List all routes             |
| GET    | `/api/assignments`          | **v0.2** List all assignments        |
| GET    | `/api/supervisor/dashboard` | **v0.2** Supervisor dashboard data   |

## Agents

- **EmployeeAgent** — Handles employee-facing interactions (registration, cab requests).
- **SupervisorAgent** — Coordinates scheduling decisions and driver assignments.
- **SchedulerAgent** (v0.2) — Orchestrates the automated scheduling engine: groups employees by pickup proximity, assigns cabs respecting capacity, and manages morning/evening cycles.

Agent implementations are defined in `backend/Agents/`.

## Database Schema

### Employees

| Column      | Type     |
| ----------- | -------- |
| Id          | int (PK) |
| Name        | string   |
| Email       | string   |
| Department  | string   |
| PickupLat   | double   |
| PickupLng   | double   |

### Cabs

| Column        | Type     |
| ------------- | -------- |
| Id            | int (PK) |
| PlateNumber   | string   |
| Model         | string   |
| Capacity      | int      |

### Drivers

| Column        | Type     |
| ------------- | -------- |
| Id            | int (PK) |
| Name          | string   |
| LicenseNumber | string   |
| CabId         | int? (FK)|

### Routes (v0.2)

| Column      | Type     |
| ----------- | -------- |
| Id          | int (PK) |
| Name        | string   |
| CabId       | int (FK) |
| DriverId    | int (FK) |
| Cycle       | string   |
| CreatedAt   | DateTime |

### Assignments (v0.2)

| Column        | Type     |
| ------------- | -------- |
| Id            | int (PK) |
| RouteId       | int (FK) |
| EmployeeId    | int (FK) |
| CabRequestId  | int (FK) |

## Next Steps

- [ ] Add AI communication agents for employee notifications.
- [ ] Implement real-time notifications via SignalR.
- [ ] Add authentication and role-based access control.

## License

This project is for learning and demonstration purposes.
