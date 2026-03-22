import { EventCard } from './EventCard'
import type { EventSummary } from '../types'

type EventListProps = {
  events: EventSummary[]
}

export function EventList({ events }: EventListProps) {
  if (events.length === 0) {
    return (
      <div className="py-16 text-center text-zinc-400">
        <p className="text-lg">No events found.</p>
      </div>
    )
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {events.map((event) => (
        <EventCard key={event.id} event={event} />
      ))}
    </div>
  )
}
