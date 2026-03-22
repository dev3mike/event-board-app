import { ApiError } from './errors'
import type { ApiErrorDto } from './types'

function getBaseUrl(): string {
  const url = process.env.API_BASE_URL
  if (!url) throw new Error('API_BASE_URL environment variable is not set')
  return url
}

type FetchOptions = RequestInit & {
  next?: { tags?: string[]; revalidate?: number }
}

export async function apiFetch<T>(path: string, options?: FetchOptions): Promise<T> {
  const url = `${getBaseUrl()}${path}`

  let response: Response
  try {
    response = await fetch(url, options)
  } catch (err) {
    throw new ApiError('UnexpectedError', 'Network error: could not reach the API', 0)
  }

  if (response.ok) {
    if (response.status === 204) return undefined as T
    return response.json() as Promise<T>
  }

  let body: ApiErrorDto
  try {
    body = (await response.json()) as ApiErrorDto
  } catch {
    throw new ApiError('UnexpectedError', `Unexpected API error (${response.status})`, response.status)
  }

  throw new ApiError(body.code, body.message, response.status, body.validation_errors)
}
