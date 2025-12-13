import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/LoginView.vue'),
    meta: { public: true }
  },
  {
    path: '/',
    redirect: '/empleados'
  },
  {
    path: '/empleados',
    name: 'Empleados',
    component: () => import('../views/GestionEmpleados.vue')
  },
  {
    path: '/departamentos',
    name: 'Departamentos',
    component: () => import('../views/GestionDepartamentos.vue')
  },
  {
    path: '/puestos',
    name: 'Puestos',
    component: () => import('../views/GestionPuestos.vue')
  },
  {
    path: '/ingresos',
    name: 'Tipos de Ingresos',
    component: () => import('../views/GestionIngresos.vue')
  },
  {
    path: '/deducciones',
    name: 'Tipos de Deducciones',
    component: () => import('../views/GestionDeducciones.vue')
  },
  {
    path: '/transacciones',
    name: 'Transacciones',
    component: () => import('../views/GestionTransacciones.vue')
  },
  {
    path: '/nomina',
    name: 'Nómina',
    component: () => import('../views/CalcularNomina.vue')
  },
  {
    path: '/reporte-nomina/:id',
    name: 'ReporteNomina',
    component: () => import('../views/ReporteNomina.vue')
  },
  {
    path: '/consulta',
    name: 'Consulta',
    component: () => import('../views/ConsultaEmpleados.vue')
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// Guard de Navegación Global
router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem('session_token');

  if (!to.meta.public && !isAuthenticated) {
    // Si la ruta no es pública y no hay token, enviar a Login
    next({ name: 'Login' });
  } else if (to.name === 'Login' && isAuthenticated) {
    // Si intenta ir a Login pero ya está autenticado, enviar a Home
    next({ path: '/' });
  } else {
    next();
  }
});

export default router;
