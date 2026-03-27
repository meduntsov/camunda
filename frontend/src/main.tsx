import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { LoginPlaceholderPage } from './pages/LoginPlaceholderPage';
import { RequestDetailsPage } from './pages/RequestDetailsPage';
import { RequestsListPage } from './pages/RequestsListPage';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Navigate to="/requests" replace />} />
          <Route path="login" element={<LoginPlaceholderPage />} />
          <Route path="requests" element={<RequestsListPage />} />
          <Route path="requests/:id" element={<RequestDetailsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>,
);
