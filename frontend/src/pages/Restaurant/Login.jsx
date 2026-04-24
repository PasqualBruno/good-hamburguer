import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { post, setToken } from '../../api/http';
import './Login.css';

export default function Login() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const data = await post('/api/auth/login', { email, password });
      setToken(data.token);
      navigate('/restaurant/kitchen');
    } catch (err) {
      setError(err.message || 'Email ou senha incorretos.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login" id="page-login">
      <div className="login__card">
        <div className="login__header">
          <span className="login__icon">👨‍🍳</span>
          <h1 className="login__title">Painel do Restaurante</h1>
          <p className="login__subtitle">
            Acesse o gerenciamento de pedidos
          </p>
        </div>

        <form className="login__form" onSubmit={handleSubmit}>
          {error && <div className="login__error">{error}</div>}

          <div className="login__field">
            <label className="login__label" htmlFor="login-email">
              Email
            </label>
            <input
              id="login-email"
              className="login__input"
              type="email"
              placeholder="admin@goodhamburger.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="login__field">
            <label className="login__label" htmlFor="login-password">
              Senha
            </label>
            <input
              id="login-password"
              className="login__input"
              type="password"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <button
            className="login__btn"
            type="submit"
            disabled={loading}
            id="btn-login"
          >
            {loading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <p className="login__back" onClick={() => navigate('/')}>
          ← Voltar para o início
        </p>
      </div>
    </div>
  );
}
