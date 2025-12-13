<template>
  <div id="layout">
    <!-- Renderizar Header solo si NO estamos en la ruta de Login -->
    <header v-if="route.name !== 'Login'">
      <div class="wrapper">
        <div class="brand">
          <h1>Sistema de Nóminas</h1>
          <span class="user-welcome" v-if="currentUser">Hola, {{ currentUser }}</span>
        </div>

        <nav>
          <router-link to="/empleados">Empleados</router-link>
          <router-link to="/departamentos">Deptos</router-link>
          <router-link to="/puestos">Puestos</router-link>
          <router-link to="/ingresos">Ingresos</router-link>
          <router-link to="/deducciones">Deducciones</router-link>
          <router-link to="/transacciones">Transacciones</router-link>
          <router-link to="/consulta" class="link-consulta">Consultas</router-link>
          <router-link to="/nomina" class="link-nomina-calculo">Nómina</router-link>

          <button @click="logout" class="btn-logout">Salir</button>
        </nav>
      </div>
    </header>

    <main>
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const currentUser = ref<string | null>(null);

function checkUser() {
  currentUser.value = localStorage.getItem('current_user');
}

function logout() {
  if (confirm('¿Cerrar sesión?')) {
    localStorage.removeItem('session_token');
    localStorage.removeItem('current_user');
    currentUser.value = null;
    router.push('/login');
  }
}

// Verificar usuario al montar
onMounted(() => {
  checkUser();
});

// Verificar usuario cada vez que cambiamos de ruta (para actualizar header)
watch(
  () => route.path,
  () => {
    checkUser();
  }
);
</script>

<style scoped>
  #layout {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
  }

  header {
    line-height: 1.5;
    background-color: var(--color-background-mute);
    border-bottom: 1px solid var(--color-border);
    padding: 1rem 2rem;
  }

  .wrapper {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 0 auto;
    flex-wrap: wrap;
    gap: 1rem;
  }

  .brand {
    display: flex;
    flex-direction: column;
  }

  h1 {
    font-weight: bold;
    font-size: 1.5rem;
    color: var(--color-heading);
    margin: 0;
  }

  .user-welcome {
    font-size: 0.85rem;
    color: hsla(160, 100%, 37%, 1);
  }

  nav {
    display: flex;
    gap: 0.5rem;
    flex-wrap: wrap;
    align-items: center;
  }

  nav a {
    font-size: 0.95rem;
    color: var(--color-text);
    text-decoration: none;
    padding: 0.35rem 0.6rem;
    border-radius: 4px;
    transition: all 0.2s;
  }

  nav a.router-link-exact-active {
    color: var(--color-background);
    background-color: hsla(160, 100%, 37%, 1);
  }

  nav a:hover {
    background-color: var(--color-border);
  }

  main {
    flex-grow: 1;
    padding: 2rem;
    margin: 0 auto;
    width: 100%;
  }

  .link-nomina-calculo {
    background-color: hsla(160, 100%, 37%, 0.1);
    border: 1px solid hsla(160, 100%, 37%, 0.2);
    font-weight: bold;
  }

  .link-consulta {
    border: 1px solid var(--color-border);
  }

  .btn-logout {
    background-color: #dc3545;
    color: white;
    border: none;
    padding: 0.35rem 0.8rem;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.9rem;
    margin-left: 0.5rem;
  }

  .btn-logout:hover {
    background-color: #c82333;
  }
</style>
