import Link from 'next/link'
import { cn } from '@/lib/utils/cn'
import { formatEventDateShort } from '@/lib/utils/dates'
import { EventStatusBadge } from './EventStatusBadge'
import { CapacityBar } from './CapacityBar'
import type { EventSummary } from '../types'

type EventCardProps = {
  event: EventSummary
}

export function EventCard({ event }: EventCardProps) {
  return (
    <Link
      href={`/events/${event.id}`}
      className={cn(
        'group flex flex-col gap-3 rounded-xl border p-5 transition-all hover:-translate-y-0.5 hover:shadow-[0_20px_45px_-30px_rgba(99,102,241,0.6)]',
        event.is_upcoming
          ? 'border-white/10 bg-zinc-900/90 hover:border-indigo-400/40'
          : 'border-white/5 bg-zinc-950/80 opacity-80',
      )}
    >
      <div className="flex items-start justify-between gap-3">
        <h2
          className={cn(
            'text-base font-semibold leading-snug',
            event.is_upcoming ? 'text-zinc-100 group-hover:text-indigo-200' : 'text-zinc-500',
          )}
        >
          {event.title}
        </h2>
        <EventStatusBadge
          is_upcoming={event.is_upcoming}
          registration_open={event.registration_open}
        />
      </div>

      <div className="flex flex-col gap-1 text-sm text-zinc-400">
        <span>📅 {formatEventDateShort(event.starts_at)}</span>
        <span>📍 {event.location}</span>
      </div>

      <CapacityBar current={event.current_registrations} max={event.max_participants} />
    </Link>
  )
}
