import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { Menu } from 'antd'
import { CoffeeOutlined, TeamOutlined } from '@ant-design/icons'
import { Link, Route, BrowserRouter as Router, Routes, useLocation } from 'react-router-dom'
import ErrorBoundary from './components/ErrorBoundary'
import AddEditCafePage from './pages/AddEditCafePage'
import AddEditEmployeePage from './pages/AddEditEmployeePage'
import CafesPage from './pages/CafesPage'
import EmployeesPage from './pages/EmployeesPage'
import NotFoundPage from './pages/NotFoundPage'
import './index.css'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: 1,
      // Don't retry on 404s — retrying won't help
      retryDelay: (attempt) => Math.min(1000 * 2 ** attempt, 10_000),
    },
  },
})

const NAV_ITEMS = [
  { key: '/cafes', icon: <CoffeeOutlined />, label: <Link to="/cafes">Cafés</Link> },
  { key: '/employees', icon: <TeamOutlined />, label: <Link to="/employees">Employees</Link> },
]

function AppLayout() {
  const { pathname } = useLocation()
  const activeKey = pathname.startsWith('/employees') ? '/employees' : '/cafes'

  return (
    <div style={{ minHeight: '100vh', background: 'var(--color-bg)' }}>
      <header className="app-header">
        <span className="app-header__logo">☕ Café Manager</span>
        <Menu
          theme="dark"
          mode="horizontal"
          selectedKeys={[activeKey]}
          items={NAV_ITEMS}
          style={{ background: 'transparent', borderBottom: 'none', flex: 1 }}
        />
      </header>

      <main>
        {/* ErrorBoundary wraps all routes: a crash in one page won't bring down the whole app */}
        <ErrorBoundary>
          <Routes>
            <Route path="/" element={<CafesPage />} />
            <Route path="/cafes" element={<CafesPage />} />
            <Route path="/cafes/new" element={<AddEditCafePage />} />
            <Route path="/cafes/edit/:id" element={<AddEditCafePage />} />
            <Route path="/employees" element={<EmployeesPage />} />
            <Route path="/employees/new" element={<AddEditEmployeePage />} />
            <Route path="/employees/edit/:id" element={<AddEditEmployeePage />} />
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </ErrorBoundary>
      </main>
    </div>
  )
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <AppLayout />
      </Router>
      {(import.meta as any).env?.DEV && <ReactQueryDevtools initialIsOpen={false} />}
    </QueryClientProvider>
  )
}
