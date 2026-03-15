import { WarningOutlined } from '@ant-design/icons'

/**
 * Shown on Add/Edit forms when the user has made unsaved changes.
 * Gives a clear visual cue before they accidentally navigate away.
 */
export default function UnsavedBanner() {
  return (
    <div className="unsaved-banner">
      <WarningOutlined />
      You have unsaved changes. Submit the form or click Cancel to discard them.
    </div>
  )
}
