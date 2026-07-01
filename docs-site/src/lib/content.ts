const modules = import.meta.glob('../../content/**/*.md', {
  query: '?raw',
  import: 'default',
  eager: true,
}) as Record<string, string>;

function pathToSlug(path: string): string {
  return path
    .replace('../../content/', '')
    .replace(/\.md$/, '');
}

export const contentBySlug: Record<string, string> = Object.fromEntries(
  Object.entries(modules).map(([path, content]) => [pathToSlug(path), content]),
);

export function getContent(slug: string): string | undefined {
  return contentBySlug[slug];
}

export function getAllSlugs(): string[] {
  return Object.keys(contentBySlug);
}

export function extractTitle(markdown: string): string {
  const match = markdown.match(/^#\s+(.+)$/m);
  return match?.[1]?.trim() ?? 'Untitled';
}

export function extractHeadings(markdown: string): { id: string; text: string; level: number }[] {
  const headings: { id: string; text: string; level: number }[] = [];
  const regex = /^(#{2,3})\s+(.+)$/gm;
  let match: RegExpExecArray | null;
  while ((match = regex.exec(markdown)) !== null) {
    const level = match[1].length;
    const text = match[2].replace(/\*\*/g, '').trim();
    const id = text
      .toLowerCase()
      .replace(/[^\w\s-]/g, '')
      .replace(/\s+/g, '-');
    headings.push({ id, text, level });
  }
  return headings;
}

export function extractExcerpt(markdown: string, maxLength = 160): string {
  const lines = markdown.split('\n');
  for (const line of lines) {
    const trimmed = line.trim();
    if (trimmed && !trimmed.startsWith('#') && !trimmed.startsWith('>')) {
      return trimmed.length > maxLength ? `${trimmed.slice(0, maxLength)}…` : trimmed;
    }
  }
  return '';
}
