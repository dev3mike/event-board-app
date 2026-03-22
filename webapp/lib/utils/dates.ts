/**
 * Formats an ISO date string into a readable date/time string.
 * e.g. "Monday, 5 April 2026 at 14:30"
 */
export function formatEventDate(isoString: string): string {
  const date = new Date(isoString)
  return date.toLocaleString('en-GB', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

/**
 * Formats an ISO date string into a short date/time string.
 * e.g. "5 Apr 2026, 14:30"
 */
export function formatEventDateShort(isoString: string): string {
  const date = new Date(isoString)
  return date.toLocaleString('en-GB', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

/**
 * Computes a human-readable countdown string from now to a future ISO date.
 * e.g. "Event starts in 3 days, 4 hours"
 * If the event is in the past, returns "Event has already started".
 */
export function computeCountdown(isoString: string): string {
  const now = Date.now()
  const target = new Date(isoString).getTime()
  const diffMs = target - now

  if (diffMs <= 0) return 'Event has already started'

  const totalMinutes = Math.floor(diffMs / 60_000)
  const totalHours = Math.floor(totalMinutes / 60)
  const days = Math.floor(totalHours / 24)
  const hours = totalHours % 24

  const parts: string[] = []
  if (days > 0) parts.push(`${days} ${days === 1 ? 'day' : 'days'}`)
  if (hours > 0) parts.push(`${hours} ${hours === 1 ? 'hour' : 'hours'}`)
  if (parts.length === 0) parts.push('less than an hour')

  return `Event starts in ${parts.join(', ')}`
}
