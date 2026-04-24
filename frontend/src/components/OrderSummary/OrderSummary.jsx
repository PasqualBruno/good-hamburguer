import './OrderSummary.css';

const typeEmojis = {
  Sandwich: '🍔',
  Side: '🍟',
  Drink: '🥤',
};

export default function OrderSummary({
  items,
  promotions,
  onConfirm,
  onClear,
  loading,
}) {
  const subtotal = items.reduce((sum, item) => sum + item.price, 0);

  const selectedTypes = items.map((i) => i.type);
  const bestPromo = promotions
    .filter((p) =>
      p.requiredItemTypes.every((type) => selectedTypes.includes(type))
    )
    .sort((a, b) => b.discountPercent - a.discountPercent)[0];

  const discountPercent = bestPromo ? bestPromo.discountPercent : 0;
  const discountValue = Math.round(subtotal * (discountPercent / 100) * 100) / 100;
  const total = subtotal - discountValue;

  const formatPrice = (val) =>
    `R$ ${val.toFixed(2).replace('.', ',')}`;

  return (
    <div className="order-summary" id="order-summary">
      <h3 className="order-summary__title">🧾 Seu Pedido</h3>

      {items.length === 0 ? (
        <p className="order-summary__empty">
          Selecione itens do cardápio para montar seu pedido
        </p>
      ) : (
        <>
          <div className="order-summary__items">
            {items.map((item) => (
              <div key={item.id} className="order-summary__item">
                <div className="order-summary__item-info">
                  <span className="order-summary__item-emoji">
                    {typeEmojis[item.type] || '🍽️'}
                  </span>
                  <span className="order-summary__item-name">{item.name}</span>
                </div>
                <span className="order-summary__item-price">
                  {formatPrice(item.price)}
                </span>
              </div>
            ))}
          </div>

          <div className="order-summary__divider" />

          <div className="order-summary__row order-summary__row--subtotal">
            <span>Subtotal</span>
            <span>{formatPrice(subtotal)}</span>
          </div>

          {bestPromo && (
            <>
              <div className="order-summary__row order-summary__row--promo-name">
                <span>🎉 {bestPromo.name}</span>
              </div>
              <div className="order-summary__row order-summary__row--promo">
                <span>Desconto ({discountPercent}%)</span>
                <span>-{formatPrice(discountValue)}</span>
              </div>
            </>
          )}

          <div className="order-summary__divider" />

          <div className="order-summary__row order-summary__row--total">
            <span>Total</span>
            <span>{formatPrice(total)}</span>
          </div>

          <button
            className="order-summary__btn order-summary__btn--confirm"
            onClick={onConfirm}
            disabled={loading}
            id="btn-confirm-order"
          >
            {loading ? 'Enviando...' : '🍔 Confirmar Pedido'}
          </button>

          <button
            className="order-summary__btn order-summary__btn--clear"
            onClick={onClear}
            id="btn-clear-order"
          >
            Limpar seleção
          </button>
        </>
      )}
    </div>
  );
}
