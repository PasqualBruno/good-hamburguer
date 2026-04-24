const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5020';

function getAuthHeaders() {
  const token = localStorage.getItem('gh_token');
  const headers = { 'Content-Type': 'application/json' };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  return headers;
}

async function handleResponse(response) {
  const data = await response.json();
  if (!response.ok) {
    const error = new Error(data.message || 'Erro na requisição');
    error.code = data.code || 'UNKNOWN';
    error.status = response.status;
    throw error;
  }
  return data;
}

export async function get(path) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: 'GET',
    headers: getAuthHeaders(),
  });
  return handleResponse(response);
}

export async function post(path, body) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(body),
  });
  return handleResponse(response);
}

export async function patch(path, body) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: 'PATCH',
    headers: getAuthHeaders(),
    body: JSON.stringify(body),
  });
  return handleResponse(response);
}

export function setToken(token) {
  localStorage.setItem('gh_token', token);
}

export function getToken() {
  return localStorage.getItem('gh_token');
}

export function clearToken() {
  localStorage.removeItem('gh_token');
}
