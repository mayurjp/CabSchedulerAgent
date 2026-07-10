# Cab Scheduler v0.1

## Overview

Initial version of the Cab Scheduler app. Employees can register and request cab service via an Angular frontend. The backend stores employees, cabs, and drivers in SQL Express. Microsoft Agent Framework is introduced with stub agents for future orchestration.

## Features

- **Employee Registration & Cab Request** — Angular 18 UI for employees to register and submit cab requests.
- **REST API** — ASP.NET Core 10 Web API backend providing endpoints for employees, cabs, and drivers.
- **Data Persistence** — SQL Express database with `Employees`, `Cabs`, and `Drivers` tables, managed via Entity Framework Core.
- **Agent Framework Stubs** — Basic agent contracts (`EmployeeAgent`, `SupervisorAgent`) leveraging the Microsoft Agent Framework for future intelligent orchestration.

## Tech Stack

| Technology               | Version            |
| ------------------------ | ------------------ |
| Angular                  | 18                 |
| ASP.NET Core             | 10                 |
| SQL Express              | Latest             |
| Entity Framework Core    | 10                 |
| Microsoft Agent Framework| Stub (latest)      |

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
│   └── Agents/                # Agent stubs
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

## API Endpoints (Planned)

| Method | Endpoint              | Description              |
| ------ | --------------------- | ------------------------ |
| GET    | `/api/employees`      | List all employees       |
| POST   | `/api/employees`      | Register an employee     |
| GET    | `/api/cabs`           | List all cabs            |
| POST   | `/api/cabs`           | Add a cab                |
| GET    | `/api/drivers`        | List all drivers         |
| POST   | `/api/drivers`        | Add a driver             |
| POST   | `/api/cab-requests`   | Submit a cab request     |

## Agents

The Microsoft Agent Framework is used with the following stub agents:

- **EmployeeAgent** — Handles employee-facing interactions (registration, cab requests).
- **SupervisorAgent** — Coordinates scheduling decisions and driver assignments.

Agent contracts are defined in `backend/Agents/`.

## Database Schema

### Employees

| Column      | Type     |
| ----------- | -------- |
| Id          | int (PK) |
| Name        | string   |
| Email       | string   |
| Department  | string   |

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

## Next Steps

- [ ] Implement scheduling engine to match cabs and drivers to employee requests.
- [ ] Add `Routes` and `Assignments` tables with corresponding API endpoints.
- [ ] Build out agent orchestration logic for automated decision-making.
- [ ] Add authentication and role-based access control.
- [ ] Implement real-time notifications for cab assignment status.

## License

This project is for learning and demonstration purposes.
