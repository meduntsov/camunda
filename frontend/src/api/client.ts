import axios from 'axios';

const baseURL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8088';

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('demo-token') ?? 'demo-token';
  config.headers.Authorization = `Bearer ${token}`;
  return config;
});
