import { Suspense } from 'react'
import { getEvents } from '@/features/events/queries'
import { EventList } from '@/features/events/components/EventList'
import { EventListFilters } from '@/features/events/components/EventListFilters'
import { Skeleton } from '@/components/ui/Skeleton'

type SearchParams = Promise<{
  upcoming_only?: string
  page?: string
}>

export const metadata = {
  title: 'Events — EventBoard',
}

export default async function EventsPage({ searchParams }: { searchParams: SearchParams }) {
  const params = await searchParams
  const upcomingOnly = params.upcoming_only === 'true'
  const page = Math.max(1, parseInt(params.page ?? '1', 10) || 1)

  const data = await getEvents({ upcoming_only: upcomingOnly, page, page_size: 12 })

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-zinc-50">Events</h1>
        <p className="mt-1 text-sm text-zinc-400">
          {data.total_count} {data.total_count === 1 ? 'event' : 'events'} total
        </p>
      </div>

      <Suspense fallback={<Skeleton className="h-10 w-56" />}>
        <EventListFilters
          upcomingOnly={upcomingOnly}
          currentPage={data.page}
          totalPages={data.total_pages}
        />
      </Suspense>

      <EventList events={data.items} />

      {data.total_pages > 1 && (
        <p className="text-center text-xs text-zinc-500">
          Page {data.page} of {data.total_pages}
        </p>
      )}
    </div>
  )
}
