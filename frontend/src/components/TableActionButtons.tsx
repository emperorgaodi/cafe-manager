import { Button, Modal, Space } from 'antd'
import { DeleteOutlined, EditOutlined } from '@ant-design/icons'

interface TableActionButtonsProps {
  onEdit: () => void
  onDelete: () => void
  deleteConfirmTitle: string
  deleteConfirmContent?: string
}

/**
 * Reusable Edit + Delete button pair for AG Grid rows.
 * Previously copy-pasted between CafesPage and EmployeesPage — now a single component.
 * Handles the confirmation modal internally so callers only pass a callback.
 */
export default function TableActionButtons({
  onEdit,
  onDelete,
  deleteConfirmTitle,
  deleteConfirmContent,
}: TableActionButtonsProps) {
  function handleDeleteClick() {
    Modal.confirm({
      title: deleteConfirmTitle,
      content: deleteConfirmContent ?? 'This action cannot be undone.',
      okText: 'Delete',
      okButtonProps: { danger: true },
      onOk: onDelete,
    })
  }

  return (
    <Space size="small">
      <Button
        size="small"
        type="primary"
        icon={<EditOutlined />}
        onClick={onEdit}
      >
        Edit
      </Button>
      <Button
        size="small"
        danger
        icon={<DeleteOutlined />}
        onClick={handleDeleteClick}
      >
        Delete
      </Button>
    </Space>
  )
}
