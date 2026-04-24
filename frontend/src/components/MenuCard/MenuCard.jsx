import './MenuCard.css';

const typeEmojis = {
  Sandwich: '🍔',
  Side: '🍟',
  Drink: '🥤',
};

const typeLabels = {
  Sandwich: 'Sanduíche',
  Side: 'Acompanhamento',
  Drink: 'Bebida',
};

export default function MenuCard({ item, selected, disabled, onToggle }) {
  const emoji = typeEmojis[item.type] || '🍽️';
  const typeLabel = typeLabels[item.type] || item.type;

  const imagePath = `/products/${item.name.toLowerCase().replace(/ /g, '-')}.png`;

  const handleClick = () => {
    if (!disabled || selected) {
      onToggle(item);
    }
  };

  let className = 'menu-card';
  if (selected) className += ' menu-card--selected';
  if (disabled && !selected) className += ' menu-card--disabled';

  return (
    <div className={className} onClick={handleClick} id={`menu-card-${item.id}`}>
      {selected && <span className="menu-card__check">✓</span>}
      <div className="menu-card__image-wrapper">
        <img
          src={imagePath}
          alt={item.name}
          onError={(e) => {
            e.target.style.display = 'none';
            e.target.parentNode.innerHTML = `<span class="menu-card__emoji">${emoji}</span>`;
          }}
        />
      </div>
      <span className="menu-card__type">{typeLabel}</span>
      <span className="menu-card__name">{item.name}</span>
      <span className="menu-card__price">
        R$ {item.price.toFixed(2).replace('.', ',')}
      </span>
    </div>
  );
}
