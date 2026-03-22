import { notFound } from 'next/navigation'
import { getEvent } from '@/features/events/queries'
import { ApiError } from '@/lib/api/errors'
import { formatEventDate } from '@/lib/utils/dates'
import { EventStatusBadge } from '@/features/events/components/EventStatusBadge'
import { CapacityBar } from '@/features/events/components/CapacityBar'
import { EventRegistrationSection } from '@/features/registrations/components/EventRegistrationSection'
import Link from 'next/link'

type Params = Promise<{ id: string }>

export async function generateMetadata({ params }: { params: Params }) {
  const { id } = await params
  try {
    const event = await getEvent(parseInt(id, 10))
    return { title: `${event.title} — EventBoard` }
  } catch {
    return { title: 'Event — EventBoard' }
  }
}

export default async function EventDetailPage({ params }: { params: Params }) {
  const { id } = await params
  const eventId = parseInt(id, 10)

  if (isNaN(eventId)) notFound()

  let event
  try {
    event = await getEvent(eventId)
  } catch (err) {
    if (err instanceof ApiError && err.code === 'NotFound') notFound()
    throw err
  }

  const spotsLeft = Math.max(0, event.max_participants - event.current_registrations)

  return (
    <div className="mx-auto max-w-2xl space-y-8">
      <div>
        <Link href="/events" className="text-sm text-zinc-400 hover:text-zinc-100">
          ← Back to events
        </Link>
      </div>

      <div className="space-y-4">
        <div className="flex flex-wrap items-start justify-between gap-3">
          <h1 className="text-3xl font-bold text-zinc-50">{event.title}</h1>
          <EventStatusBadge
            is_upcoming={event.is_upcoming}
            registration_open={event.registration_open}
          />
        </div>

        <div className="flex flex-col gap-2 text-sm text-zinc-400">
          <span>📅 {formatEventDate(event.starts_at)}</span>
          <span>📍 {event.location}</span>
          {event.registration_deadline && (
            <span>⏰ Registration deadline: {formatEventDate(event.registration_deadline)}</span>
          )}
        </div>

        {event.description && (
          <p className="text-base leading-relaxed text-zinc-300">{event.description}</p>
        )}
      </div>

      <div className="space-y-4 rounded-xl border border-white/10 bg-zinc-900/90 p-6">
        <h2 className="text-lg font-semibold text-zinc-50">Capacity</h2>
        <CapacityBar current={event.current_registrations} max={event.max_participants} />
        {event.is_upcoming && event.registration_open && spotsLeft > 0 && spotsLeft <= 5 && (
          <p className="text-sm font-medium text-amber-200">
            Only {spotsLeft} {spotsLeft === 1 ? 'spot' : 'spots'} remaining, register soon.
          </p>
        )}
      </div>

      <div className="rounded-xl border border-white/10 bg-zinc-900/90 p-6">
        <EventRegistrationSection
          eventId={eventId}
          registrationOpen={event.registration_open}
          isUpcoming={event.is_upcoming}
        />
      </div>
    </div>
  )
}
