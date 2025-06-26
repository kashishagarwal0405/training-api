# Training Management API

A .NET 6 Web API for managing training requests, sessions, and participants using Dapper for data access.

## Features

- **Training Request Management**: Create, update, and track training requests
- **Training Sessions**: Schedule and manage training sessions
- **Participant Management**: Register users for training sessions
- **User Management**: Manage users with different roles (employee, L&D, admin)
- **Reporting**: Generate reports for training requests, departments, and budgets
- **Role-based Access**: Different dashboards and permissions based on user roles

## Technology Stack

- **.NET 6** - Web API framework
- **Dapper** - Micro ORM for data access
- **SQL Server LocalDB** - Database
- **Swagger/OpenAPI** - API documentation
- **CORS** - Cross-origin resource sharing

## Prerequisites

- .NET 6 SDK
- SQL Server LocalDB (included with Visual Studio)
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Database Setup

1. Open SQL Server Management Studio or Azure Data Studio
2. Connect to your LocalDB instance: `(localdb)\mssqllocaldb`
3. Create a new database named `TrainingManagementDB`
4. Run the `database_setup.sql` script to create tables and sample data

```sql
-- Create database
CREATE DATABASE TrainingManagementDB;
GO
USE TrainingManagementDB;
GO

-- Then run the database_setup.sql script
```

### 2. Connection String

The API uses the following connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TrainingManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Run the API

```bash
cd TrainingAPI
dotnet restore
dotnet run
```

The API will be available at:
- HTTP: http://localhost:7001
- HTTPS: https://localhost:7001
- Swagger UI: https://localhost:7001/swagger

## API Endpoints

### Training Requests
- `GET /api/training/requests` - Get all training requests
- `GET /api/training/requests/{id}` - Get training request by ID
- `GET /api/training/requests/user/{userId}` - Get requests by user
- `GET /api/training/requests/status/{status}` - Get requests by status
- `POST /api/training/requests` - Create new training request
- `PUT /api/training/requests/{id}/status` - Update request status
- `DELETE /api/training/requests/{id}` - Delete training request

### Training Sessions
- `GET /api/training/sessions` - Get all training sessions
- `GET /api/training/sessions/{id}` - Get session by ID
- `GET /api/training/sessions/request/{requestId}` - Get sessions by request
- `POST /api/training/sessions` - Create new training session
- `PUT /api/training/sessions/{id}` - Update training session
- `DELETE /api/training/sessions/{id}` - Delete training session

### Participants
- `GET /api/training/sessions/{sessionId}/participants` - Get session participants
- `POST /api/training/sessions/{sessionId}/register/{userId}` - Register for session
- `DELETE /api/training/sessions/{sessionId}/unregister/{userId}` - Unregister from session
- `PUT /api/training/sessions/{sessionId}/participants/{userId}/status` - Update participant status

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/email/{email}` - Get user by email
- `GET /api/users/role/{role}` - Get users by role
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `PUT /api/users/{id}/deactivate` - Deactivate user

### Reports
- `GET /api/reports/training-requests` - Get training request report
- `GET /api/reports/departments` - Get department report
- `GET /api/reports/budget` - Get budget report

### Dashboard
- `GET /api/dashboard/stats/{role}` - Get dashboard statistics by role

## Data Models

### User
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public string Role { get; set; } // "employee", "ld", "admin"
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
```

### TrainingRequest
```csharp
public class TrainingRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Department { get; set; }
    public string TrainingType { get; set; }
    public string Priority { get; set; } // "low", "medium", "high", "urgent"
    public int ExpectedParticipants { get; set; }
    public string Duration { get; set; }
    public DateTime PreferredDate { get; set; }
    public string Description { get; set; }
    public string BusinessJustification { get; set; }
    public decimal? Budget { get; set; }
    public string Status { get; set; } // "pending", "approved", "rejected", "completed"
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int RequesterId { get; set; }
    public string RequesterName { get; set; }
    public string RequesterEmail { get; set; }
}
```

### TrainingSession
```csharp
public class TrainingSession
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Trainer { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string Status { get; set; } // "scheduled", "in-progress", "completed", "cancelled"
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TrainingRequestId { get; set; }
    public string RequestTitle { get; set; }
}
```

### TrainingParticipant
```csharp
public class TrainingParticipant
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TrainingSessionId { get; set; }
    public string Status { get; set; } // "registered", "attended", "no-show", "cancelled"
    public DateTime RegisteredAt { get; set; }
    public DateTime? AttendedAt { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
}
```

## Architecture

The API uses a clean architecture pattern:

- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic layer using Dapper for data access
- **Models**: Data transfer objects
- **Dapper**: Micro ORM for database operations

### Key Features

1. **Dapper Integration**: Direct SQL queries with parameterized queries for security
2. **Connection Management**: Proper connection disposal using `using` statements
3. **Error Handling**: Comprehensive error handling and logging
4. **CORS Support**: Configured for cross-origin requests
5. **Swagger Documentation**: Auto-generated API documentation

## Sample Data

The database setup script includes sample data:

- **Users**: Admin, employees, and L&D users
- **Training Requests**: Sample requests for different departments
- **Training Sessions**: Scheduled sessions with trainers
- **Participants**: Sample registrations

## Security Considerations

- Parameterized queries prevent SQL injection
- Input validation on all endpoints
- Role-based access control
- CORS configuration for frontend integration

## Troubleshooting

### Common Issues

1. **Connection String**: Ensure LocalDB is running and the connection string is correct
2. **Database**: Make sure the database and tables are created using the setup script
3. **Port Conflicts**: Change the port in `appsettings.json` if 7001 is in use

### Logs

Check the console output for detailed error messages and logs.

## Frontend Integration

This API is designed to work with the React frontend. The frontend can be found in the parent directory and uses:

- React Query for data fetching
- Axios for HTTP requests
- Role-based routing and components

## License

This project is for educational purposes. 