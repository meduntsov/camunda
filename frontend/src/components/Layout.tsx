import { Link, Outlet } from 'react-router-dom';

export function Layout() {
  return (
    <div style={{ fontFamily: 'Arial, sans-serif', maxWidth: 900, margin: '0 auto', padding: 16 }}>
      <h1>Workflow MVP</h1>
      <nav style={{ marginBottom: 16 }}>
        <Link to="/login" style={{ marginRight: 12 }}>Login</Link>
        <Link to="/requests">Requests</Link>
      </nav>
      <Outlet />
    </div>
  );
}
