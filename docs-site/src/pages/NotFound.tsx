import { Link } from 'react-router-dom';

export function NotFound() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center px-4">
      <h1 className="mb-4 text-4xl font-bold">404</h1>
      <p className="mb-6 text-gray-600 dark:text-gray-400">Page not found.</p>
      <Link to="/docs/introduction" className="font-medium text-dotnet underline dark:text-dotnet-light">
        Back to documentation
      </Link>
    </div>
  );
}
