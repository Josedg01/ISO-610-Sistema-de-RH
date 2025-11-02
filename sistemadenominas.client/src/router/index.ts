import { createRouter, createWebHistory } from 'vue-router'
import GestionEmpleados from '../views/GestionEmpleados.vue'

const routes = [
  {
    path: '/',
    redirect: '/empleados'
  },
  {
    path: '/empleados',
    name: 'Empleados',
    component: GestionEmpleados
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
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
