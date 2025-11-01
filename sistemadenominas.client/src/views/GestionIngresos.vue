<template>
  <div class="gestion-ingresos">
    <h2>Gestión de Tipos de Ingresos</h2>

    <div class="card form-card">
      <h3>Registrar Nuevo Tipo de Ingreso</h3>
      <form @submit.prevent="crearIngreso">
        <div class="form-group">
          <label for="nombre">Nombre (Ej: Bono, Comisión):</label>
          <input type="text" id="nombre" v-model="nuevoIngreso.Nombre" required>
        </div>
        <div class="form-group">
          <label for="idEmpleado">ID Empleado (Asociado):</label>
          <input type="number" id="idEmpleado" v-model="nuevoIngreso.idEmpleado" required>
        </div>
        <div class="form-group">
          <label for="estado">Estado (Activo/Inactivo):</label>
          <input type="text" id="estado" v-model="nuevoIngreso.Estado" required>
        </div>
        <button type="submit" class="btn-primary">Guardar Tipo de Ingreso</button>
      </form>
    </div>

    <div class="card list-card">
      <h3>Listado de Tipos de Ingresos</h3>
      <div v-if="loading" class="loading">Cargando...</div>
      <table v-if="!loading && ingresos.length > 0">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>ID Empleado</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="ingreso in ingresos" :key="ingreso.id">
            <td>{{ ingreso.id }}</td>
            <td>{{ ingreso.Nombre }}</td>
            <td>{{ ingreso.idEmpleado }}</td>
            <td>{{ ingreso.Estado }}</td>
            <td>
              <button class="btn-secondary">Editar</button>
              <button class="btn-danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && ingresos.length === 0">No hay tipos de ingresos registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Interface basada en el modelo TipodeIngresos.cs
interface TipodeIngresos {
  id: number;
  Nombre: string;
  idEmpleado: number;
  Estado: string;
}

const ingresos = ref<TipodeIngresos[]>([]);
const loading = ref(true);
const nuevoIngreso = ref<Omit<TipodeIngresos, 'id'>>({
  Nombre: '',
  idEmpleado: 0,
  Estado: 'Activo', // Valor por defecto
});

async function getIngresos() {
  loading.value = true;
  try {
    const response = await fetch('/TiposDeIngresos'); // Llama al API
    if (response.ok) {
      ingresos.value = await response.json();
    } else {
      console.error('Error al cargar tipos de ingresos:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  } finally {
    loading.value = false;
  }
}

async function crearIngreso() {
  try {
    const response = await fetch('/TiposDeIngresos', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(nuevoIngreso.value),
    });

    if (response.ok) {
      // Limpiar formulario
      nuevoIngreso.value = {
        Nombre: '',
        idEmpleado: 0,
        Estado: 'Activo',
      };
      // Recargar la lista
      await getIngresos();
    } else {
      console.error('Error al crear tipo de ingreso:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  }
}

// Cargar los datos cuando el componente se monte
onMounted(() => {
  getIngresos();
});
</script>

<style scoped>
  /* Estilos consistentes con los otros módulos */
  .gestion-ingresos {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .card {
    background: var(--color-background-soft);
    border: 1px solid var(--color-border);
    border-radius: 8px;
    padding: 1.5rem;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  }

  .form-card form {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1rem;
    margin-top: 1rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
  }

  .form-group label {
    margin-bottom: 0.5rem;
    font-weight: 500;
    color: var(--color-text-light-2);
  }

  .form-group input {
    padding: 0.75rem;
    border: 1px solid var(--color-border);
    border-radius: 4px;
    background: var(--color-background);
    color: var(--color-text);
  }

  button {
    padding: 0.75rem 1rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
  }

  .btn-primary {
    background-color: hsla(160, 100%, 37%, 1);
    color: white;
    grid-column: 1 / 1;
    margin-top: 1rem;
  }

  .btn-secondary {
    background-color: #f0f0f0;
    color: #333;
    margin-right: 0.5rem;
  }

  .btn-danger {
    background-color: #ffcccc;
    color: #a00;
  }

  table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
  }

  th, td {
    padding: 0.75rem 1rem;
    text-align: left;
    border-bottom: 1px solid var(--color-border);
  }

  thead th {
    background-color: var(--color-background-mute);
    color: var(--color-heading);
  }

  .loading {
    padding: 2rem;
    text-align: center;
    color: var(--color-text-light-2);
  }
</style>
