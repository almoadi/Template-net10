import { useState } from 'react';
import { Link, Outlet } from 'react-router-dom';
import { Header } from './Header';
import { Sidebar } from './Sidebar';
import { SearchDialog, useSearchShortcut } from './SearchDialog';
import { useTheme } from '../lib/useTheme';

export function DocsLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [searchOpen, setSearchOpen] = useState(false);
  const { theme, toggleTheme } = useTheme();

  useSearchShortcut(() => setSearchOpen(true));

  return (
    <div className="min-h-screen">
      <Header
        onMenuClick={() => setSidebarOpen(true)}
        onSearchClick={() => setSearchOpen(true)}
        onThemeToggle={toggleTheme}
        theme={theme}
      />
      <SearchDialog open={searchOpen} onClose={() => setSearchOpen(false)} />
      <div className="mx-auto flex max-w-7xl pt-14 lg:gap-8">
        <Sidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} />
        <main className="min-w-0 flex-1 px-4 py-8 lg:px-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

interface PageNavProps {
  prev?: { title: string; slug: string };
  next?: { title: string; slug: string };
}

export function PageNav({ prev, next }: PageNavProps) {
  if (!prev && !next) return null;

  return (
    <nav className="mt-12 flex items-center justify-between border-t border-gray-200 pt-8 dark:border-gray-800">
      {prev ? (
        <Link
          to={`/docs/${prev.slug}`}
          className="group flex flex-col text-sm text-gray-600 hover:text-dotnet dark:text-gray-400"
        >
          <span className="text-xs uppercase tracking-wide">Previous</span>
          <span className="font-medium group-hover:text-dotnet dark:group-hover:text-dotnet-light">
            ← {prev.title}
          </span>
        </Link>
      ) : (
        <span />
      )}
      {next ? (
        <Link
          to={`/docs/${next.slug}`}
          className="group flex flex-col text-right text-sm text-gray-600 hover:text-dotnet dark:text-gray-400"
        >
          <span className="text-xs uppercase tracking-wide">Next</span>
          <span className="font-medium group-hover:text-dotnet dark:group-hover:text-dotnet-light">
            {next.title} →
          </span>
        </Link>
      ) : (
        <span />
      )}
    </nav>
  );
}
