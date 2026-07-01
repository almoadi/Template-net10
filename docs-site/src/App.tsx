import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { DocsLayout } from './components/DocsLayout';
import { DocPage } from './pages/DocPage';
import { NotFound } from './pages/NotFound';

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/docs/introduction" replace />} />
        <Route path="/docs" element={<DocsLayout />}>
          <Route path="*" element={<DocPage />} />
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}
