import { cn } from '@/lib/utils/cn'

type BadgeVariant = 'blue' | 'green' | 'red' | 'amber' | 'zinc' | 'indigo'

type BadgeProps = {
  variant: BadgeVariant
  children: React.ReactNode
  className?: string
}

const variantClasses: Record<BadgeVariant, string> = {
  blue: 'bg-blue-500/15 text-blue-200 ring-1 ring-inset ring-blue-400/20',
  green: 'bg-emerald-500/15 text-emerald-200 ring-1 ring-inset ring-emerald-400/20',
  red: 'bg-red-500/15 text-red-200 ring-1 ring-inset ring-red-400/20',
  amber: 'bg-amber-500/15 text-amber-200 ring-1 ring-inset ring-amber-400/20',
  zinc: 'bg-white/8 text-zinc-300 ring-1 ring-inset ring-white/10',
  indigo: 'bg-indigo-500/15 text-indigo-200 ring-1 ring-inset ring-indigo-400/20',
}

export function Badge({ variant, children, className }: BadgeProps) {
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium',
        variantClasses[variant],
        className,
      )}
    >
      {children}
    </span>
  )
}
