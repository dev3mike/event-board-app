export type RegistrationRequest = {
  name: string
  email: string
}

export type RegistrationConfirmation = {
  registration_id: number
  event_title: string
  event_starts_at: string
  event_location: string
  registered_name: string
  registered_email: string
}

export type RegistrationActionResult =
  | { status: 'idle' }
  | { status: 'success'; data: RegistrationConfirmation }
  | { status: 'error'; code: string; message: string; field_errors?: Record<string, string[]> }
