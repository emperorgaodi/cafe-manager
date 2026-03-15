import { Form, Input } from 'antd'
import type { FormItemProps } from 'antd'

interface ReusableTextInputProps extends FormItemProps {
  placeholder?: string
  maxLength?: number
  disabled?: boolean
}

/**
 * A reusable Ant Design form text input that wraps Form.Item + Input together.
 * Keeps form field boilerplate out of individual forms.
 */
export default function ReusableTextInput({
  placeholder,
  maxLength,
  disabled,
  ...formItemProps
}: ReusableTextInputProps) {
  return (
    <Form.Item {...formItemProps}>
      <Input
        placeholder={placeholder}
        maxLength={maxLength}
        disabled={disabled}
        showCount={!!maxLength}
      />
    </Form.Item>
  )
}
