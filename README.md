# Kanban Board

A full-stack Kanban board application with a React frontend and ASP.NET Core backend.

## Solution Overview

This project implements a simple Kanban board where users can manage tasks across three statuses: Todo, In Progress, and Done. The frontend provides a drag-and-drop interface for updating task statuses, while the backend handles CRUD operations via RESTful APIs.

### Technologies Used

- **Frontend**: React 19, TypeScript, Vite, TanStack Query, dnd-kit, CSS Modules
- **Backend**: ASP.NET Core, C#, Entity Framework (assumed)
- **Database**: In-memory or configured database (check `appsettings.json`)

## API Design

The backend exposes a RESTful API under the `/kanban/tasks` endpoint. The full API contract and interactive documentation is available via Swagger at `http://localhost:5257/swagger` when the backend is running.

### Endpoints

- `GET /kanban/tasks` - Retrieve all tasks
  - Response: Array of task objects
- `POST /kanban/tasks` - Create a new task
  - Request Body: `{ title: string, description: string }`
  - Response: Created task object
- `PUT /kanban/tasks` - Update an existing task
  - Request Body: `{ id: number, title: string, description: string, status: string }`
  - Response: Updated task object
- `DELETE /kanban/tasks/{id}` - Delete a task
  - Response: Success status
- `PATCH /kanban/tasks/{id}` - Move a task to a new status
  - Request Body: `{ status: string }`
  - Response: Updated task object

### Task Object Structure

```json
{
  "id": 1,
  "title": "Task Title",
  "description": "Task Description",
  "status": "Todo" | "InProgress" | "Done"
}
```

## How to Build and Run

### Prerequisites

- .NET 8.0 or later
- Node.js 18 or later
- npm or yarn

### Backend

1. Navigate to `kanban-backend`:
   ```bash
   cd kanban-backend
   ```

2. Restore and build:
   ```bash
   dotnet restore
   dotnet build
   ```

3. Run:
   ```bash
   dotnet run --project Kanban.Api
   ```

   Runs on `http://localhost:5257`.

### Frontend

1. Navigate to `kanban-frontend`:
   ```bash
   cd kanban-frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Run in development:
   ```bash
   npm run dev
   ```

   Runs on `http://localhost:5173` with API proxy to backend.

### Full Application

1. Start the backend first.
2. Start the frontend.
3. Open `http://localhost:5173` in your browser.

## Testing

- Backend: `dotnet test` in `kanban-backend`
- Frontend: No automated tests currently

## Project Structure

- `kanban-backend/` - ASP.NET Core API
- `kanban-frontend/` - React application
- `README.md` - This file
