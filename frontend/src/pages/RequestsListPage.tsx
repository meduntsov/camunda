import { FormEvent, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { createRequest, getRequests } from '../api/requests';
import { RequestListItem } from '../types/request';

export function RequestsListPage() {
  const [items, setItems] = useState<RequestListItem[]>([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  async function load() {
    const data = await getRequests();
    setItems(data);
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    await createRequest(title, description);
    setTitle('');
    setDescription('');
    await load();
  }

  useEffect(() => {
    load();
  }, []);

  return (
    <section>
      <h2>Requests</h2>
      <form onSubmit={onSubmit} style={{ display: 'grid', gap: 8, marginBottom: 20 }}>
        <input value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Title" required />
        <textarea value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Description" required />
        <button type="submit">Create request</button>
      </form>

      <div style={{ display: 'grid', gap: 8 }}>
        {items.map((item) => (
          <article key={item.id} style={{ border: '1px solid #ddd', padding: 12, borderRadius: 8 }}>
            <h3>{item.title}</h3>
            <p>{item.description}</p>
            <p><strong>Status:</strong> {item.status}</p>
            <Link to={`/requests/${item.id}`}>Open details</Link>
          </article>
        ))}
      </div>
    </section>
  );
}
