import Board from './components/Board';
import styles from './styles/App.module.css';

export default function App() {
  return (
    <div className={styles.appContainer}>
      <header className={styles.header}>
        <div>
          <h1>Effi Kanban Board</h1>
          <p>React 19 + TypeScript + Vite + TanStack Query + dnd-kit</p>
          <a href="https://github.com/andreghigobr/kanban-board" target="_blank" rel="noopener noreferrer">View on Github</a>
        </div>
      </header>
      <main className={styles.main}> 
        <Board />
      </main>
    </div>
  );
}
