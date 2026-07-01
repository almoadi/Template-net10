import { useEffect, useState } from 'react';
import { codeToHtml } from 'shiki';

interface CodeBlockProps {
  code: string;
  language?: string;
}

export function CodeBlock({ code, language = 'text' }: CodeBlockProps) {
  const [html, setHtml] = useState<string>('');
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    let cancelled = false;
    codeToHtml(code, {
      lang: language,
      theme: 'github-dark',
    }).then((result) => {
      if (!cancelled) setHtml(result);
    }).catch(() => {
      if (!cancelled) {
        setHtml(`<pre><code>${code.replace(/</g, '&lt;')}</code></pre>`);
      }
    });
    return () => { cancelled = true; };
  }, [code, language]);

  const copy = async () => {
    await navigator.clipboard.writeText(code);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="group relative mb-6 overflow-hidden rounded-xl border border-gray-800 bg-[#0d1117] shadow-lg shadow-dotnet/5 ring-1 ring-dotnet/10">
      <div className="flex items-center justify-between border-b border-gray-800 bg-gradient-to-r from-[#161b22] to-[#12101a] px-4 py-2.5">
        <div className="flex items-center gap-2">
          <span className="h-2 w-2 rounded-full bg-dotnet" />
          <span className="font-mono text-xs uppercase tracking-wide text-gray-400">
            {language}
          </span>
        </div>
        <button
          type="button"
          onClick={copy}
          className="rounded-md px-2.5 py-1 text-xs text-gray-400 transition hover:bg-dotnet/20 hover:text-dotnet-light"
        >
          {copied ? 'Copied!' : 'Copy'}
        </button>
      </div>
      <div
        className="overflow-x-auto p-4 text-sm [&_pre]:!m-0 [&_pre]:!bg-transparent [&_code]:font-mono"
        dangerouslySetInnerHTML={{ __html: html }}
      />
    </div>
  );
}
