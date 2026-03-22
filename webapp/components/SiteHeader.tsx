import Link from 'next/link'

export function SiteHeader() {
  return (
    <header className="border-b border-white/10 bg-zinc-950/80 backdrop-blur">
      <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-4">
        <Link href="/events" className="text-xl font-bold text-indigo-300 hover:text-indigo-200">
          EventBoard
        </Link>
        <nav>
          <Link
            href="/events"
            className="text-sm font-medium text-zinc-400 transition-colors hover:text-zinc-100"
          >
            Events
          </Link>
        </nav>
      </div>
    </header>
  )
}
