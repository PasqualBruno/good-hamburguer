import './KanbanColumn.css';

const nextStatusMap = {
  Received: 'Preparing',
  Preparing: 'Ready',
  Ready: 'Delivered',
};

const nextStatusLabel = {
  Received: '🔥 Preparar',
  Preparing: '✅ Pronto',
  Ready: '📦 Entregar',
};

export default function KanbanColumn({
  title,
  icon,
  status,
  orders,
  onUpdateStatus,
}) {
  const filtered = orders.filter((o) => o.status === status);

  const formatTime = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const formatPrice = (val) =>
    `R$ ${val.toFixed(2).replace('.', ',')}`;

  const canCancel = status === 'Received' || status === 'Preparing';
  const canAdvance = !!nextStatusMap[status];

  return (
    <div className="kanban-column" id={`kanban-col-${status.toLowerCase()}`}>
      <div className="kanban-column__header">
        <span className="kanban-column__title">
          {icon} {title}
        </span>
        <span className="kanban-column__count">{filtered.length}</span>
      </div>

      <div className="kanban-column__body">
        {filtered.map((order) => (
          <div
            key={order.id}
            className={`kanban-card ${
              order.status === 'Cancelled' ? 'kanban-card--cancelled' : ''
            }`}
            id={`kanban-card-${order.id}`}
          >
            <div className="kanban-card__header">
              <span className="kanban-card__code">{order.code}</span>
              <span className="kanban-card__time">
                {formatTime(order.createdAt)}
              </span>
            </div>

            <div className="kanban-card__items">
              {order.items.map((item, idx) => (
                <span key={idx} className="kanban-card__item-chip">
                  {item.name}
                </span>
              ))}
            </div>

            <div className="kanban-card__total">
              <span>{formatPrice(order.total)}</span>
              {order.promotionName && (
                <span className="kanban-card__promo">
                  🎉 {order.promotionName}
                </span>
              )}
            </div>

            {(canAdvance || canCancel) && (
              <div className="kanban-card__actions">
                {canAdvance && (
                  <button
                    className="kanban-card__btn kanban-card__btn--advance"
                    onClick={() =>
                      onUpdateStatus(order.id, nextStatusMap[status])
                    }
                  >
                    {nextStatusLabel[status]}
                  </button>
                )}
                {canCancel && (
                  <button
                    className="kanban-card__btn kanban-card__btn--cancel"
                    onClick={() => onUpdateStatus(order.id, 'Cancelled')}
                  >
                    ✕ Cancelar
                  </button>
                )}
              </div>
            )}
          </div>
        ))}

        {filtered.length === 0 && (
          <p
            style={{
              color: 'var(--text-muted)',
              fontSize: '0.85rem',
              textAlign: 'center',
              padding: '24px 0',
            }}
          >
            Nenhum pedido
          </p>
        )}
      </div>
    </div>
  );
}
