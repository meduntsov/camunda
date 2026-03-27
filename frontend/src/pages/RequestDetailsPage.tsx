import { FormEvent, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { approveRequest, getRequestById, getSummary, rejectRequest } from '../api/requests';
import { RequestDetail } from '../types/request';

export function RequestDetailsPage() {
  const { id } = useParams();
  const [detail, setDetail] = useState<RequestDetail | null>(null);
  const [comment, setComment] = useState('');
  const [summary, setSummary] = useState('');

  async function load() {
    if (!id) return;
    const data = await getRequestById(id);
    setDetail(data);
  }

  async function onApprove(event: FormEvent) {
    event.preventDefault();
    if (!id) return;
    const updated = await approveRequest(id, comment);
    setDetail(updated);
    setComment('');
  }

  async function onReject(event: FormEvent) {
    event.preventDefault();
    if (!id) return;
    const updated = await rejectRequest(id, comment);
    setDetail(updated);
    setComment('');
  }

  async function onSummary() {
    if (!id) return;
    const data = await getSummary(id);
    setSummary(data.summary);
  }

  useEffect(() => {
    load();
  }, [id]);

  if (!detail) {
    return <p>Loading...</p>;
  }

  return (
    <section>
      <h2>{detail.title}</h2>
      <p>{detail.description}</p>
      <p><strong>Status:</strong> {detail.status}</p>
      <p><strong>Camunda Instance:</strong> {detail.camundaProcessInstanceKey ?? 'Not available'}</p>

      <form style={{ display: 'flex', gap: 8, marginBottom: 12 }}>
        <input value={comment} onChange={(e) => setComment(e.target.value)} placeholder="Comment" />
        <button onClick={onApprove}>Approve</button>
        <button onClick={onReject}>Reject</button>
        <button type="button" onClick={onSummary}>Generate summary</button>
      </form>

      {summary && (
        <div style={{ background: '#f6f6f6', padding: 10, borderRadius: 6, marginBottom: 12 }}>
          <strong>OpenClaw summary:</strong>
          <p>{summary}</p>
        </div>
      )}

      <h3>Comments</h3>
      <ul>
        {detail.comments.map((item) => (
          <li key={`${item.author}-${item.createdAt}`}>{item.author}: {item.comment}</li>
        ))}
      </ul>

      <h3>Audit log</h3>
      <ul>
        {detail.auditLog.map((item) => (
          <li key={`${item.action}-${item.createdAt}`}>{item.createdAt}: {item.details}</li>
        ))}
      </ul>
    </section>
  );
}
