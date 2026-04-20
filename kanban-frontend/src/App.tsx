import Board from './components/Board';
import styles from './styles/App.module.css';

export default function App() {
  return (
    <div className={styles.appContainer}>
      <header className={styles.header}>
        <div>
          <h1>Kanban Board</h1>
          <p>React 19 + TypeScript + Vite + TanStack Query + dnd-kit</p>
        </div>
      </header>
      <main className={styles.main}> 
        <Board />
      </main>
    </div>
  );
}
