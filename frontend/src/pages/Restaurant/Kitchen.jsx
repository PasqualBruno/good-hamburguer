import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { get, patch, getToken, clearToken } from '../../api/http';
import { createConnection, startConnection, joinGroup } from '../../api/signalr';
import KanbanColumn from '../../components/KanbanColumn/KanbanColumn';
import './Kitchen.css';

const columns = [
  { status: 'Received', title: 'Recebidos', icon: '📩' },
  { status: 'Preparing', title: 'Preparando', icon: '🔥' },
  { status: 'Ready', title: 'Prontos', icon: '✅' },
  { status: 'Delivered', title: 'Entregues', icon: '📦' },
];

export default function Kitchen() {
  const navigate = useNavigate();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [connected, setConnected] = useState(false);
  const connectionRef = useRef(null);

  useEffect(() => {
    if (!getToken()) {
      navigate('/restaurant/login');
      return;
    }

    async function fetchOrders() {
      try {
        const data = await get('/api/admin/orders');
        setOrders(data);
      } catch (err) {
        if (err.status === 401) {
          clearToken();
          navigate('/restaurant/login');
          return;
        }
        setError('Erro ao carregar pedidos.');
      } finally {
        setLoading(false);
      }
    }

    fetchOrders();
  }, [navigate]);

  useEffect(() => {
    const connection = createConnection();
    connectionRef.current = connection;

    connection.on('NewOrderReceived', (order) => {
      setOrders((prev) => {
        const exists = prev.some((o) => o.id === order.id);
        if (exists) return prev;
        return [order, ...prev];
      });
    });

    connection.on('OrderStatusChanged', (data) => {
      setOrders((prev) =>
        prev.map((o) =>
          o.id === data.orderId ? { ...o, status: data.newStatus } : o
        )
      );
    });

    connection.onreconnecting(() => setConnected(false));
    connection.onreconnected(() => setConnected(true));
    connection.onclose(() => setConnected(false));

    startConnection(connection).then(() => {
      setConnected(true);
      joinGroup(connection, 'admin');
    });

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, []);

  const handleUpdateStatus = async (orderId, newStatus) => {
    try {
      await patch(`/api/admin/orders/${orderId}/status`, {
        status: newStatus,
      });
      setOrders((prev) =>
        prev.map((o) => (o.id === orderId ? { ...o, status: newStatus } : o))
      );
    } catch (err) {
      setError(err.message || 'Erro ao atualizar status.');
      setTimeout(() => setError(null), 4000);
    }
  };

  const handleLogout = () => {
    clearToken();
    navigate('/');
  };

  return (
    <div className="kitchen" id="page-kitchen">
      <header className="kitchen__header">
        <div className="kitchen__header-left">
          <div className="kitchen__header-brand">
            <span className="kitchen__header-emoji">🍔</span>
            <span className="kitchen__header-title">Good Hamburger</span>
          </div>
          <span className="kitchen__header-badge">Cozinha</span>
        </div>

        <div className="kitchen__header-right">
          <span
            className={`kitchen__connection-dot ${
              !connected ? 'kitchen__connection-dot--disconnected' : ''
            }`}
            title={connected ? 'Conectado' : 'Desconectado'}
          />
          <button
            className="kitchen__btn-logout"
            onClick={handleLogout}
            id="btn-logout"
          >
            Sair
          </button>
        </div>
      </header>

      {error && (
        <div className="kitchen__error">
          <div className="toast toast--error">{error}</div>
        </div>
      )}

      {loading ? (
        <div className="kitchen__loading">
          <span>Carregando pedidos...</span>
        </div>
      ) : (
        <div className="kitchen__board">
          {columns.map((col) => (
            <KanbanColumn
              key={col.status}
              title={col.title}
              icon={col.icon}
              status={col.status}
              orders={orders}
              onUpdateStatus={handleUpdateStatus}
            />
          ))}
        </div>
      )}
    </div>
  );
}
