import { Cafe, CreateCafePayload, UpdateCafePayload } from '../types'
import apiClient from './client'

export const cafeApi = {
  getAll: async (location?: string): Promise<Cafe[]> => {
    const params = location ? { location } : {}
    const { data } = await apiClient.get('/v1/cafes', { params })
    return data
  },

  create: async (payload: CreateCafePayload): Promise<{ id: string }> => {
    const { data } = await apiClient.post('/v1/cafes', buildFormData(payload))
    return data
  },

  update: async (payload: UpdateCafePayload): Promise<void> => {
    const form = buildFormData(payload)
    if (payload.existingLogo) form.append('existingLogo', payload.existingLogo)
    await apiClient.put(`/v1/cafes/${payload.id}`, form)
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/v1/cafes/${id}`)
  },
}

function buildFormData(payload: CreateCafePayload): FormData {
  const form = new FormData()
  form.append('name', payload.name)
  form.append('description', payload.description)
  form.append('location', payload.location)
  if (payload.logo) form.append('logo', payload.logo)
  return form
}
