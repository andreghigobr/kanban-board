import type { Task, TaskStatus } from './types';

const API_BASE = '';

async function checkResponse(response: Response) {
  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || response.statusText);
  }
  return response;
}

export async function fetchTasks(): Promise<Task[]> {
  const response = await fetch(`${API_BASE}/kanban/Tasks`);
  await checkResponse(response);
  return response.json();
}

export async function createTask(payload: Pick<Task, 'title' | 'description' | 'status'>): Promise<Task> {
  const response = await fetch(`${API_BASE}/kanban/Tasks`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });
  await checkResponse(response);
  return response.json();
}

export async function updateTask(payload: Pick<Task, 'id' | 'title' | 'description'>): Promise<Task> {
  const response = await fetch(`${API_BASE}/kanban/Tasks`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });
  await checkResponse(response);
  return response.json();
}

export async function deleteTask(id: string): Promise<void> {
  const response = await fetch(`${API_BASE}/kanban/Tasks/${id}`, {
    method: 'DELETE'
  });
  await checkResponse(response);
}

export async function moveTask(id: string, status: TaskStatus): Promise<Task> {
  const response = await fetch(`${API_BASE}/kanban/Tasks/${id}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ status })
  });
  await checkResponse(response);
  return response.json();
}
