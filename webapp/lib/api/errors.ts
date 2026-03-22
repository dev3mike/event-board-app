export type ApiErrorCode =
  | 'NotFound'
  | 'ValidationFailed'
  | 'RegistrationClosed'
  | 'EventFull'
  | 'DuplicateRegistration'
  | 'UnexpectedError'

export class ApiError extends Error {
  constructor(
    public readonly code: ApiErrorCode | string,
    message: string,
    public readonly status: number,
    public readonly validation_errors?: Record<string, string[]>,
  ) {
    super(message)
    this.name = 'ApiError'
  }
}
