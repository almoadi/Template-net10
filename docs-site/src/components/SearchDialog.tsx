import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { searchDocs, type SearchDocument } from '../lib/search-index';

interface SearchDialogProps {
  open: boolean;
  onClose: () => void;
}

export function SearchDialog({ open, onClose }: SearchDialogProps) {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<SearchDocument[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    if (!open) {
      setQuery('');
      setResults([]);
    }
  }, [open]);

  useEffect(() => {
    setResults(searchDocs(query));
  }, [query]);

  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && open) onClose();
    };
    window.addEventListener('keydown', handler);
    return () => window.removeEventListener('keydown', handler);
  }, [open, onClose]);

  if (!open) return null;

  const goTo = (slug: string) => {
    navigate(`/docs/${slug}`);
    onClose();
  };

  return (
    <div className="fixed inset-0 z-[100] flex items-start justify-center bg-black/50 pt-[15vh] px-4">
      <button type="button" className="absolute inset-0" onClick={onClose} aria-label="Close search" />
      <div className="relative w-full max-w-xl overflow-hidden rounded-xl border border-gray-200 bg-white shadow-2xl shadow-dotnet/10 ring-1 ring-dotnet/5 dark:border-gray-700 dark:bg-[#14121f]">
        <div className="flex items-center gap-3 border-b border-gray-200 px-4 dark:border-gray-700">
          <svg className="h-5 w-5 text-dotnet dark:text-dotnet-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input
            autoFocus
            type="search"
            placeholder="Search documentation…"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            className="flex-1 bg-transparent py-4 text-gray-900 outline-none focus:ring-0 dark:text-white"
          />
          <kbd className="rounded bg-gray-100 px-2 py-1 font-mono text-xs text-gray-500 dark:bg-gray-800">Esc</kbd>
        </div>
        <ul className="max-h-80 overflow-y-auto py-2">
          {query && results.length === 0 && (
            <li className="px-4 py-3 text-sm text-gray-500">No results found.</li>
          )}
          {results.map((doc) => (
            <li key={doc.slug}>
              <button
                type="button"
                onClick={() => goTo(doc.slug)}
                className="w-full px-4 py-3 text-left transition hover:bg-dotnet-muted/50 dark:hover:bg-dotnet/10"
              >
                <p className="font-medium text-gray-900 dark:text-white">{doc.title}</p>
                {doc.excerpt && (
                  <p className="mt-0.5 truncate text-sm text-gray-500 dark:text-gray-400">{doc.excerpt}</p>
                )}
              </button>
            </li>
          ))}
          {!query && (
            <li className="px-4 py-3 text-sm text-gray-500">Type to search all documentation pages…</li>
          )}
        </ul>
      </div>
    </div>
  );
}

// Export hook for global Ctrl+K
export function useSearchShortcut(onOpen: () => void) {
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        onOpen();
      }
    };
    window.addEventListener('keydown', handler);
    return () => window.removeEventListener('keydown', handler);
  }, [onOpen]);
}
