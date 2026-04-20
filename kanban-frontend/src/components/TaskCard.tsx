import { CSS } from '@dnd-kit/utilities';
import { useDraggable } from '@dnd-kit/core';
import type { Task } from '../types';
import styles from '../styles/TaskCard.module.css';

interface TaskCardProps {
  task: Task;
  onEdit: (task: Task) => void;
  onDelete: (id: string) => void;
  isOverlay?: boolean;
}

export default function TaskCard({ task, onEdit, onDelete, isOverlay = false }: TaskCardProps) {
  const draggable = !isOverlay ? useDraggable({ id: task.id }) : null;
  const transform = draggable?.transform;
  const isDragging = draggable?.isDragging ?? false;
  const style = {
    transform: transform ? CSS.Translate.toString(transform) : undefined,
    opacity: isDragging || isOverlay ? 0.8 : 1
  };

  return (
    <article
      ref={draggable?.setNodeRef}
      style={style}
      className={styles.card}
      {...draggable?.attributes}
      {...draggable?.listeners}
    >
      <div className={styles.headline}>
        <h3>{task.title}</h3>
        <div className={styles.actions}>
          <button type="button" className={styles.editButton} onClick={() => onEdit(task)}>
            Edit
          </button>
          <button type="button" className={styles.deleteButton} onClick={() => onDelete(task.id)}>
            Delete
          </button>
        </div>
      </div>
      <p className={styles.description}>{task.description}</p>
      <div className={styles.metaRow}>
        <span>{new Date(task.createdAt).toLocaleDateString()}</span>
        <span>{new Date(task.updatedAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
      </div>
    </article>
  );
}
