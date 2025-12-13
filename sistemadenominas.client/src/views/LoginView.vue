<template>
  <div class="login-container">
    <div class="card login-card">
      <div class="tabs">
        <button :class="{ active: mode === 'login' }"
                @click="mode = 'login'">
          Iniciar Sesión
        </button>
        <button :class="{ active: mode === 'register' }"
                @click="mode = 'register'">
          Registrarse
        </button>
      </div>

      <h2 class="title">{{ mode === 'login' ? 'Bienvenido' : 'Crear Cuenta' }}</h2>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="username">Usuario:</label>
          <input type="text"
                 id="username"
                 v-model="form.username"
                 placeholder="Ej: admin"
                 required>
        </div>

        <div class="form-group">
          <label for="password">Contraseña:</label>
          <input type="password"
                 id="password"
                 v-model="form.password"
                 placeholder="********"
                 required>
        </div>

        <p v-if="error" class="error-msg">{{ error }}</p>
        <p v-if="success" class="success-msg">{{ success }}</p>

        <button type="submit" class="btn-primary full-width">
          {{ mode === 'login' ? 'Ingresar' : 'Registrar Usuario' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const mode = ref<'login' | 'register'>('login');
const error = ref('');
const success = ref('');

const form = reactive({
  username: '',
  password: ''
});

function handleSubmit() {
  error.value = '';
  success.value = '';

  if (mode.value === 'register') {
    registerUser();
  } else {
    loginUser();
  }
}

function registerUser() {
  // Obtener usuarios existentes
  const usersStr = localStorage.getItem('db_users');
  const users = usersStr ? JSON.parse(usersStr) : [];

  // Verificar si existe
  const exists = users.find((u: any) => u.username === form.username);
  if (exists) {
    error.value = 'El usuario ya existe.';
    return;
  }

  // Guardar nuevo usuario
  users.push({ ...form });
  localStorage.setItem('db_users', JSON.stringify(users));

  success.value = 'Usuario registrado con éxito. Ahora puedes iniciar sesión.';
  mode.value = 'login';
  form.password = ''; // Limpiar contraseña
}

function loginUser() {
  const usersStr = localStorage.getItem('db_users');
  const users = usersStr ? JSON.parse(usersStr) : [];

  // Buscar usuario coincidente
  const user = users.find((u: any) => u.username === form.username && u.password === form.password);

  if (user) {
    // Guardar sesión "token" simulado
    const sessionToken = btoa(JSON.stringify({
      user: user.username,
      date: new Date().toISOString()
    }));

    localStorage.setItem('session_token', sessionToken);
    localStorage.setItem('current_user', user.username);

    // Disparar evento para actualizar App.vue (opcional si usamos watch en router)
    window.dispatchEvent(new Event('storage'));

    router.push('/');
  } else {
    error.value = 'Credenciales inválidas.';
  }
}
</script>

<style scoped>
  .login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 80vh;
  }

  .login-card {
    width: 100%;
    max-width: 400px;
    padding: 2rem;
    background: var(--color-background-soft);
    border: 1px solid var(--color-border);
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0,0,0,0.1);
  }

  .tabs {
    display: flex;
    margin-bottom: 2rem;
    border-bottom: 1px solid var(--color-border);
  }

    .tabs button {
      flex: 1;
      background: none;
      border: none;
      padding: 1rem;
      font-weight: bold;
      color: var(--color-text-light-2);
      cursor: pointer;
      transition: all 0.3s;
    }

      .tabs button.active {
        color: hsla(160, 100%, 37%, 1);
        border-bottom: 2px solid hsla(160, 100%, 37%, 1);
      }

  .title {
    text-align: center;
    margin-bottom: 1.5rem;
    color: var(--color-heading);
  }

  .form-group {
    margin-bottom: 1.2rem;
    display: flex;
    flex-direction: column;
  }

    .form-group label {
      margin-bottom: 0.5rem;
      font-weight: 500;
    }

    .form-group input {
      padding: 0.8rem;
      border: 1px solid var(--color-border);
      border-radius: 6px;
      background: var(--color-background);
      color: var(--color-text);
    }

  .full-width {
    width: 100%;
    margin-top: 1rem;
    padding: 0.8rem;
    font-size: 1rem;
  }

  .error-msg {
    color: #dc3545;
    font-size: 0.9rem;
    text-align: center;
    margin: 0.5rem 0;
  }

  .success-msg {
    color: #28a745;
    font-size: 0.9rem;
    text-align: center;
    margin: 0.5rem 0;
  }

  .dummy-info {
    margin-top: 1.5rem;
    text-align: center;
    color: var(--color-text-light-2);
    font-size: 0.8rem;
  }

  .btn-primary {
    background-color: hsla(160, 100%, 37%, 1);
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
  }
</style>
