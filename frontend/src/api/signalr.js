import * as signalR from '@microsoft/signalr';

const HUB_URL = `${import.meta.env.VITE_API_URL || 'http://localhost:5020'}/hubs/orders`;

export function createConnection() {
  console.log('[SignalR] Creating connection to:', HUB_URL);
  return new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL)
    .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
    .configureLogging(signalR.LogLevel.Information)
    .build();
}

export async function startConnection(connection) {
  try {
    await connection.start();
    console.log('[SignalR] Connected');
  } catch (err) {
    console.error('[SignalR] Connection failed:', err);
    setTimeout(() => startConnection(connection), 3000);
  }
}

export async function joinGroup(connection, groupName) {
  try {
    await connection.invoke('JoinGroup', groupName);
    console.log(`[SignalR] Joined group: ${groupName}`);
  } catch (err) {
    console.error(`[SignalR] Failed to join group ${groupName}:`, err);
  }
}

export async function leaveGroup(connection, groupName) {
  try {
    await connection.invoke('LeaveGroup', groupName);
    console.log(`[SignalR] Left group: ${groupName}`);
  } catch (err) {
    console.error(`[SignalR] Failed to leave group ${groupName}:`, err);
  }
}
