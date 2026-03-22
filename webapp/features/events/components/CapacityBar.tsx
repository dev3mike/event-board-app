import { cn } from '@/lib/utils/cn'
import { Badge } from '@/components/ui/Badge'

type CapacityBarProps = {
  current: number
  max: number
}

function getBarColor(ratio: number): string {
  if (ratio >= 1) return 'bg-red-500'
  if (ratio >= 0.8) return 'bg-red-400'
  if (ratio >= 0.5) return 'bg-amber-400'
  return 'bg-green-500'
}

export function CapacityBar({ current, max }: CapacityBarProps) {
  const ratio = max > 0 ? current / max : 0
  const isFull = current >= max
  const remaining = Math.max(0, max - current)
  const barColor = getBarColor(ratio)

  return (
    <div className="space-y-1.5">
      <div className="flex items-center justify-between text-xs text-zinc-400">
        <span>
          {current} / {max} registered
        </span>
        {isFull ? (
          <Badge variant="red">Full</Badge>
        ) : (
          <span className={cn(remaining <= 3 ? 'font-semibold text-amber-300' : '')}>
            {remaining} {remaining === 1 ? 'spot' : 'spots'} left
          </span>
        )}
      </div>
      <div className="h-1.5 w-full overflow-hidden rounded-full bg-white/10">
        <div
          className={cn('h-full rounded-full transition-all', barColor)}
          style={{ width: `${Math.min(ratio * 100, 100)}%` }}
        />
      </div>
    </div>
  )
}
