import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { DndContext, DragEndEvent, DragStartEvent, DragOverlay, PointerSensor, useDraggable, useDroppable, useSensor, useSensors } from '@dnd-kit/core';
import type { Task, TaskStatus } from '../types';
import { fetchTasks, createTask, updateTask, deleteTask, moveTask } from '../api';
import TaskCard from './TaskCard';
import TaskForm from './TaskForm';
import Modal from './Modal';
import styles from '../styles/Board.module.css';

const statuses: Array<{ key: TaskStatus; label: string }> = [
  { key: 'todo', label: 'To Do' },
  { key: 'in progress', label: 'In Progress' },
  { key: 'done', label: 'Done' }
];

function StatusColumn({
  status,
  label,
  tasks,
  onEdit,
  onDelete,
  isLoading
}: {
  status: TaskStatus;
  label: string;
  tasks: Task[];
  onEdit: (task: Task) => void;
  onDelete: (id: string) => void;
  isLoading?: boolean;
}) {
  const { setNodeRef, isOver } = useDroppable({ id: status });

  return (
    <section className={styles.column}>
      <div className={styles.columnHeader}>
        <h2>{label}</h2>
        <span>{tasks.length}</span>
      </div>
      <div
        ref={setNodeRef}
        className={`${styles.dropZone} ${isOver ? styles.dropZoneActive : ''}`}
        id={status}
      >
        {isLoading ? (
          <p className={styles.loadingMessage}>Loading tasks...</p>
        ) : tasks.length === 0 ? (
          <p className={styles.emptyMessage}>Drop tasks here or create a new one.</p>
        ) : (
          tasks.map((task) => (
            <TaskCard key={task.id} task={task} onEdit={onEdit} onDelete={onDelete} />
          ))
        )}
      </div>
    </section>
  );
}

export default function Board() {
  const queryClient = useQueryClient();
  const [activeId, setActiveId] = useState<string | null>(null);
  const [editingTask, setEditingTask] = useState<Task | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const todoTasksQuery = useQuery<Task[]>({
    queryKey: ['tasks', 'todo'],
    queryFn: () => fetchTasks('todo'),
    staleTime: 10000,
    refetchOnWindowFocus: false
  });

  const inProgressTasksQuery = useQuery<Task[]>({
    queryKey: ['tasks', 'in progress'],
    queryFn: () => fetchTasks('in progress'),
    staleTime: 10000,
    refetchOnWindowFocus: false
  });

  const doneTasksQuery = useQuery<Task[]>({
    queryKey: ['tasks', 'done'],
    queryFn: () => fetchTasks('done'),
    staleTime: 10000,
    refetchOnWindowFocus: false
  });

  // Combine all tasks for drag and drop operations
  const allTasks = [
    ...(todoTasksQuery.data || []),
    ...(inProgressTasksQuery.data || []),
    ...(doneTasksQuery.data || [])
  ];

  const isLoading = todoTasksQuery.isLoading || inProgressTasksQuery.isLoading || doneTasksQuery.isLoading;
  const isError = todoTasksQuery.isError || inProgressTasksQuery.isError || doneTasksQuery.isError;

  const createMutation = useMutation({
    mutationFn: createTask,
    onSuccess: (newTask) => {
      queryClient.invalidateQueries({ queryKey: ['tasks', newTask.status] });
      setIsModalOpen(false);
    }
  });

  const updateMutation = useMutation({
    mutationFn: updateTask,
    onSuccess: () => {
      // Invalidate all task queries since we don't know the old status
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
      setEditingTask(null);
      setIsModalOpen(false);
    }
  });

  const deleteMutation = useMutation({
    mutationFn: deleteTask,
    onSuccess: () => {
      // Invalidate all task queries since we don't know which status the task was in
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    }
  });

  const moveMutation = useMutation({
    mutationFn: ({ id, status }: { id: string; status: TaskStatus }) => moveTask(id, status),
    onSuccess: (_, { status }) => {
      // Invalidate all task queries to refresh all columns
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    }
  });

  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 5 } }));

  const handleDragStart = (event: DragStartEvent) => {
    setActiveId(event.active.id as string);
  };

  const handleDragEnd = (event: DragEndEvent) => {
    setActiveId(null);
    const activeTaskId = event.active.id as string;
    const overId = event.over?.id as string | null;

    if (!overId || activeTaskId === overId) {
      return;
    }

    const newStatus = statuses.find((column) => column.key === overId)?.key;
    if (!newStatus) {
      return;
    }

    const task = allTasks.find((item) => item.id === activeTaskId);
    if (!task || task.status === newStatus) {
      return;
    }

    moveMutation.mutate({ id: activeTaskId, status: newStatus });
  };

  const handleSubmit = (payload: { title: string; description: string }) => {
    if (editingTask) {
      updateMutation.mutate({ id: editingTask.id, title: payload.title, description: payload.description });
    } else {
      createMutation.mutate({ title: payload.title, description: payload.description, status: 'todo' });
    }
  };

  const handleEdit = (task: Task) => {
    setEditingTask(task);
    setIsModalOpen(true);
  };

  const handleCancelEdit = () => {
    setEditingTask(null);
    setIsModalOpen(false);
  };

  const handleCreateTask = () => {
    setEditingTask(null);
    setIsModalOpen(true);
  };

  const handleDelete = (id: string) => {
    deleteMutation.mutate(id);
  };

  const activeTask = activeId ? allTasks.find((task) => task.id === activeId) ?? null : null;

  return (
    <div className={styles.boardShell}>
      <aside className={styles.sidebar}>
        <button className={styles.createButton} onClick={handleCreateTask}>
          + Create Task
        </button>
        <div className={styles.statusPanel}>
          <h3>Task Status</h3>
          <p>Drag any task into a new column to update its status.</p>
          <div className={styles.badges}>
            {statuses.map((status) => (
              <div key={status.key} className={styles.badge}>
                {status.label}
              </div>
            ))}
          </div>
        </div>
      </aside>

      <section className={styles.content}>
        <div className={styles.queryStatus}>
          {isLoading && <p>Loading tasks...</p>}
          {isError && <p className={styles.error}>Failed to load tasks.</p>}
          {createMutation.isError && <p className={styles.error}>Unable to create task.</p>}
          {updateMutation.isError && <p className={styles.error}>Unable to update task.</p>}
          {deleteMutation.isError && <p className={styles.error}>Unable to delete task.</p>}
        </div>

        <DndContext sensors={sensors} onDragStart={handleDragStart} onDragEnd={handleDragEnd}>
          <div className={styles.columnsContainer}>
            {statuses.map((column) => {
              let tasks: Task[] = [];
              let isColumnLoading = false;

              switch (column.key) {
                case 'todo':
                  tasks = todoTasksQuery.data || [];
                  isColumnLoading = todoTasksQuery.isLoading;
                  break;
                case 'in progress':
                  tasks = inProgressTasksQuery.data || [];
                  isColumnLoading = inProgressTasksQuery.isLoading;
                  break;
                case 'done':
                  tasks = doneTasksQuery.data || [];
                  isColumnLoading = doneTasksQuery.isLoading;
                  break;
              }

              return (
                <StatusColumn
                  key={column.key}
                  status={column.key}
                  label={column.label}
                  tasks={tasks}
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                  isLoading={isColumnLoading}
                />
              );
            })}
          </div>

          <DragOverlay>{activeTask ? <TaskCard task={activeTask} onEdit={() => {}} onDelete={() => {}} isOverlay /> : null}</DragOverlay>
        </DndContext>
      </section>

      <Modal
        isOpen={isModalOpen}
        onClose={handleCancelEdit}
        title={editingTask ? 'Edit task' : 'Create task'}
      >
        <TaskForm
          key={editingTask?.id ?? 'create'}
          initialTask={editingTask ?? undefined}
          onSubmit={handleSubmit}
          onCancel={handleCancelEdit}
          isSaving={createMutation.isPending || updateMutation.isPending}
        />
      </Modal>
    </div>
  );
}
