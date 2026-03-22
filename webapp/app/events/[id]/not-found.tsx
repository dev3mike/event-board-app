import Link from 'next/link'

export default function EventNotFound() {
  return (
    <div className="py-24 text-center">
      <h1 className="text-4xl font-bold text-zinc-50">Event not found</h1>
      <p className="mt-3 text-zinc-400">This event doesn&apos;t exist or has been removed.</p>
      <Link href="/events" className="mt-6 inline-block text-sm font-medium text-indigo-300 hover:text-indigo-200">
        Back to events →
      </Link>
    </div>
  )
}
