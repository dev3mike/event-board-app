import Link from 'next/link'

export default function NotFound() {
  return (
    <div className="py-24 text-center">
      <h1 className="text-4xl font-bold text-zinc-50">404</h1>
      <p className="mt-3 text-zinc-400">Page not found.</p>
      <Link href="/events" className="mt-6 inline-block text-sm font-medium text-indigo-300 hover:text-indigo-200">
        Back to events →
      </Link>
    </div>
  )
}
