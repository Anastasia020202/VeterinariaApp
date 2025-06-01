import axios from 'axios';
import router from '@/router';
import { useAuthStore } from '@/stores/auth'; // Import agregado para poder gestionar autenticación

// Crear una instancia personalizada de Axios con la URL base de las variables de entorno
const apiBase = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api';

console.log('API Base configurada en:', apiBase);

// Obtener token inicial si existe
const initialToken = localStorage.getItem('token');

const api = axios.create({
  baseURL: apiBase,
  headers: {
    'Content-Type': 'application/json',
    // Establecer token desde el inicio si existe
    ...(initialToken ? { Authorization: `Bearer ${initialToken}` } : {})
  }
});

// Función auxiliar para verificar si el token está válido
const isTokenValid = () => {
  const token = localStorage.getItem('token');
  if (!token) return false;
  
  const expiration = localStorage.getItem('tokenExpiration');
  if (expiration && new Date() > new Date(expiration)) {
    // Token expirado, limpiar
    localStorage.removeItem('token');
    localStorage.removeItem('tokenExpiration');
    return false;
  }
  
  return true;
};

// Configurar interceptores para añadir el token de autenticación
api.interceptors.request.use(
  config => {
    // Obtener token fresco para cada petición (asegura que se usa el más reciente)
    const token = localStorage.getItem('token');
    
    // Verificar validez del token
    if (token && isTokenValid()) {
      console.log(`Enviando petición autorizada a: ${config.url}`);
      // Siempre sobreescribir el header para asegurar token actualizado
      config.headers.Authorization = `Bearer ${token}`;
    } else if (token) {
      // Token existe pero no es válido
      console.warn('Token expirado detectado, cerrando sesión...');
      // Redirigir solo si estamos en una página protegida
      if (router.currentRoute.value.meta.requiresAuth) {
        router.push('/login');
      }
    } else {
      console.log(`Enviando petición sin autorización a: ${config.url}`);
      // Asegurar que no queden headers de autorización antiguos
      delete config.headers.Authorization;
    }
    
    return config;
  },
  error => {
    console.error('Error en interceptor de solicitud:', error);
    return Promise.reject(error);
  }
);

// Interceptor para manejar respuestas y errores
api.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    console.error('Error en respuesta API:', error.response?.data || error.message);
    
    // Si el error es 401 (no autorizado), cerrar sesión automáticamente
    if (error.response?.status === 401) {
      console.log('Sesión caducada o no autorizada');
      
      // Limpiar la sesión
      localStorage.removeItem('token');
      localStorage.removeItem('tokenExpiration');
      
      // Si estamos en una ruta protegida, redirigir al login
      if (router.currentRoute.value.meta.requiresAuth) {
        router.push('/login');
      }
    }
    
    return Promise.reject(error);
  }
);

export default api;
