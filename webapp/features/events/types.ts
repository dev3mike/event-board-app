export type EventSummary = {
  id: number
  title: string
  starts_at: string
  location: string
  max_participants: number
  current_registrations: number
  is_upcoming: boolean
  registration_open: boolean
}

export type EventDetail = EventSummary & {
  description: string
  registration_deadline: string | null
}

export type EventsPage = {
  items: EventSummary[]
  page: number
  page_size: number
  total_count: number
  total_pages: number
}
