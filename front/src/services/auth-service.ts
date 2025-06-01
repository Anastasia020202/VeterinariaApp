import api from '@/api';
import axios from 'axios';
import router from '@/router';

// URL base de la API (se utiliza la variable de entorno)
const API_URL = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api';

class AuthService {
  private token: string | null = localStorage.getItem('token');
  private tokenExpirationKey = 'tokenExpiration';

  constructor() {
    this.initializeAxiosInterceptors();
  }

  /**
   * Iniciar sesión de usuario
   * @param email Email del usuario
   * @param password Contraseña del usuario
   * @returns Datos del login (token, etc)
   */
  async login(email: string, password: string) {
    try {
      // Primera fase: obtener el token sin usarlo para la autenticación
      const response = await axios.post(`${API_URL}/auth/login`, {
        email,
        password
      });
      
      if (response.data && response.data.token) {
        // Guardar el token en memoria y localStorage
        const token = response.data.token;
        console.log('Token recibido del servidor:', token.substring(0, 20) + '...');
        
        // Configurar el nuevo token - primero en memoria
        this.token = token;
        
        // Luego en localStorage, SIN eliminar el anterior primero
        window.localStorage.setItem('token', token);
        console.log('Token guardado en localStorage:', window.localStorage.getItem('token')?.substring(0, 20) + '...');
        
        if (response.data.expiracion) {
          window.localStorage.setItem(this.tokenExpirationKey, response.data.expiracion);
        }
        
        // Configurar token para TODAS las futuras peticiones axios
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        
        console.log('Token configurado para todas las peticiones HTTP');
        console.log('Headers configurados:', { 
          'Authorization': `Bearer ${token.substring(0, 20)}...` 
        });
        
        // Esperar un momento para asegurar que el token se configuró
        await new Promise(resolve => setTimeout(resolve, 500));
        
        // Verificar nuevamente que el token está en localStorage
        const storedToken = window.localStorage.getItem('token');
        console.log('Verificación de token en localStorage:', storedToken ? storedToken.substring(0, 20) + '...' : 'NO ENCONTRADO');
        
        if (!storedToken) {
          // Intentar guardar nuevamente si no existe
          console.warn('Token no encontrado en localStorage, intentando nuevamente...');
          window.localStorage.setItem('token', token);
        }
        
        // Segunda fase: hacer una petición de prueba para validar la autenticación
        try {
          console.log('Realizando petición de prueba para validar token...');
          
          // Usando el token directamente, sin depender de localStorage
          const testResponse = await axios.get(`${API_URL}/especies`, {
            headers: {
              'Authorization': `Bearer ${token}`
            }
          });
          console.log('¡Petición de prueba exitosa!', testResponse.status);
        } catch (testError) {
          console.warn('Error en petición de prueba:', testError);
          // No fallamos aquí, solo es una comprobación
        }
      } else {
        console.error('No se recibió un token válido desde el servidor');
      }
      
      return response.data;
    } catch (error) {
      console.error('Error en login:', error);
      throw error;
    }
  }

  /**
   * Registro de nuevo usuario
   * @param email Email del usuario
   * @param password Contraseña del usuario
   * @param confirmPassword Confirmación de contraseña
   * @returns Datos del registro
   */
  async register(email: string, password: string, confirmPassword: string) {
    return axios.post(`${API_URL}/auth/registro`, {
      email,
      password,
      confirmPassword
    });
  }

  /**
   * Cerrar sesión del usuario
   */
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem(this.tokenExpirationKey);
    this.token = null;
    
    // Eliminar headers de autenticación
    delete axios.defaults.headers.common['Authorization'];
    delete api.defaults.headers.common['Authorization'];
    
    // Redirigir al login
    router.push('/login');
  }

  /**
   * Comprobar si el usuario está autenticado
   * @returns true si está autenticado, false en caso contrario
   */
  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;

    const expiration = localStorage.getItem(this.tokenExpirationKey);
    if (expiration && new Date() > new Date(expiration)) {
      this.logout();
      return false;
    }

    return true;
  }

  /**
   * Inicializar interceptores para axios
   * Esto asegura que todas las peticiones tengan el token
   * y que se maneje correctamente el caso de token expirado
   */
  private initializeAxiosInterceptors() {
    // Interceptor para configurar el token en todas las peticiones
    axios.interceptors.request.use(
      config => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers['Authorization'] = `Bearer ${token}`;
          console.log(`Petición con token: ${config.method?.toUpperCase()} ${config.url}`);
        }
        return config;
      },
      error => {
        return Promise.reject(error);
      }
    );

    // Interceptor para manejar 401 (no autorizado)
    axios.interceptors.response.use(
      response => response,
      error => {
        if (error.response?.status === 401) {
          console.log('Error 401: Sesión expirada o no autorizado');
          this.logout();
        }
        return Promise.reject(error);
      }
    );
  }
}

export default new AuthService();
