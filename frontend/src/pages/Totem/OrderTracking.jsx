import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { get } from '../../api/http';
import { createConnection, startConnection, joinGroup, leaveGroup } from '../../api/signalr';
import './OrderTracking.css';

const steps = [
  { key: 'Received', label: 'Recebido', icon: '📩' },
  { key: 'Preparing', label: 'Preparando', icon: '🔥' },
  { key: 'Ready', label: 'Pronto', icon: '✅' },
  { key: 'Delivered', label: 'Entregue', icon: '📦' },
];

const statusMessages = {
  Received: { msg: 'Pedido recebido!', desc: 'Aguardando a cozinha iniciar o preparo.' },
  Preparing: { msg: 'Em preparo! 🔥', desc: 'Seu lanche está sendo preparado com carinho.' },
  Ready: { msg: 'Pronto! 🎉', desc: 'Seu pedido está pronto para retirada!' },
  Delivered: { msg: 'Entregue! ✅', desc: 'Bom apetite! Obrigado por escolher o Good Hamburger.' },
  Cancelled: { msg: 'Pedido cancelado', desc: 'Infelizmente seu pedido foi cancelado.' },
};

function Confetti() {
  const colors = ['#f59e0b', '#ef4444', '#22c55e', '#3b82f6', '#fbbf24', '#a855f7'];
  const pieces = Array.from({ length: 40 }, (_, i) => ({
    id: i,
    left: `${Math.random() * 100}%`,
    color: colors[Math.floor(Math.random() * colors.length)],
    delay: `${Math.random() * 2}s`,
    size: `${6 + Math.random() * 8}px`,
  }));

  return (
    <div className="confetti-container">
      {pieces.map((p) => (
        <div
          key={p.id}
          className="confetti-piece"
          style={{
            left: p.left,
            background: p.color,
            animationDelay: p.delay,
            width: p.size,
            height: p.size,
          }}
        />
      ))}
    </div>
  );
}

export default function OrderTracking() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showConfetti, setShowConfetti] = useState(false);
  const connectionRef = useRef(null);
  const prevStatusRef = useRef(null);

  useEffect(() => {
    async function fetchOrder() {
      try {
        const data = await get(`/api/orders/${id}`);
        setOrder(data);
        prevStatusRef.current = data.status;
      } catch {
        setOrder(null);
      } finally {
        setLoading(false);
      }
    }
    fetchOrder();
  }, [id]);

  useEffect(() => {
    const connection = createConnection();
    connectionRef.current = connection;

    connection.on('OrderStatusChanged', (data) => {
      if (data.orderId === id) {
        setOrder((prev) => (prev ? { ...prev, status: data.newStatus } : prev));

        if (data.newStatus === 'Ready') {
          setShowConfetti(true);
          setTimeout(() => setShowConfetti(false), 4000);
        }
      }
    });

    startConnection(connection).then(() => {
      joinGroup(connection, `order-${id}`);
    });

    return () => {
      if (connectionRef.current) {
        leaveGroup(connectionRef.current, `order-${id}`);
        connectionRef.current.stop();
      }
    };
  }, [id]);

  if (loading) {
    return (
      <div className="order-tracking">
        <p className="order-tracking__loading">Carregando pedido...</p>
      </div>
    );
  }

  if (!order) {
    return (
      <div className="order-tracking">
        <div className="order-tracking__card">
          <p className="order-tracking__cancelled">Pedido não encontrado</p>
          <button className="order-tracking__btn" onClick={() => navigate('/totem')}>
            Fazer novo pedido
          </button>
        </div>
      </div>
    );
  }

  const isCancelled = order.status === 'Cancelled';
  const currentIndex = steps.findIndex((s) => s.key === order.status);
  const statusInfo = statusMessages[order.status] || statusMessages.Received;

  const formatPrice = (val) => `R$ ${val.toFixed(2).replace('.', ',')}`;

  return (
    <div className="order-tracking" id="page-order-tracking">
      {showConfetti && <Confetti />}

      <div className="order-tracking__card">
        <span className="order-tracking__label">Seu pedido</span>
        <span className="order-tracking__code">{order.code}</span>

        {!isCancelled ? (
          <div className="order-tracking__progress">
            {steps.map((step, idx) => {
              let stepClass = 'order-tracking__step';
              if (idx < currentIndex) stepClass += ' order-tracking__step--done';
              else if (idx === currentIndex) stepClass += ' order-tracking__step--active';

              return (
                <div key={step.key} className={stepClass}>
                  <div className="order-tracking__step-dot">{step.icon}</div>
                  <span className="order-tracking__step-label">{step.label}</span>
                  {idx < steps.length - 1 && <div className="order-tracking__step-line" />}
                </div>
              );
            })}
          </div>
        ) : (
          <p className="order-tracking__cancelled">❌ Pedido Cancelado</p>
        )}

        <p className="order-tracking__status-msg">{statusInfo.msg}</p>
        <p className="order-tracking__status-desc">{statusInfo.desc}</p>

        <div className="order-tracking__items">
          {order.items.map((item, idx) => (
            <span key={idx} className="order-tracking__item-chip">
              {item.name}
            </span>
          ))}
        </div>

        <span className="order-tracking__total">{formatPrice(order.total)}</span>

        <button
          className="order-tracking__btn"
          onClick={() => navigate('/totem')}
          id="btn-new-order"
        >
          🍔 Fazer novo pedido
        </button>
      </div>
    </div>
  );
}
