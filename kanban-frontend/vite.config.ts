import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 4173,
    proxy: {
      '/kanban': {
        target: 'http://localhost:5257',
        changeOrigin: true,
        secure: false
      }
    }
  }
});
