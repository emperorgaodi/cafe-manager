import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { employeeApi } from '../api/employeeApi'
import { CreateEmployeePayload, UpdateEmployeePayload } from '../types'

export const EMPLOYEE_QUERY_KEY = 'employees'

export function useEmployees(cafeId?: string) {
  return useQuery({
    queryKey: [EMPLOYEE_QUERY_KEY, cafeId],
    queryFn: () => employeeApi.getAll(cafeId),
  })
}

export function useCreateEmployee() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateEmployeePayload) => employeeApi.create(payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [EMPLOYEE_QUERY_KEY] }),
  })
}

export function useUpdateEmployee() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: UpdateEmployeePayload) => employeeApi.update(payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [EMPLOYEE_QUERY_KEY] }),
  })
}

export function useDeleteEmployee() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => employeeApi.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [EMPLOYEE_QUERY_KEY] }),
  })
}
