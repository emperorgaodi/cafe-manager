import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { cafeApi } from '../api/cafeApi'
import { CreateCafePayload, UpdateCafePayload } from '../types'

export const CAFE_QUERY_KEY = 'cafes'

export function useCafes(location?: string) {
  return useQuery({
    queryKey: [CAFE_QUERY_KEY, location],
    queryFn: () => cafeApi.getAll(location),
  })
}

export function useCreateCafe() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateCafePayload) => cafeApi.create(payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [CAFE_QUERY_KEY] }),
  })
}

export function useUpdateCafe() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: UpdateCafePayload) => cafeApi.update(payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [CAFE_QUERY_KEY] }),
  })
}

export function useDeleteCafe() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => cafeApi.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [CAFE_QUERY_KEY] }),
  })
}
