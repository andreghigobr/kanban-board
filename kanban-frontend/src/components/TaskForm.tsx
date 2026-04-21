import { useEffect, useState, type FormEvent } from 'react';
import type { Task } from '../types';
import styles from '../styles/TaskForm.module.css';

interface TaskFormProps {
  initialTask?: Task;
  onSubmit: (payload: { title: string; description: string }) => void;
  onCancel?: () => void;
  isSaving: boolean;
}

export default function TaskForm({ initialTask, onSubmit, onCancel, isSaving }: TaskFormProps) {
  const [title, setTitle] = useState(initialTask?.title ?? '');
  const [description, setDescription] = useState(initialTask?.description ?? '');

  useEffect(() => {
    setTitle(initialTask?.title ?? '');
    setDescription(initialTask?.description ?? '');
  }, [initialTask]);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!title.trim() || !description.trim()) {
      return;
    }
    onSubmit({ title: title.trim(), description: description.trim() });
    if (!initialTask) {
      setTitle('');
      setDescription('');
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
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

      {initialTask ? (
        <div className={styles.readOnlyStatus}>Status: {initialTask.status}</div>
      ) : null}

      <div className={styles.buttonGroup}>
        {onCancel && (
          <button type="button" className={styles.cancelButton} onClick={onCancel}>
            Cancel
          </button>
        )}
        <button type="submit" className={styles.submitButton} disabled={isSaving}>
          {isSaving ? 'Saving…' : initialTask ? 'Update task' : 'Create task'}
        </button>
      </div>
    </form>
  );
}
