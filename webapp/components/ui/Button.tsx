import { cn } from '@/lib/utils/cn'

type ButtonVariant = 'primary' | 'secondary' | 'ghost'

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: ButtonVariant
}

const variantClasses: Record<ButtonVariant, string> = {
  primary:
    'bg-indigo-500 text-white hover:bg-indigo-400 disabled:bg-indigo-500/40 focus-visible:outline-indigo-400',
  secondary:
    'bg-zinc-900 text-zinc-100 ring-1 ring-inset ring-white/10 hover:bg-zinc-800 disabled:opacity-50',
  ghost: 'text-zinc-400 hover:bg-white/5 hover:text-zinc-100 disabled:opacity-50',
}

export function Button({ variant = 'primary', className, children, ...props }: ButtonProps) {
  return (
    <button
      className={cn(
        'inline-flex items-center justify-center gap-2 rounded-md px-4 py-2 text-sm font-semibold',
        'transition-colors focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2',
        'disabled:cursor-not-allowed',
        variantClasses[variant],
        className,
      )}
      {...props}
    >
      {children}
    </button>
  )
}
