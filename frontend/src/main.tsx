import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import ErrorBoundary from './components/ErrorBoundary'

const rootElement = document.getElementById('root')
if (!rootElement) throw new Error('Root element not found. Check index.html.')

ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    {/* Outer ErrorBoundary catches errors in App itself (e.g. broken context providers) */}
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </React.StrictMode>
)
