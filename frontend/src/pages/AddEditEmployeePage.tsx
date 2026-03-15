import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Button, Form, Radio, Select, Space, Typography, message } from 'antd'
import dayjs from 'dayjs'
import { useCafes } from '../hooks/useCafes'
import { useCreateEmployee, useEmployees, useUpdateEmployee } from '../hooks/useEmployees'
import { useUnsavedChangesGuard } from '../hooks/useUnsavedChangesGuard'
import ReusableTextInput from '../components/ReusableTextInput'
import UnsavedBanner from '../components/UnsavedBanner'

export default function AddEditEmployeePage() {
  const { id } = useParams<{ id?: string }>()
  const isEditMode = !!id
  const navigate = useNavigate()
  const [form] = Form.useForm()
  const [isDirty, setIsDirty] = useState(false)

  useUnsavedChangesGuard(isDirty)

  const { data: cafes = [] } = useCafes()
  const { data: employees = [] } = useEmployees()
  const createEmployee = useCreateEmployee()
  const updateEmployee = useUpdateEmployee()

  useEffect(() => {
    if (!isEditMode) return
    const existing = employees.find((e) => e.id === id)
    if (existing) {
      const matchedCafe = cafes.find((c) => c.name === existing.cafe)
      form.setFieldsValue({
        name: existing.name,
        emailAddress: existing.emailAddress,
        phoneNumber: existing.phoneNumber,
        gender: existing.gender,
        cafeId: matchedCafe?.id,
      })
    }
  }, [id, employees, cafes, form, isEditMode])

  async function handleSubmit(values: {
    name: string
    emailAddress: string
    phoneNumber: string
    gender: string
    cafeId?: string
  }) {
    try {
      if (isEditMode) {
        await updateEmployee.mutateAsync({ id: id!, ...values })
        message.success('Employee updated successfully.')
      } else {
        await createEmployee.mutateAsync(values)
        message.success('Employee created successfully.')
      }
      setIsDirty(false)
      navigate('/employees')
    } catch (err) {
      message.error(err instanceof Error ? err.message : 'Failed to save employee.')
    }
  }

  return (
    <div className="page-card" style={{ maxWidth: 560 }}>
      <Typography.Title level={3} style={{ marginTop: 0, color: 'var(--color-brand)' }}>
        {isEditMode ? '✏ Edit Employee' : '👤 New Employee'}
      </Typography.Title>

      {isDirty && <UnsavedBanner />}

      {!isEditMode && (
        <p style={{ color: 'var(--color-text-muted)', fontSize: 13, marginBottom: 16 }}>
          Start date will be set to today: <strong>{dayjs().format('D MMM YYYY')}</strong>
        </p>
      )}

      <div className="form-card">
        <Form
          form={form}
          layout="vertical"
          onValuesChange={() => setIsDirty(true)}
          onFinish={handleSubmit}
        >
          <ReusableTextInput
            name="name"
            label="Full Name"
            placeholder="e.g. Alice Tan"
            maxLength={30}
            rules={[
              { required: true, message: 'Name is required.' },
              { min: 6, message: 'Minimum 6 characters.' },
              { max: 30, message: 'Maximum 30 characters.' },
            ]}
          />
          <ReusableTextInput
            name="emailAddress"
            label="Email Address"
            placeholder="employee@example.com"
            rules={[
              { required: true, message: 'Email is required.' },
              { type: 'email', message: 'Enter a valid email address.' },
            ]}
          />
          <ReusableTextInput
            name="phoneNumber"
            label="Phone Number"
            placeholder="8 or 9, followed by 7 digits"
            rules={[
              { required: true, message: 'Phone number is required.' },
              { pattern: /^[89]\d{7}$/, message: 'Must start with 8 or 9 and be exactly 8 digits.' },
            ]}
          />
          <Form.Item
            name="gender"
            label="Gender"
            rules={[{ required: true, message: 'Gender is required.' }]}
          >
            <Radio.Group>
              <Radio value="Male">Male</Radio>
              <Radio value="Female">Female</Radio>
            </Radio.Group>
          </Form.Item>
          <Form.Item name="cafeId" label="Assigned Café">
            <Select
              placeholder="Select a café (optional)"
              allowClear
              options={cafes.map((c) => ({ value: c.id, label: c.name }))}
            />
          </Form.Item>
          <Form.Item style={{ marginBottom: 0 }}>
            <Space>
              <Button
                type="primary"
                htmlType="submit"
                loading={createEmployee.isPending || updateEmployee.isPending}
              >
                {isEditMode ? 'Save Changes' : 'Create Employee'}
              </Button>
              <Button onClick={() => navigate('/employees')}>Cancel</Button>
            </Space>
          </Form.Item>
        </Form>
      </div>
    </div>
  )
}