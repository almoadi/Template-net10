import { Link, useLocation } from 'react-router-dom';
import { navigation } from '../lib/navigation';

interface SidebarProps {
  open: boolean;
  onClose: () => void;
}

export function Sidebar({ open, onClose }: SidebarProps) {
  const location = useLocation();
  const currentSlug = location.pathname.replace(/^\/docs\/?/, '');

  return (
    <>
      {open && (
        <button
          type="button"
          className="fixed inset-0 z-40 bg-black/40 lg:hidden"
          onClick={onClose}
          aria-label="Close sidebar"
        />
      )}
      <aside
        className={`fixed inset-y-0 left-0 z-50 w-72 transform overflow-y-auto border-r border-gray-200 bg-dotnet-surface pt-16 transition-transform dark:border-gray-800 dark:bg-dotnet-surface-dark lg:static lg:z-auto lg:translate-x-0 lg:pt-0 ${
          open ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <nav className="px-3 py-6">
          {navigation.map((section) => (
            <div key={section.title} className="mb-6">
              <p className="mb-2 px-3 text-[11px] font-semibold uppercase tracking-wider text-dotnet-dark/70 dark:text-dotnet-light/70">
                {section.title}
              </p>
              <ul className="space-y-0.5">
                {section.items.map((item) => {
                  const isActive = currentSlug === item.slug;
                  return (
                    <li key={item.slug}>
                      <Link
                        to={`/docs/${item.slug}`}
                        onClick={onClose}
                        className={`block rounded-lg px-3 py-2 text-sm transition ${
                          isActive
                            ? 'border-l-2 border-dotnet bg-white font-medium text-dotnet shadow-sm dark:border-dotnet-light dark:bg-dotnet/15 dark:text-dotnet-light'
                            : 'border-l-2 border-transparent text-gray-700 hover:border-dotnet/30 hover:bg-white/70 dark:text-gray-300 dark:hover:bg-dotnet/10'
                        }`}
                      >
                        {item.title}
                      </Link>
                    </li>
                  );
                })}
              </ul>
            </div>
          ))}
        </nav>
      </aside>
    </>
  );
}
