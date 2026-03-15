import axios, { AxiosError } from 'axios'

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const apiBaseUrl = (import.meta as any).env?.VITE_API_BASE_URL ?? '/api'

const apiClient = axios.create({
  baseURL: apiBaseUrl,
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
