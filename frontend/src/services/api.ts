import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5235/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptador para logar erros
api.interceptors.response.use(
    (response) => response,
    (error) => {
        console.error('Erro na requisição:', error.response || error.message);
        return Promise.reject(error);
    }
);

export default api;
