'use client'

import { formatEventDate } from '@/lib/utils/dates'
import { computeCountdown } from '@/lib/utils/dates'
import type { RegistrationConfirmation as RegistrationConfirmationType } from '../types'

type RegistrationConfirmationProps = {
  confirmation: RegistrationConfirmationType
}

export function RegistrationConfirmation({ confirmation }: RegistrationConfirmationProps) {
  const countdown = computeCountdown(confirmation.event_starts_at)

  return (
    <div className="space-y-4 rounded-xl border border-emerald-400/20 bg-emerald-500/10 p-6">
      <div className="flex items-center gap-3">
        <span className="flex h-10 w-10 items-center justify-center rounded-full bg-emerald-500/15 text-xl text-emerald-200">
          ✓
        </span>
        <div>
          <h3 className="text-lg font-semibold text-emerald-50">You&apos;re registered!</h3>
          <p className="text-sm text-emerald-200">{countdown}</p>
        </div>
      </div>

      <dl className="divide-y divide-white/10 rounded-lg border border-white/10 bg-zinc-950/80 text-sm">
        <div className="flex px-4 py-3 gap-4">
          <dt className="w-28 shrink-0 font-medium text-zinc-400">Event</dt>
          <dd className="text-zinc-100">{confirmation.event_title}</dd>
        </div>
        <div className="flex px-4 py-3 gap-4">
          <dt className="w-28 shrink-0 font-medium text-zinc-400">Date</dt>
          <dd className="text-zinc-100">{formatEventDate(confirmation.event_starts_at)}</dd>
        </div>
        <div className="flex px-4 py-3 gap-4">
          <dt className="w-28 shrink-0 font-medium text-zinc-400">Location</dt>
          <dd className="text-zinc-100">{confirmation.event_location}</dd>
        </div>
        <div className="flex px-4 py-3 gap-4">
          <dt className="w-28 shrink-0 font-medium text-zinc-400">Name</dt>
          <dd className="text-zinc-100">{confirmation.registered_name}</dd>
        </div>
        <div className="flex px-4 py-3 gap-4">
          <dt className="w-28 shrink-0 font-medium text-zinc-400">Email</dt>
          <dd className="text-zinc-100">{confirmation.registered_email}</dd>
        </div>
      </dl>
    </div>
  )
}
