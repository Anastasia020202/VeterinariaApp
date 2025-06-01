import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true },
    },
    {
      path: '/especies',
      name: 'especies',
      component: () => import('../views/EspeciesView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/consultas',
      name: 'consultas',
      component: () => import('../views/ConsultasView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/tratamientos',
      name: 'tratamientos',
      component: () => import('../views/TratamientosView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/planes',
      name: 'planes',
      component: () => import('../views/PlanesView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { guest: true },
    },
    {
      path: '/registro',
      name: 'registro',
      component: RegisterView,
      meta: { guest: true },
    },
  ],
})

// Guardia de rutas para autenticación
router.beforeEach((to, from) => {
  const authStore = useAuthStore()
  
  // Verificar autenticación para rutas protegidas
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login' }
  }
  
  // Redirigir usuarios autenticados lejos de páginas de invitado
  if (to.meta.guest && authStore.isAuthenticated) {
    return { name: 'dashboard' }
  }
})

export default router
