import { Component, ReactNode, ErrorInfo } from 'react'
import { Button, Result } from 'antd'

interface Props {
  children: ReactNode
}

interface State {
  hasError: boolean
  error: Error | null
}

/**
 * Catches unhandled React rendering errors and shows a friendly fallback
 * instead of a blank white screen. Without this, any unhandled error in a
 * child component crashes the entire application.
 *
 * Must be a class component — React hooks cannot implement componentDidCatch.
 */
export default class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false, error: null }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error }
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    // In production this would forward to an error tracking service (e.g. Sentry)
    console.error('[ErrorBoundary] Unhandled error:', error, info.componentStack)
  }

  render() {
    if (!this.state.hasError) return this.props.children

    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <Result
          status="error"
          title="Something went wrong"
          subTitle="An unexpected error occurred. Refresh the page to try again."
          extra={
            <Button type="primary" onClick={() => window.location.reload()}>
              Refresh Page
            </Button>
          }
        />
      </div>
    )
  }
}
