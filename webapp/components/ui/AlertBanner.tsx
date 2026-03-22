import { cn } from '@/lib/utils/cn'

type AlertVariant = 'error' | 'success' | 'info'

type AlertBannerProps = {
  variant: AlertVariant
  message: string
  children?: React.ReactNode
}

const variantClasses: Record<AlertVariant, string> = {
  error: 'border border-red-400/20 bg-red-500/10 text-red-100',
  success: 'border border-emerald-400/20 bg-emerald-500/10 text-emerald-100',
  info: 'border border-blue-400/20 bg-blue-500/10 text-blue-100',
}

const iconMap: Record<AlertVariant, string> = {
  error: '✕',
  success: '✓',
  info: 'ℹ',
}

export function AlertBanner({ variant, message, children }: AlertBannerProps) {
  return (
    <div className={cn('rounded-lg p-4', variantClasses[variant])}>
      <div className="flex gap-3">
        <span className="shrink-0 font-bold">{iconMap[variant]}</span>
        <div className="flex-1">
          <p className="text-sm font-medium">{message}</p>
          {children && <div className="mt-2 text-sm">{children}</div>}
        </div>
      </div>
    </div>
  )
}
