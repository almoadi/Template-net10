interface TableOfContentsProps {
  headings: { id: string; text: string; level: number }[];
}

export function TableOfContents({ headings }: TableOfContentsProps) {
  if (headings.length === 0) return null;

  return (
    <aside className="hidden xl:block">
      <div className="sticky top-24">
        <p className="mb-3 text-[11px] font-semibold uppercase tracking-wider text-dotnet-dark/70 dark:text-dotnet-light/70">
          On this page
        </p>
        <nav className="space-y-1 border-l border-gray-200 dark:border-gray-800">
          {headings.map((heading) => (
            <a
              key={heading.id}
              href={`#${heading.id}`}
              className={`block border-l-2 border-transparent py-1 text-sm text-gray-600 transition hover:border-dotnet hover:text-dotnet dark:text-gray-400 dark:hover:border-dotnet-light dark:hover:text-dotnet-light ${
                heading.level === 3 ? 'pl-6' : 'pl-4'
              }`}
            >
              {heading.text}
            </a>
          ))}
        </nav>
      </div>
    </aside>
  );
}
