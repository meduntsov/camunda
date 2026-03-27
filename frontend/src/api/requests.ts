import { apiClient } from './client';
import { RequestDetail, RequestListItem } from '../types/request';

export async function getRequests(): Promise<RequestListItem[]> {
  const { data } = await apiClient.get('/api/requests');
  return data;
}

export async function getRequestById(id: string): Promise<RequestDetail> {
  const { data } = await apiClient.get(`/api/requests/${id}`);
  return data;
}

export async function createRequest(title: string, description: string): Promise<RequestDetail> {
  const { data } = await apiClient.post('/api/requests', { title, description });
  return data;
}

export async function approveRequest(id: string, comment: string): Promise<RequestDetail> {
  const { data } = await apiClient.post(`/api/requests/${id}/approve`, { comment });
  return data;
}

export async function rejectRequest(id: string, comment: string): Promise<RequestDetail> {
  const { data } = await apiClient.post(`/api/requests/${id}/reject`, { comment });
  return data;
}

export async function getSummary(id: string): Promise<{requestId: string; status: string; summary: string}> {
  const { data } = await apiClient.get(`/api/requests/${id}/summary`);
  return data;
}
