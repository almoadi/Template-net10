interface HeaderProps {
  onMenuClick: () => void;
  onSearchClick: () => void;
  onThemeToggle: () => void;
  theme: 'light' | 'dark';
}

export function Header({ onMenuClick, onSearchClick, onThemeToggle, theme }: HeaderProps) {
  return (
    <header className="fixed inset-x-0 top-0 z-50 border-b border-gray-200/80 bg-white/90 backdrop-blur-md dark:border-gray-800/80 dark:bg-[#0b0a12]/90">
      <div className="mx-auto flex h-14 max-w-7xl items-center gap-4 px-4 lg:px-6">
        <button
          type="button"
          onClick={onMenuClick}
          className="rounded-lg p-2 text-gray-600 transition hover:bg-dotnet-muted/60 lg:hidden dark:text-gray-300 dark:hover:bg-dotnet/15"
          aria-label="Open menu"
        >
          <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>

        <div className="flex items-center gap-3">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-gradient-to-br from-dotnet to-dotnet-cyan shadow-sm shadow-dotnet/25">
            <svg className="h-4 w-4 text-white" viewBox="0 0 24 24" fill="currentColor" aria-hidden>
              <path d="M4.5 4.5h6.75L12 9.75 8.25 15H4.5V4.5zm9 0H20.25V15H16.5l-3.75-5.25L16.5 4.5z" />
            </svg>
          </div>
          <div className="flex items-center gap-2">
            <span className="text-lg font-semibold tracking-tight text-gray-900 dark:text-white">
              Template-net10
            </span>
            <span className="rounded-md bg-gradient-to-r from-dotnet to-dotnet-cyan px-2 py-0.5 text-[11px] font-semibold uppercase tracking-wide text-white shadow-sm">
              .NET 10
            </span>
          </div>
        </div>

        <div className="ml-auto flex items-center gap-2">
          <button
            type="button"
            onClick={onSearchClick}
            className="flex items-center gap-2 rounded-lg border border-gray-200 px-3 py-1.5 text-sm text-gray-500 transition hover:border-dotnet/30 hover:bg-dotnet-muted/40 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-dotnet/40 dark:border-gray-700 dark:text-gray-400 dark:hover:border-dotnet-light/30 dark:hover:bg-dotnet/10"
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <span className="hidden sm:inline">Search</span>
            <kbd className="hidden rounded bg-gray-100 px-1.5 py-0.5 font-mono text-xs text-gray-500 sm:inline dark:bg-gray-800 dark:text-gray-400">
              Ctrl K
            </kbd>
          </button>

          <button
            type="button"
            onClick={onThemeToggle}
            className="rounded-lg p-2 text-gray-600 transition hover:bg-dotnet-muted/60 dark:text-gray-300 dark:hover:bg-dotnet/15"
            aria-label="Toggle theme"
          >
            {theme === 'light' ? (
              <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
              </svg>
            ) : (
              <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            )}
          </button>
        </div>
      </div>
    </header>
  );
}
