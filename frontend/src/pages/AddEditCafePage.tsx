import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Button, Form, Space, Typography, Upload, message } from 'antd'
import { UploadOutlined } from '@ant-design/icons'
import { useCafes, useCreateCafe, useUpdateCafe } from '../hooks/useCafes'
import { useUnsavedChangesGuard } from '../hooks/useUnsavedChangesGuard'
import ReusableTextInput from '../components/ReusableTextInput'
import UnsavedBanner from '../components/UnsavedBanner'

const MAX_LOGO_SIZE_BYTES = 2 * 1024 * 1024

export default function AddEditCafePage() {
  const { id } = useParams<{ id?: string }>()
  const isEditMode = !!id
  const navigate = useNavigate()
  const [form] = Form.useForm()
  const [isDirty, setIsDirty] = useState(false)
  const [logoFile, setLogoFile] = useState<File | undefined>()

  useUnsavedChangesGuard(isDirty)

  const { data: cafes = [] } = useCafes()
  const createCafe = useCreateCafe()
  const updateCafe = useUpdateCafe()

  useEffect(() => {
    if (!isEditMode) return
    const existing = cafes.find((c) => c.id === id)
    if (existing) {
      form.setFieldsValue({
        name: existing.name,
        description: existing.description,
        location: existing.location,
      })
    }
  }, [id, cafes, form, isEditMode])

  async function handleSubmit(values: { name: string; description: string; location: string }) {
    try {
      if (isEditMode) {
        const existing = cafes.find((c) => c.id === id)
        await updateCafe.mutateAsync({ id: id!, ...values, logo: logoFile, existingLogo: existing?.logo })
        message.success('Café updated successfully.')
      } else {
        await createCafe.mutateAsync({ ...values, logo: logoFile })
        message.success('Café created successfully.')
      }
      setIsDirty(false)
      navigate('/cafes')
    } catch (err) {
      message.error(err instanceof Error ? err.message : 'Failed to save café.')
    }
  }

  function beforeUpload(file: File): boolean {
    if (file.size > MAX_LOGO_SIZE_BYTES) {
      message.error('Logo must be smaller than 2 MB.')
      return false
    }
    setLogoFile(file)
    setIsDirty(true)
    return false
  }

  return (
    <div className="page-card" style={{ maxWidth: 560 }}>
      <Typography.Title level={3} style={{ marginTop: 0, color: 'var(--color-brand)' }}>
        {isEditMode ? '✏ Edit Café' : '☕ New Café'}
      </Typography.Title>

      {isDirty && <UnsavedBanner />}

      <div className="form-card">
        <Form
          form={form}
          layout="vertical"
          onValuesChange={() => setIsDirty(true)}
          onFinish={handleSubmit}
        >
          <ReusableTextInput
            name="name"
            label="Café Name"
            placeholder="e.g. Bean Bliss"
            maxLength={10}
            rules={[
              { required: true, message: 'Name is required.' },
              { min: 6, message: 'Minimum 6 characters.' },
              { max: 10, message: 'Maximum 10 characters.' },
            ]}
          />
          <ReusableTextInput
            name="description"
            label="Description"
            placeholder="Short description of the café"
            maxLength={256}
            rules={[
              { required: true, message: 'Description is required.' },
              { max: 256, message: 'Maximum 256 characters.' },
            ]}
          />
          <ReusableTextInput
            name="location"
            label="Location"
            placeholder="e.g. Marina Bay"
            rules={[{ required: true, message: 'Location is required.' }]}
          />
          <Form.Item label="Logo (optional — max 2 MB)">
            <Upload beforeUpload={beforeUpload} maxCount={1} accept="image/*">
              <Button icon={<UploadOutlined />}>Choose File</Button>
            </Upload>
          </Form.Item>
          <Form.Item style={{ marginBottom: 0 }}>
            <Space>
              <Button
                type="primary"
                htmlType="submit"
                loading={createCafe.isPending || updateCafe.isPending}
              >
                {isEditMode ? 'Save Changes' : 'Create Café'}
              </Button>
              <Button onClick={() => navigate('/cafes')}>Cancel</Button>
            </Space>
          </Form.Item>
        </Form>
      </div>
    </div>
  )
}