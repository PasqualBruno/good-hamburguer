import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { get, post } from '../../api/http';
import MenuCard from '../../components/MenuCard/MenuCard';
import OrderSummary from '../../components/OrderSummary/OrderSummary';
import './Totem.css';

const typeOrder = ['Sandwich', 'Side', 'Drink'];
const groupLabels = {
  Sandwich: '🍔 Sanduíches',
  Side: '🍟 Acompanhamentos',
  Drink: '🥤 Bebidas',
};

export default function Totem() {
  const navigate = useNavigate();
  const [menuItems, setMenuItems] = useState([]);
  const [promotions, setPromotions] = useState([]);
  const [selectedItems, setSelectedItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchData() {
      try {
        const [items, promos] = await Promise.all([
          get('/api/menu'),
          get('/api/promotions'),
        ]);
        setMenuItems(items);
        setPromotions(promos);
      } catch (err) {
        setError('Erro ao carregar cardápio. Tente novamente.');
      } finally {
        setLoading(false);
      }
    }
    fetchData();
  }, []);

  const selectedTypes = selectedItems.map((i) => i.type);

  const handleToggle = (item) => {
    const isSelected = selectedItems.some((i) => i.id === item.id);
    if (isSelected) {
      setSelectedItems((prev) => prev.filter((i) => i.id !== item.id));
    } else {
      setSelectedItems((prev) => prev.filter((i) => i.type !== item.type).concat(item));
    }
  };

  const handleConfirm = async () => {
    setSubmitting(true);
    try {
      const order = await post('/api/orders', {
        menuItemIds: selectedItems.map((i) => i.id),
      });
      navigate(`/totem/order/${order.id}`);
    } catch (err) {
      setError(err.message || 'Erro ao criar pedido.');
      setTimeout(() => setError(null), 4000);
    } finally {
      setSubmitting(false);
    }
  };

  const handleClear = () => setSelectedItems([]);

  if (loading) {
    return (
      <div className="totem__loading">
        <span>Carregando cardápio...</span>
      </div>
    );
  }

  const grouped = typeOrder
    .map((type) => ({
      type,
      label: groupLabels[type],
      items: menuItems.filter((i) => i.type === type),
    }))
    .filter((g) => g.items.length > 0);

  return (
    <div className="totem" id="page-totem">
      <header className="totem__header">
        <div
          className="totem__header-brand"
          onClick={() => navigate('/')}
        >
          <span className="totem__header-emoji">🍔</span>
          <span className="totem__header-title">Good Hamburger</span>
        </div>
        <button
          className="totem__header-back"
          onClick={() => navigate('/')}
        >
          ← Voltar
        </button>
      </header>

      {error && (
        <div className="toast-container">
          <div className="toast toast--error">{error}</div>
        </div>
      )}

      <div className="totem__content">
        <div className="totem__menu">
          <h2 className="totem__section-title">📋 Cardápio</h2>
          <p className="totem__section-subtitle">
            Selecione até 1 item de cada categoria
          </p>

          {grouped.map((group) => (
            <div key={group.type} className="totem__group">
              <h3 className="totem__group-title">{group.label}</h3>
              <div className="totem__grid">
                {group.items.map((item) => {
                  const isSelected = selectedItems.some(
                    (i) => i.id === item.id
                  );
                  const isTypeTaken =
                    !isSelected && selectedTypes.includes(item.type);

                  return (
                    <MenuCard
                      key={item.id}
                      item={item}
                      selected={isSelected}
                      disabled={isTypeTaken}
                      onToggle={handleToggle}
                    />
                  );
                })}
              </div>
            </div>
          ))}
        </div>

        <div className="totem__sidebar">
          <OrderSummary
            items={selectedItems}
            promotions={promotions}
            onConfirm={handleConfirm}
            onClear={handleClear}
            loading={submitting}
          />
        </div>
      </div>
    </div>
  );
}
