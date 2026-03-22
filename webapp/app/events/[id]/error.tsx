'use client'

export default function EventDetailError({
  error,
  unstable_retry,
}: {
  error: Error & { digest?: string }
  unstable_retry: () => void
}) {
  return (
    <div className="mx-auto max-w-2xl py-16 text-center">
      <h2 className="text-xl font-semibold text-zinc-50">Could not load event</h2>
      <p className="mt-2 text-sm text-zinc-400">{error.message}</p>
      <button
        onClick={unstable_retry}
        className="mt-6 rounded-md bg-indigo-500 px-4 py-2 text-sm font-semibold text-white hover:bg-indigo-400"
      >
        Try again
      </button>
    </div>
  )
}
