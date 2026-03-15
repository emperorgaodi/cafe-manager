export interface Cafe {
  id: string
  name: string
  description: string
  employees: number
  logo?: string
  location: string
}

export interface Employee {
  id: string
  name: string
  emailAddress: string
  phoneNumber: string
  gender: 'Male' | 'Female'
  daysWorked: number
  cafe?: string  // Café name — matches the "cafe" field in the API spec. Blank if unassigned.
}

export interface CreateCafePayload {
  name: string
  description: string
  location: string
  logo?: File
}

export interface UpdateCafePayload extends CreateCafePayload {
  id: string
  existingLogo?: string
}

export interface CreateEmployeePayload {
  name: string
  emailAddress: string
  phoneNumber: string
  gender: string
  cafeId?: string
}

export interface UpdateEmployeePayload extends CreateEmployeePayload {
  id: string
}
