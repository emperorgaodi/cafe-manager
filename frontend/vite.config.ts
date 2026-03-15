import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      // Forwards /api/v1/... → http://localhost:5000/api/v1/...
      // No rewrite: the backend route prefix "api/v{version}/[controller]" is kept intact
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
})
