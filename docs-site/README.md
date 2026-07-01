# Template-net10 Documentation Site

Standalone React documentation site for Template-net10. Content lives in markdown files under `content/` — no database required.

## Prerequisites

- Node.js 18+

## Development

```powershell
cd docs-site
npm install
npm run dev
```

Open the URL printed by Vite (typically `http://localhost:5173`).

## Build for Production

```powershell
npm run build
npm run preview
```

Static files are output to `dist/` and can be deployed to any static host (Netlify, Vercel, Azure Static Web Apps, nginx, etc.).

## Adding a Page

1. Create a markdown file under `content/` (e.g. `content/my-feature/overview.md`).
2. Add an entry to `src/lib/navigation.ts` with the matching slug (`my-feature/overview`).
3. The page is automatically available at `/docs/my-feature/overview`.

## Features

- Laravel-inspired layout (sidebar, header, TOC)
- Syntax-highlighted code blocks with copy button
- Client-side search (Ctrl+K)
- Light / dark theme
- Mobile-friendly collapsible sidebar

## Tech Stack

Vite + React + TypeScript + Tailwind CSS + react-markdown + shiki + flexsearch
