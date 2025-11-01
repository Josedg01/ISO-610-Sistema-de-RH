<template>
  <div class="gestion-puestos">
    <h2>Gestión de Puestos</h2>

    <div class="card form-card">
      <h3>Registrar Nuevo Puesto</h3>
      <form @submit.prevent="crearPuesto">
        <div class="form-group">
          <label for="nombre">Nombre del Puesto:</label>
          <input type="text" id="nombre" v-model="nuevoPuesto.Nombre" required>
        </div>
        <div class="form-group">
          <label for="nivelRiesgo">Nivel de Riesgo (1-5):</label>
          <input type="number" id="nivelRiesgo" v-model="nuevoPuesto.NivelDeRiesgo" min="1" max="5" required>
        </div>
        <div class="form-group">
          <label for="minSalario">Salario Mínimo:</label>
          <input type="number" step="0.01" id="minSalario" v-model="nuevoPuesto.MinimoSalario" required>
        </div>
        <div class="form-group">
          <label for="maxSalario">Salario Máximo:</label>
          <input type="number" step="0.01" id="maxSalario" v-model="nuevoPuesto.MaximoSalario" required>
        </div>
        <button type="submit" class="btn-primary">Guardar Puesto</button>
      </form>
    </div>

    <div class="card list-card">
      <h3>Listado de Puestos</h3>
      <div v-if="loading" class="loading">Cargando...</div>
      <table v-if="!loading && puestos.length > 0">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Nivel Riesgo</th>
            <th>Salario Mínimo</th>
            <th>Salario Máximo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="puesto in puestos" :key="puesto.id">
            <td>{{ puesto.id }}</td>
            <td>{{ puesto.Nombre }}</td>
            <td>{{ puesto.NivelDeRiesgo }}</td>
            <td>{{ formatCurrency(puesto.MinimoSalario) }}</td>
            <td>{{ formatCurrency(puesto.MaximoSalario) }}</td>
            <td>
              <button class="btn-secondary">Editar</button>
              <button class="btn-danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && puestos.length === 0">No hay puestos registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Interface basada en el modelo Puestos.cs
interface Puesto {
  id: number;
  Nombre: string;
  NivelDeRiesgo: number;
  MinimoSalario: number; // Coincide con el 'decimal' de C# (manejado como number en TS)
  MaximoSalario: number;
}

const puestos = ref<Puesto[]>([]);
const loading = ref(true);
const nuevoPuesto = ref<Omit<Puesto, 'id'>>({
  Nombre: '',
  NivelDeRiesgo: 1,
  MinimoSalario: 0,
  MaximoSalario: 0,
});

// Helper para formatear moneda
function formatCurrency(value: number) {
  return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
}

async function getPuestos() {
  loading.value = true;
  try {
    const response = await fetch('/Puestos'); // Llama al API
    if (response.ok) {
      puestos.value = await response.json();
    } else {
      console.error('Error al cargar puestos:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  } finally {
    loading.value = false;
  }
}

async function crearPuesto() {
  try {
    const response = await fetch('/Puestos', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(nuevoPuesto.value),
    });

    if (response.ok) {
      // Limpiar formulario
      nuevoPuesto.value = {
        Nombre: '',
        NivelDeRiesgo: 1,
        MinimoSalario: 0,
        MaximoSalario: 0,
      };
      // Recargar la lista
      await getPuestos();
    } else {
      console.error('Error al crear puesto:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  }
}

// Cargar los datos cuando el componente se monte
onMounted(() => {
  getPuestos();
});
</script>

<style scoped>
  /* Estilos consistentes con los otros módulos */
  .gestion-puestos {
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
