import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

import './assets/styles/main.scss'

// Crear la aplicación Vue
const app = createApp(App)

// Usar Pinia para gestión de estado
app.use(createPinia())

// Configurar rutas
app.use(router)

// Montar la aplicación en el DOM
app.mount('#app')

console.log('Aplicación veterinaria inicializada')
