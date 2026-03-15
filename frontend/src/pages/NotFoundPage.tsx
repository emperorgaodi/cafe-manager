import { useNavigate } from 'react-router-dom'
import { Button, Result } from 'antd'

export default function NotFoundPage() {
  const navigate = useNavigate()
  return (
    <div className="flex items-center justify-center min-h-[60vh]">
      <Result
        status="404"
        title="404"
        subTitle="The page you are looking for does not exist."
        extra={
          <Button type="primary" onClick={() => navigate('/cafes')}>
            Back to Cafés
          </Button>
        }
      />
    </div>
  )
}
