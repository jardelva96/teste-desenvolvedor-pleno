import React, { useState } from 'react';
import axios from 'axios';
import { FaUser, FaLock, FaFacebook, FaGoogle } from 'react-icons/fa'; // Certifique-se de usar ou remover essas importações
import './Login.module.css';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [formType, setFormType] = useState<'login' | 'signup'>('login');
  const [email, setEmail] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setLoading(true);
    try {
      let url = formType === 'login' ? 'http://localhost:5235/api/Auth/Login' : 'http://localhost:5235/api/Auth/Register';
      
      const requestBody = {
        username,
        password,
        email: formType === 'signup' ? email : undefined,
      };

      const response = await axios.post(url, requestBody, {
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (response.data.token) {
        localStorage.setItem('token', response.data.token);
        window.location.href = '/home'; // Redireciona para a página inicial ou área protegida
      } else {
        setError('Token não retornado. Verifique o backend.');
      }
    } catch (err) {
      setError('Erro ao processar. Verifique suas credenciais.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="form-container">
        <h2>{formType === 'login' ? 'Login de Natal' : 'Criar Conta de Natal'}</h2>
        <form onSubmit={handleSubmit}>
          {formType === 'signup' && (
            <div className="input-group">
              <label htmlFor="email">Email</label>
              <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Digite seu e-mail"
                required
              />
            </div>
          )}
          <div className="input-group">
            <label htmlFor="username">Username</label>
            <div className="input-with-icon">
              <FaUser className="input-icon" />
              <input
                id="username"
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Digite seu nome de usuário"
                required
              />
            </div>
          </div>
          <div className="input-group">
            <label htmlFor="password">Senha</label>
            <div className="input-with-icon">
              <FaLock className="input-icon" />
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Digite sua senha"
                required
              />
            </div>
          </div>
          {formType === 'signup' && (
            <div className="input-group">
              <label htmlFor="confirmPassword">Confirmar Senha</label>
              <div className="input-with-icon">
                <FaLock className="input-icon" />
                <input
                  id="confirmPassword"
                  type="password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  placeholder="Confirme sua senha"
                  required
                />
              </div>
            </div>
          )}
          <div className="remember-me">
            <label>
              <input type="checkbox" /> Lembrar-me
            </label>
            <button type="button" className="forgot-password">Esqueceu sua senha?</button>
          </div>
          {error && <p className="error-message">{error}</p>}
          <button type="submit" className="submit-btn" disabled={loading}>
            {loading ? 'Processando...' : formType === 'login' ? 'ENTRAR' : 'CRIAR CONTA'}
          </button>
        </form>
        <div className="social-login">
          <p>Ou entre com</p>
          <div className="social-buttons">
            <button className="social-btn facebook"><FaFacebook /></button>
            <button className="social-btn google"><FaGoogle /></button>
          </div>
        </div>
        <div className="switch-form">
          <p>
            {formType === 'login' ? "Não tem uma conta?" : "Já tem uma conta?"} 
            <button onClick={() => setFormType(formType === 'login' ? 'signup' : 'login')}>
              {formType === 'login' ? 'Registrar' : 'Login'}
            </button>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Login;
