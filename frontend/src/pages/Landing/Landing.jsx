import { useNavigate } from 'react-router-dom';
import './Landing.css';

export default function Landing() {
  const navigate = useNavigate();

  return (
    <div className="landing" id="page-landing">
      <div className="landing__brand">
        <div className="landing__logo">🍔</div>
        <h1 className="landing__title">Good Hamburger</h1>
        <p className="landing__subtitle">
          O melhor hambúrguer da cidade, feito com carinho
        </p>
      </div>

      <div className="landing__cards">
        <div
          className="landing__card"
          onClick={() => navigate('/totem')}
          id="card-totem"
        >
          <span className="landing__card-icon">📱</span>
          <h2 className="landing__card-title">Fazer Pedido</h2>
          <p className="landing__card-desc">
            Monte seu lanche, escolha acompanhamentos e bebidas
          </p>
        </div>

        <div
          className="landing__card"
          onClick={() => navigate('/restaurant/login')}
          id="card-restaurant"
        >
          <span className="landing__card-icon">👨‍🍳</span>
          <h2 className="landing__card-title">Sou Restaurante</h2>
          <p className="landing__card-desc">
            Acesse o painel de gerenciamento de pedidos
          </p>
        </div>
      </div>

      <p className="landing__footer">Good Hamburger © 2026</p>
    </div>
  );
}
