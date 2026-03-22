'use client'

import { useActionState } from 'react'
import { AlertBanner } from '@/components/ui/AlertBanner'
import { registerForEvent } from '../actions'
import { RegistrationConfirmation } from './RegistrationConfirmation'
import { RegistrationForm } from './RegistrationForm'
import type { RegistrationActionResult } from '../types'

type EventRegistrationSectionProps = {
  eventId: number
  registrationOpen: boolean
  isUpcoming: boolean
}

const INITIAL_STATE: RegistrationActionResult = { status: 'idle' }

export function EventRegistrationSection({
  eventId,
  registrationOpen,
  isUpcoming,
}: EventRegistrationSectionProps) {
  const boundAction = registerForEvent.bind(null, eventId)
  const [state, formAction] = useActionState(boundAction, INITIAL_STATE)

  // Success state must live here so it survives RSC refresh that sets registration_open
  // to false immediately after the last seat is taken (before a child useEffect could run).
  if (state.status === 'success') {
    return <RegistrationConfirmation confirmation={state.data} />
  }

  if (registrationOpen) {
    return <RegistrationForm formAction={formAction} state={state} />
  }

  return (
    <AlertBanner
      variant="info"
      message={
        isUpcoming
          ? 'Registration is closed for this event. The deadline has passed.'
          : 'This event has already taken place.'
      }
    />
  )
}
