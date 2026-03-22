'use client'

export default function GlobalError({
  error,
  unstable_retry,
}: {
  error: Error & { digest?: string }
  unstable_retry: () => void
}) {
  return (
    <div className="py-24 text-center">
      <h2 className="text-2xl font-semibold text-zinc-50">Something went wrong</h2>
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
