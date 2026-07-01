import { Index } from 'flexsearch';
import { contentBySlug, extractExcerpt } from './content';
import { getAllNavItems } from './navigation';

export interface SearchDocument {
  slug: string;
  title: string;
  excerpt: string;
}

export const searchDocuments: SearchDocument[] = getAllNavItems()
  .filter((item) => contentBySlug[item.slug])
  .map((item) => ({
    slug: item.slug,
    title: item.title,
    excerpt: extractExcerpt(contentBySlug[item.slug]),
  }));

const index = new Index({
  tokenize: 'forward',
  cache: true,
});

searchDocuments.forEach((doc, i) => {
  index.add(i, `${doc.title} ${doc.excerpt} ${doc.slug.replace(/\//g, ' ')}`);
});

export function searchDocs(query: string, limit = 8): SearchDocument[] {
  const trimmed = query.trim();
  if (!trimmed) return [];

  const results = index.search(trimmed, { limit }) as number[];
  return results.map((i) => searchDocuments[i]).filter(Boolean);
}
