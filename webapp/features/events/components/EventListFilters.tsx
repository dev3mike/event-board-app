'use client'

import { useRouter, useSearchParams } from 'next/navigation'
import { cn } from '@/lib/utils/cn'

type EventListFiltersProps = {
  upcomingOnly: boolean
  currentPage: number
  totalPages: number
}

export function EventListFilters({ upcomingOnly, currentPage, totalPages }: EventListFiltersProps) {
  const router = useRouter()
  const searchParams = useSearchParams()

  function buildUrl(updates: Record<string, string | null>): string {
    const params = new URLSearchParams(searchParams.toString())
    for (const [key, value] of Object.entries(updates)) {
      if (value === null) {
        params.delete(key)
      } else {
        params.set(key, value)
      }
    }
    const qs = params.toString()
    return qs ? `/events?${qs}` : '/events'
  }

  function handleToggle() {
    router.push(buildUrl({ upcoming_only: upcomingOnly ? null : 'true', page: null }))
  }

  function handlePage(page: number) {
    router.push(buildUrl({ page: String(page) }))
  }

  return (
    <div className="flex flex-wrap items-center justify-between gap-4">
      <button
        onClick={handleToggle}
        className={cn(
          'inline-flex items-center gap-2 rounded-lg border px-4 py-2 text-sm font-medium transition-colors',
          upcomingOnly
            ? 'border-indigo-400/30 bg-indigo-500/15 text-indigo-100 hover:bg-indigo-500/20'
            : 'border-white/10 bg-zinc-900 text-zinc-200 hover:bg-zinc-800',
        )}
      >
        <span
          className={cn(
            'inline-block h-2 w-2 rounded-full',
            upcomingOnly ? 'bg-indigo-300' : 'bg-zinc-500',
          )}
        />
        {upcomingOnly ? 'Showing upcoming only' : 'Show all events'}
      </button>

      {totalPages > 1 && (
        <div className="flex items-center gap-2">
          <button
            onClick={() => handlePage(currentPage - 1)}
            disabled={currentPage <= 1}
            className="rounded-md border border-white/10 bg-zinc-900 px-3 py-1.5 text-sm text-zinc-300 hover:bg-zinc-800 disabled:cursor-not-allowed disabled:opacity-40"
          >
            ← Prev
          </button>
          <span className="text-sm text-zinc-400">
            Page {currentPage} of {totalPages}
          </span>
          <button
            onClick={() => handlePage(currentPage + 1)}
            disabled={currentPage >= totalPages}
            className="rounded-md border border-white/10 bg-zinc-900 px-3 py-1.5 text-sm text-zinc-300 hover:bg-zinc-800 disabled:cursor-not-allowed disabled:opacity-40"
          >
            Next →
          </button>
        </div>
      )}
    </div>
  )
}
