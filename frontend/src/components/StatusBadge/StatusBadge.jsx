import './StatusBadge.css';

const statusMap = {
  Received: 'received',
  Preparing: 'preparing',
  Ready: 'ready',
  Delivered: 'delivered',
  Cancelled: 'cancelled',
};

const labelMap = {
  Received: 'Recebido',
  Preparing: 'Preparando',
  Ready: 'Pronto',
  Delivered: 'Entregue',
  Cancelled: 'Cancelado',
};

export default function StatusBadge({ status }) {
  const variant = statusMap[status] || 'received';
  const label = labelMap[status] || status;

  return (
    <span className={`status-badge status-badge--${variant}`}>
      <span className="status-badge__dot" />
      {label}
    </span>
  );
}
