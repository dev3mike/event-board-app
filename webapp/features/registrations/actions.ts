'use server'

import { updateTag } from 'next/cache'
import { apiFetch } from '@/lib/api/client'
import { ApiError } from '@/lib/api/errors'
import { EVENTS_LIST_TAG, eventDetailTag } from '@/features/events/cache-tags'
import type { RegistrationActionResult, RegistrationConfirmation } from './types'

const ERROR_MESSAGES: Record<string, string> = {
  RegistrationClosed: 'Registration for this event has closed.',
  EventFull: 'Sorry, this event is now full.',
  DuplicateRegistration: 'This email is already registered for this event.',
  UnexpectedError: 'Something went wrong. Please try again.',
}

function errorMessage(code: string): string {
  return ERROR_MESSAGES[code] ?? 'Something went wrong. Please try again.'
}

export async function registerForEvent(
  eventId: number,
  _prevState: RegistrationActionResult,
  formData: FormData,
): Promise<RegistrationActionResult> {
  const name = (formData.get('name') as string | null)?.trim() ?? ''
  const email = (formData.get('email') as string | null)?.trim().toLowerCase() ?? ''

  // Fast pre-validation before touching the API
  const fieldErrors: Record<string, string[]> = {}
  if (!name) fieldErrors['name'] = ['Name is required.']
  if (!email) {
    fieldErrors['email'] = ['Email is required.']
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    fieldErrors['email'] = ['Please enter a valid email address.']
  }

  if (Object.keys(fieldErrors).length > 0) {
    return {
      status: 'error',
      code: 'ValidationFailed',
      message: 'Please correct the errors below.',
      field_errors: fieldErrors,
    }
  }

  try {
    const confirmation = await apiFetch<RegistrationConfirmation>(
      `/api/events/${eventId}/registrations`,
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name, email }),
      },
    )

    // Invalidate caches so capacity counts reflect the new registration
    updateTag(eventDetailTag(eventId))
    updateTag(EVENTS_LIST_TAG)

    return { status: 'success', data: confirmation }
  } catch (err) {
    if (err instanceof ApiError) {
      if (err.code === 'ValidationFailed') {
        return {
          status: 'error',
          code: err.code,
          message: 'Please correct the errors below.',
          field_errors: err.validation_errors,
        }
      }
      return {
        status: 'error',
        code: err.code,
        message: errorMessage(err.code),
      }
    }

    return {
      status: 'error',
      code: 'UnexpectedError',
      message: errorMessage('UnexpectedError'),
    }
  }
}
