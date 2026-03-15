import axios, { AxiosError } from 'axios'

// Always use /api as the base — nginx proxies it to the backend
const apiClient = axios.create({
  baseURL: '/api',
  timeout: 15_000,
})

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ProblemDetails>) => {
    const problem = error.response?.data
    const message =
      problem?.detail ??
      problem?.title ??
      'An unexpected error occurred. Please try again.'
    return Promise.reject(new Error(message))
  }
)

interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
  instance?: string
  errors?: Record<string, string[]>
}

export default apiClient
