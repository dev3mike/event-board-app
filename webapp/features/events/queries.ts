import { apiFetch } from '@/lib/api/client'
import { EVENTS_LIST_TAG, eventDetailTag } from './cache-tags'
import type { EventDetail, EventsPage } from './types'

type GetEventsParams = {
  upcoming_only?: boolean
  page?: number
  page_size?: number
}

export async function getEvents(params: GetEventsParams = {}): Promise<EventsPage> {
  const query = new URLSearchParams()
  if (params.upcoming_only != null) query.set('upcoming_only', String(params.upcoming_only))
  if (params.page != null) query.set('page', String(params.page))
  if (params.page_size != null) query.set('page_size', String(params.page_size))

  const qs = query.toString()
  const path = qs ? `/api/events?${qs}` : '/api/events'

  return apiFetch<EventsPage>(path, {
    next: { tags: [EVENTS_LIST_TAG], revalidate: 60 },
  })
}

export async function getEvent(id: number): Promise<EventDetail> {
  return apiFetch<EventDetail>(`/api/events/${id}`, {
    next: { tags: [eventDetailTag(id)], revalidate: 60 },
  })
}
