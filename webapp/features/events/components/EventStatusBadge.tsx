import { Badge } from '@/components/ui/Badge'

type EventStatusBadgeProps = {
  is_upcoming: boolean
  registration_open: boolean
}

export function EventStatusBadge({ is_upcoming, registration_open }: EventStatusBadgeProps) {
  return (
    <div className="flex flex-wrap gap-1.5">
      <Badge variant={is_upcoming ? 'indigo' : 'zinc'}>
        {is_upcoming ? 'Upcoming' : 'Past'}
      </Badge>
      {is_upcoming && (
        <Badge variant={registration_open ? 'green' : 'red'}>
          {registration_open ? 'Registration open' : 'Registration closed'}
        </Badge>
      )}
    </div>
  )
}
