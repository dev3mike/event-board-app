'use client'

import { AlertBanner } from '@/components/ui/AlertBanner'
import { SubmitButton } from './SubmitButton'
import type { RegistrationActionResult } from '../types'

type RegistrationFormProps = {
  formAction: (payload: FormData) => void
  state: RegistrationActionResult
}

export function RegistrationForm({ formAction, state }: RegistrationFormProps) {
  const fieldErrors = state.status === 'error' ? (state.field_errors ?? {}) : {}

  return (
    <div className="space-y-5">
      <h3 className="text-lg font-semibold text-zinc-50">Register for this event</h3>

      {state.status === 'error' && (
        <AlertBanner variant="error" message={state.message} />
      )}

      <form action={formAction} className="space-y-4">
        <div>
          <label htmlFor="name" className="block text-sm font-medium text-zinc-300">
            Full name
          </label>
          <input
            id="name"
            name="name"
            type="text"
            autoComplete="name"
            required
            className="mt-1 block w-full rounded-md border border-white/10 bg-zinc-950 px-3 py-2 text-sm text-zinc-100 placeholder-zinc-500 shadow-sm focus:border-indigo-400 focus:outline-none focus:ring-1 focus:ring-indigo-400"
            placeholder="Jane Smith"
          />
          {fieldErrors['name'] && (
            <p className="mt-1 text-xs text-red-300">{fieldErrors['name'][0]}</p>
          )}
        </div>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-zinc-300">
            Email address
          </label>
          <input
            id="email"
            name="email"
            type="email"
            autoComplete="email"
            required
            className="mt-1 block w-full rounded-md border border-white/10 bg-zinc-950 px-3 py-2 text-sm text-zinc-100 placeholder-zinc-500 shadow-sm focus:border-indigo-400 focus:outline-none focus:ring-1 focus:ring-indigo-400"
            placeholder="jane@example.com"
          />
          {fieldErrors['email'] && (
            <p className="mt-1 text-xs text-red-300">{fieldErrors['email'][0]}</p>
          )}
        </div>

        <SubmitButton />
      </form>
    </div>
  )
}
