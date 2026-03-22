export type ApiErrorDto = {
  code: string
  message: string
  validation_errors?: Record<string, string[]>
}

export type PaginatedResponse<T> = {
  items: T[]
  page: number
  page_size: number
  total_count: number
  total_pages: number
}
