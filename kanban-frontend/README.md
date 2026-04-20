# Kanban Frontend

This is the frontend application for the Kanban Board, built with React 19, TypeScript, Vite, and other modern libraries.

## Prerequisites

- Node.js 18 or later
- npm or yarn

## Installing Dependencies

1. Navigate to the frontend directory:
   ```bash
   cd kanban-frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

## Running the Application

1. Start the development server:
   ```bash
   npm run dev
   ```

   The app will run on `http://localhost:5173` and proxy API calls to the backend.

## Building the Application

1. Build for production:
   ```bash
   npm run build
   ```

   The build artifacts will be stored in the `dist/` directory.

## Testing the Application

Currently, there are no automated tests set up. Manual testing can be done by interacting with the UI.

## Features

- Create, edit, and delete tasks
- Drag and drop tasks between status columns (Todo, In Progress, Done)
- Responsive design with CSS Modules

## Project Structure

- `src/` - Source code
  - `components/` - React components
  - `api.ts` - API client functions
  - `types.ts` - TypeScript interfaces
- `vite.config.ts` - Vite configuration
- `package.json` - Dependencies and scripts

## Environment Variables

- `VITE_API_BASE` - Base URL for the backend API (defaults to proxy in dev mode)