<template>
  <div class="gestion-puestos">
    <h2>Gestión de Puestos</h2>

    <div class="card form-card">
      <h3>{{ modoEdicion ? 'Editar Puesto' : 'Registrar Nuevo Puesto' }}</h3>
      <form @submit.prevent="guardarPuesto">
        <div class="form-group">
          <label for="nombre">Nombre del Puesto:</label>
          <input type="text" id="nombre" v-model="puestoForm.nombre" required>
        </div>
        <div class="form-group">
          <label for="nivelRiesgo">Nivel de Riesgo (1-5):</label>
          <input type="number" id="nivelRiesgo" v-model="puestoForm.nivelDeRiesgo" min="1" max="5" required>
        </div>
        <div class="form-group">
          <label for="minSalario">Salario Mínimo:</label>
          <input type="number" step="0.01" id="minSalario" v-model="puestoForm.minimoSalario" required>
        </div>
        <div class="form-group">
          <label for="maxSalario">Salario Máximo:</label>
          <input type="number" step="0.01" id="maxSalario" v-model="puestoForm.maximoSalario" required>
        </div>

        <div class="button-group">
          <button type="submit" class="btn-primary">
            {{ modoEdicion ? 'Actualizar' : 'Guardar' }}
          </button>
          <button type="button" v-if="modoEdicion" @click="cancelarEdicion" class="btn-secondary">
            Cancelar
          </button>
        </div>
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
            <td>{{ puesto.nombre }}</td>
            <td>{{ puesto.nivelDeRiesgo }}</td>
            <td>{{ formatCurrency(puesto.minimoSalario) }}</td>
            <td>{{ formatCurrency(puesto.maximoSalario) }}</td>
            <td>
              <button class="btn-secondary btn-sm" @click="cargarEdicion(puesto)">Editar</button>
              <button class="btn-danger btn-sm" @click="eliminarPuesto(puesto.id)">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && puestos.length === 0">No hay puestos registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, reactive } from 'vue'

  interface Puesto {
    id: number;
    nombre: string;
    nivelDeRiesgo: number;
    minimoSalario: number;
    maximoSalario: number;
  }

  const puestos = ref<Puesto[]>([]);
  const loading = ref(true);
  const modoEdicion = ref(false);
  const idEnEdicion = ref<number | null>(null);

  const puestoForm = reactive({
    nombre: '',
    nivelDeRiesgo: 1,
    minimoSalario: 0,
    maximoSalario: 0,
  });

  function formatCurrency(value: number) {
    return (value || 0).toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
  }

  async function getPuestos() {
    loading.value = true;
    try {
      const response = await fetch('/Puestos');
      if (response.ok) {
        puestos.value = await response.json();
      }
    } catch (error) {
      console.error(error);
    } finally {
      loading.value = false;
    }
  }

  async function guardarPuesto() {
    const url = modoEdicion.value ? `/Puestos/${idEnEdicion.value}` : '/Puestos';
    const method = modoEdicion.value ? 'PUT' : 'POST';
    const bodyData = { ...puestoForm, id: modoEdicion.value ? idEnEdicion.value : 0 };

    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(bodyData),
      });

      if (response.ok) {
        cancelarEdicion();
        await getPuestos();
      } else {
        alert('Error al guardar puesto.');
      }
    } catch (error) {
      console.error(error);
    }
  }

  async function eliminarPuesto(id: number) {
    if (!confirm('¿Eliminar este puesto permanentemente?')) return;
    try {
      const res = await fetch(`/Puestos/${id}`, { method: 'DELETE' });
      if (res.ok) await getPuestos();
      else alert('Error al eliminar.');
    } catch (e) {
      console.error(e);
    }
  }

  function cargarEdicion(item: Puesto) {
    modoEdicion.value = true;
    idEnEdicion.value = item.id;
    puestoForm.nombre = item.nombre;
    puestoForm.nivelDeRiesgo = item.nivelDeRiesgo;
    puestoForm.minimoSalario = item.minimoSalario;
    puestoForm.maximoSalario = item.maximoSalario;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function cancelarEdicion() {
    modoEdicion.value = false;
    idEnEdicion.value = null;
    puestoForm.nombre = '';
    puestoForm.nivelDeRiesgo = 1;
    puestoForm.minimoSalario = 0;
    puestoForm.maximoSalario = 0;
  }

  onMounted(() => {
    getPuestos();
  });
</script>

<style scoped>
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
    }

    .form-group input {
      padding: 0.75rem;
      border: 1px solid var(--color-border);
      border-radius: 4px;
    }

  .button-group {
    grid-column: 1 / -1;
    display: flex;
    gap: 1rem;
    margin-top: 1rem;
  }

  button {
    padding: 0.5rem 1rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
  }

  .btn-primary {
    background-color: hsla(160, 100%, 37%, 1);
    color: white;
  }

  .btn-secondary {
    background-color: #6c757d;
    color: white;
  }

  .btn-danger {
    background-color: #dc3545;
    color: white;
    margin-left: 0.5rem;
  }

  .btn-sm {
    padding: 0.25rem 0.5rem;
    font-size: 0.875rem;
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
  }

  .loading {
    padding: 2rem;
    text-align: center;
  }
</style>
