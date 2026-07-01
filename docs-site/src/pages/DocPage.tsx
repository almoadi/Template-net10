import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getContent } from '../lib/content';
import { getAdjacentSlugs } from '../lib/navigation';
import { MarkdownPage } from '../components/MarkdownPage';
import { TableOfContents } from '../components/TableOfContents';
import { PageNav } from '../components/DocsLayout';

export function DocPage() {
  const { '*': slugPath } = useParams();
  const slug = slugPath ?? '';
  const content = getContent(slug);
  const [headings, setHeadings] = useState<{ id: string; text: string; level: number }[]>([]);
  const { prev, next } = getAdjacentSlugs(slug);

  if (!content) {
    return (
      <div className="py-16 text-center">
        <h1 className="mb-4 text-2xl font-bold">Page not found</h1>
        <p className="mb-6 text-gray-600 dark:text-gray-400">
          The documentation page &quot;{slug}&quot; does not exist.
        </p>
        <Link to="/docs/introduction" className="font-medium text-dotnet underline dark:text-dotnet-light">
          Go to Introduction
        </Link>
      </div>
    );
  }

  return (
    <div className="flex gap-10">
      <div className="min-w-0 flex-1 max-w-3xl">
        <MarkdownPage content={content} onHeadingsChange={setHeadings} />
        <PageNav prev={prev} next={next} />
      </div>
      <div className="w-56 shrink-0">
        <TableOfContents headings={headings} />
      </div>
    </div>
  );
}
