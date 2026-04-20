# Kanban Backend

This is the backend API for the Kanban Board application, built with ASP.NET Core.

## Prerequisites

- .NET 8.0 or later
- A C# development environment (e.g., Visual Studio, VS Code with C# extension)

## Building the Application

1. Navigate to the backend directory:
   ```bash
   cd kanban-backend
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

## Running the Application

1. From the `kanban-backend` directory, run:
   ```bash
   dotnet run --project Kanban.Api
   ```

   The API will start on `http://localhost:5257` (or the port configured in `appsettings.json`).

## API Documentation

Swagger UI is available at `http://localhost:5257/swagger` when the application is running. This provides interactive API documentation and allows testing the endpoints directly.

## Testing the Application

1. Run the tests:
   ```bash
   dotnet test
   ```

   This will execute the unit tests in the `Kanban.Tests` project.

## API Endpoints

- `GET /kanban/tasks` - Get all tasks
- `POST /kanban/tasks` - Create a new task
- `PUT /kanban/tasks` - Update an existing task
- `DELETE /kanban/tasks/{id}` - Delete a task by ID
- `PATCH /kanban/tasks/{id}` - Move a task to a new status

## Project Structure

- `Kanban.Api/` - Main API project
- `Kanban.Tests/` - Unit tests
- `Kanban.sln` - Solution file

## Configuration

- `appsettings.json` - Application settings
- `appsettings.Development.json` - Development-specific settings