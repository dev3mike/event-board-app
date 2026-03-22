import { Skeleton } from '@/components/ui/Skeleton'

export default function EventDetailLoading() {
  return (
    <div className="mx-auto max-w-2xl space-y-8">
      <Skeleton className="h-4 w-24" />
      <div className="space-y-4">
        <Skeleton className="h-9 w-3/4" />
        <div className="space-y-2">
          <Skeleton className="h-4 w-48" />
          <Skeleton className="h-4 w-40" />
        </div>
        <div className="space-y-2">
          <Skeleton className="h-4 w-full" />
          <Skeleton className="h-4 w-5/6" />
          <Skeleton className="h-4 w-4/6" />
        </div>
      </div>
      <div className="space-y-3 rounded-xl border border-white/10 bg-zinc-900/90 p-6">
        <Skeleton className="h-6 w-24" />
        <Skeleton className="h-2 w-full" />
      </div>
      <div className="space-y-4 rounded-xl border border-white/10 bg-zinc-900/90 p-6">
        <Skeleton className="h-6 w-48" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
      </div>
    </div>
  )
}
