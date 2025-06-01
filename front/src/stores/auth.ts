import { defineStore } from 'pinia';
import router from '@/router';
import api from '@/api';

interface UserCredentials {
  email: string;
  password: string;
}

interface RegisterData extends UserCredentials {
  confirmPassword: string;
}

interface AuthState {
  token: string;
  isAuthenticated: boolean;
  user: any | null;
  tokenExpiration: Date | null;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: localStorage.getItem('token') || '',
    isAuthenticated: !!localStorage.getItem('token'),
    user: null,
    tokenExpiration: localStorage.getItem('tokenExpiration') 
      ? new Date(localStorage.getItem('tokenExpiration') as string) 
      : null,
  }),

  getters: {
    authHeader: (state) => state.token ? { headers: { Authorization: `Bearer ${state.token}` } } : {},
    
    hasValidToken: (state) => {
      if (!state.token) {
        console.log('Auth: No hay token');
        return false;
      }
      
      if (state.tokenExpiration && new Date() > state.tokenExpiration) {
        console.log('Auth: Token expirado');
        return false;
      }
      
      console.log('Auth: Token válido');
      return true;
    }
  },

  actions: {
    async login(email: string, password: string) {
      console.log('Auth: Intentando iniciar sesión', { email });
      
      try {
        const response = await api.post('/auth/login', { email, password });
        const { token, expiracion } = response.data;
        
        // Configurar token y esperar a que se complete
        await this.setToken(token, expiracion);
        console.log('Auth: Inicio de sesión exitoso');
        
        // Añadir un pequeño retraso para asegurar que todas las configuraciones se apliquen
        await new Promise(resolve => setTimeout(resolve, 300));
        
        return true;
      } catch (error: any) {
        console.error('Auth: Error en inicio de sesión', error?.response?.data || error?.message || error);
        this.clearAuth();
        throw error;
      }
    },

    async register(data: RegisterData) {
      console.log('Auth: Intentando registrar nuevo usuario', { email: data.email });
      
      try {
        const response = await api.post('/auth/registro', {
          email: data.email,
          password: data.password,
          confirmPassword: data.confirmPassword
        });
        
        console.log('Auth: Registro exitoso');
        return response.data;
      } catch (error: any) {
        console.error('Auth: Error en registro', error?.response?.data || error?.message || error);
        throw error;
      }
    },

    async setToken(token: string, expiration?: string) {
      console.log('Auth: Configurando token', { expiration });
      
      this.token = token;
      localStorage.setItem('token', token);
      
      // Establecer expiración del token si se proporciona
      if (expiration) {
        this.tokenExpiration = new Date(expiration);
        localStorage.setItem('tokenExpiration', expiration);
      }
      
      // Esperar a que el localStorage se actualice
      await new Promise(resolve => setTimeout(resolve, 100));
      
      this.isAuthenticated = true;
      console.log('Auth: Token configurado en cliente HTTP');
    },

    logout() {
      console.log('Auth: Cerrando sesión');
      this.clearAuth();
      router.push('/login');
    },

    clearAuth() {
      console.log('Auth: Limpiando autenticación');
      
      this.token = '';
      this.user = null;
      this.tokenExpiration = null;
      
      localStorage.removeItem('token');
      localStorage.removeItem('tokenExpiration');
      
      this.isAuthenticated = false;
    },

    initialize() {
      console.log('Auth: Inicializando estado de autenticación');
      
      const token = localStorage.getItem('token');
      const expiration = localStorage.getItem('tokenExpiration');
      
      if (token) {
        console.log('Auth: Token encontrado en almacenamiento');
        
        if (expiration) {
          this.tokenExpiration = new Date(expiration);
          
          // Si el token está expirado, limpiar autenticación
          if (new Date() > this.tokenExpiration) {
            console.log('Auth: Token almacenado expirado, limpiando');
            this.clearAuth();
            return;
          }
        }
        
        // Token válido, configurar autenticación
        this.setToken(token, expiration || undefined);
        console.log('Auth: Autenticación restaurada desde almacenamiento');
      } else {
        console.log('Auth: No hay token en almacenamiento, usuario no autenticado');
      }
    }
  }
});
