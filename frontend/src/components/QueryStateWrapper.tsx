import { ReactNode } from 'react'
import { Alert, Button, Spin } from 'antd'
import { ReloadOutlined } from '@ant-design/icons'

interface Props {
  isLoading: boolean
  isError: boolean
  error: Error | null
  isEmpty: boolean
  emptyTitle: string
  emptyDescription: string
  onAddNew?: () => void
  addNewLabel?: string
  onRetry: () => void
  children: ReactNode
}

/**
 * Wraps a data-fetching section and renders the correct state:
 * - Loading spinner while data is being fetched
 * - Error alert with retry button if the request failed
 * - Empty state with a prompt to add the first item
 * - The actual content (children) when data is available
 *
 * Centralising this logic prevents every page from duplicating isLoading/isError checks.
 */
export default function QueryStateWrapper({
  isLoading, isError, error, isEmpty,
  emptyTitle, emptyDescription, onAddNew, addNewLabel,
  onRetry, children,
}: Props) {
  if (isLoading) {
    return (
      <div className="flex justify-center items-center py-16">
        <Spin size="large" tip="Loading…" />
      </div>
    )
  }

  if (isError) {
    return (
      <Alert
        type="error"
        showIcon
        message="Failed to load data"
        description={error?.message ?? 'An unexpected error occurred.'}
        action={
          <Button size="small" icon={<ReloadOutlined />} onClick={onRetry}>
            Retry
          </Button>
        }
        className="my-4"
      />
    )
  }

  if (isEmpty) {
    return (
      <div className="empty-state">
        <p className="empty-state__icon">☕</p>
        <p className="empty-state__title">{emptyTitle}</p>
        <p className="empty-state__description">{emptyDescription}</p>
        {onAddNew && (
          <Button type="primary" onClick={onAddNew}>
            {addNewLabel ?? 'Add New'}
          </Button>
        )}
      </div>
    )
  }

  return <>{children}</>
}
