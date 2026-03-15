import { CreateEmployeePayload, Employee, UpdateEmployeePayload } from '../types'
import apiClient from './client'

export const employeeApi = {
  getAll: async (cafeId?: string): Promise<Employee[]> => {
    const params = cafeId ? { cafe: cafeId } : {}
    const { data } = await apiClient.get('/v1/employees', { params })
    return data
  },

  create: async (payload: CreateEmployeePayload): Promise<{ id: string }> => {
    const { data } = await apiClient.post('/v1/employees', payload)
    return data
  },

  update: async (payload: UpdateEmployeePayload): Promise<void> => {
    const { id, ...body } = payload
    await apiClient.put(`/v1/employees/${id}`, body)
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/v1/employees/${id}`)
  },
}
