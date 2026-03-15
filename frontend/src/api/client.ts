import axios, { AxiosError } from 'axios'

// In Railway the browser calls the backend directly.
// VITE_API_BASE_URL is set at build time via Railway environment variables.
// Falls back to /api for local Docker Compose (where nginx proxies it).
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
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
