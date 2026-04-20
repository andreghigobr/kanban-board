import { useEffect, useState, type FormEvent } from 'react';
import type { Task, TaskStatus } from '../types';
import styles from '../styles/TaskForm.module.css';

const statusOptions: Array<{ value: TaskStatus; label: string }> = [
  { value: 'todo', label: 'Todo' },
  { value: 'in progress', label: 'In Progress' },
  { value: 'done', label: 'Done' }
];

interface TaskFormProps {
  initialTask?: Task;
  onSubmit: (payload: { title: string; description: string; status: TaskStatus }) => void;
  onCancel?: () => void;
  isSaving: boolean;
}

export default function TaskForm({ initialTask, onSubmit, onCancel, isSaving }: TaskFormProps) {
  const [title, setTitle] = useState(initialTask?.title ?? '');
  const [description, setDescription] = useState(initialTask?.description ?? '');
  const [status, setStatus] = useState<TaskStatus>(initialTask?.status ?? 'todo');

  useEffect(() => {
    setTitle(initialTask?.title ?? '');
    setDescription(initialTask?.description ?? '');
    setStatus(initialTask?.status ?? 'todo');
  }, [initialTask]);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!title.trim() || !description.trim()) {
      return;
    }
    onSubmit({ title: title.trim(), description: description.trim(), status });
    if (!initialTask) {
      setTitle('');
      setDescription('');
      setStatus('todo');
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <div className={styles.formHeader}>
        <h2>{initialTask ? 'Edit Task' : 'Create Task'}</h2>
        {initialTask && onCancel ? (
          <button type="button" className={styles.cancelButton} onClick={onCancel}>
            Cancel
          </button>
        ) : null}
      </div>

      <label className={styles.label}>
        Title
        <input
          className={styles.input}
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          placeholder="Task title"
          required
        />
      </label>

      <label className={styles.label}>
        Description
        <textarea
          className={styles.textarea}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          placeholder="Task details"
          rows={4}
          required
        />
      </label>

      {!initialTask ? (
        <label className={styles.label}>
          Status
          <select className={styles.select} value={status} onChange={(event) => setStatus(event.target.value as TaskStatus)}>
            {statusOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>
      ) : (
        <div className={styles.readOnlyStatus}>Status: {initialTask.status}</div>
      )}

      <button type="submit" className={styles.submitButton} disabled={isSaving}>
        {isSaving ? 'Saving…' : initialTask ? 'Update Task' : 'Create Task'}
      </button>
    </form>
  );
}
