import type { Task, TaskStatus } from './types';

const API_BASE = import.meta.env.VITE_API_BASE || '';

type ApiTaskStatus = 'ToDo' | 'InProgress' | 'Done';

interface TaskResponse {
  taskId: string;
  title: string;
  description: string | null;
  status: ApiTaskStatus;
  createdAt: string;
  updatedAt: string;
}

interface CreateTaskRequest {
  title: string;
  description?: string | null;
  status: ApiTaskStatus;
}

interface UpdateTaskRequest {
  taskId: string;
  title: string;
  description?: string | null;
}

interface MoveTaskRequest {
  status: ApiTaskStatus;
}

const backendStatusByFrontend: Record<TaskStatus, ApiTaskStatus> = {
  todo: 'ToDo',
  'in progress': 'InProgress',
  done: 'Done'
};

const frontendStatusByBackend: Record<ApiTaskStatus, TaskStatus> = {
  ToDo: 'todo',
  InProgress: 'in progress',
  Done: 'done'
};

async function checkResponse(response: Response) {
  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || response.statusText);
  }
  return response;
}

function mapTaskResponse(task: TaskResponse): Task {
  return {
    id: task.taskId,
    title: task.title,
    description: task.description ?? '',
    status: frontendStatusByBackend[task.status],
    createdAt: task.createdAt,
    updatedAt: task.updatedAt
  };
}

export async function fetchTasks(): Promise<Task[]> {
  const response = await fetch(`${API_BASE}/kanban/tasks`);
  await checkResponse(response);
  const result = await response.json() as TaskResponse[];
  return result.map(mapTaskResponse);
}

export async function createTask(payload: Pick<Task, 'title' | 'description' | 'status'>): Promise<Task> {
  const body: CreateTaskRequest = {
    title: payload.title,
    description: payload.description || null,
    status: backendStatusByFrontend[payload.status]
  };

  const response = await fetch(`${API_BASE}/kanban/tasks`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });

  await checkResponse(response);
  const result = await response.json() as TaskResponse;
  return mapTaskResponse(result);
}

export async function updateTask(payload: Pick<Task, 'id' | 'title' | 'description'>): Promise<void> {
  const body: UpdateTaskRequest = {
    taskId: payload.id,
    title: payload.title,
    description: payload.description || null
  };

  const response = await fetch(`${API_BASE}/kanban/tasks`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });

  await checkResponse(response);
}

export async function deleteTask(id: string): Promise<void> {
  const response = await fetch(`${API_BASE}/kanban/tasks/${id}`, {
    method: 'DELETE'
  });
  await checkResponse(response);
}

export async function moveTask(id: string, status: TaskStatus): Promise<void> {
  const body: MoveTaskRequest = {
    status: backendStatusByFrontend[status]
  };

  const response = await fetch(`${API_BASE}/kanban/tasks/${id}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });

  await checkResponse(response);
}
