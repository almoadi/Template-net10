import { useEffect, useMemo } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { Link } from 'react-router-dom';
import { CodeBlock } from './CodeBlock';

interface MarkdownPageProps {
  content: string;
  onHeadingsChange?: (headings: { id: string; text: string; level: number }[]) => void;
}

function slugify(text: string): string {
  return text
    .replace(/[^\w\s-]/g, '')
    .trim()
    .toLowerCase()
    .replace(/\s+/g, '-');
}

export function MarkdownPage({ content, onHeadingsChange }: MarkdownPageProps) {
  const headings = useMemo(() => {
    const result: { id: string; text: string; level: number }[] = [];
    const regex = /^(#{2,3})\s+(.+)$/gm;
    let match: RegExpExecArray | null;
    while ((match = regex.exec(content)) !== null) {
      const level = match[1].length;
      const text = match[2].replace(/\*\*/g, '').trim();
      result.push({ id: slugify(text), text, level });
    }
    return result;
  }, [content]);

  useEffect(() => {
    onHeadingsChange?.(headings);
  }, [headings, onHeadingsChange]);

  return (
    <article className="prose-docs">
      <ReactMarkdown
        remarkPlugins={[remarkGfm]}
        components={{
          h2: ({ children }) => {
            const text = String(children);
            return <h2 id={slugify(text)}>{children}</h2>;
          },
          h3: ({ children }) => {
            const text = String(children);
            return <h3 id={slugify(text)}>{children}</h3>;
          },
          a: ({ href, children }) => {
            if (href?.startsWith('/docs/')) {
              return (
                <Link to={href} className="text-dotnet underline dark:text-dotnet-light">
                  {children}
                </Link>
              );
            }
            return (
              <a href={href} target="_blank" rel="noopener noreferrer">
                {children}
              </a>
            );
          },
          pre: ({ children }) => {
            const child = children as React.ReactElement<{ className?: string; children?: string }>;
            const className = child?.props?.className ?? '';
            const langMatch = className.match(/language-(\w+)/);
            const language = langMatch?.[1] ?? 'text';
            const code = String(child?.props?.children ?? '').replace(/\n$/, '');
            return <CodeBlock code={code} language={language} />;
          },
          table: ({ children }) => (
            <div className="mb-6 max-w-full overflow-x-auto">
              <table>{children}</table>
            </div>
          ),
          code: ({ className, children, ...props }) => {
            if (className?.startsWith('language-')) {
              return <code className={className} {...props}>{children}</code>;
            }
            return (
              <code className={className} {...props}>
                {children}
              </code>
            );
          },
        }}
      >
        {content}
      </ReactMarkdown>
    </article>
  );
}
