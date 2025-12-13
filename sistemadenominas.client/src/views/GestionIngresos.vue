<template>
  <div class="gestion-ingresos">
    <h2>Gestión de Tipos de Ingresos</h2>

    <div class="card form-card">
      <h3>{{ modoEdicion ? 'Editar Ingreso' : 'Registrar Nuevo Tipo de Ingreso' }}</h3>
      <form @submit.prevent="guardarIngreso">
        <div class="form-group">
          <label for="nombre">Nombre (Ej: Bono, Comisión):</label>
          <input type="text" id="nombre" v-model="ingresoForm.nombre" required>
        </div>
        <div class="form-group">
          <label for="idEmpleado">ID Empleado (Asociado):</label>
          <input type="number" id="idEmpleado" v-model="ingresoForm.idEmpleado" required>
        </div>
        <div class="form-group">
          <label for="estado">Estado (Activo/Inactivo):</label>
          <input type="text" id="estado" v-model="ingresoForm.estado" required>
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
            <td>{{ ingreso.nombre }}</td>
            <td>{{ ingreso.idEmpleado }}</td>
            <td>{{ ingreso.estado }}</td>
            <td>
              <button class="btn-secondary btn-sm" @click="cargarEdicion(ingreso)">Editar</button>
              <button class="btn-danger btn-sm" @click="eliminarIngreso(ingreso.id)">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && ingresos.length === 0">No hay tipos de ingresos registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, reactive } from 'vue'

  interface TipodeIngresos {
    id: number;
    nombre: string;
    idEmpleado: number;
    estado: string;
  }

  const ingresos = ref<TipodeIngresos[]>([]);
  const loading = ref(true);
  const modoEdicion = ref(false);
  const idEnEdicion = ref<number | null>(null);

  const ingresoForm = reactive({
    nombre: '',
    idEmpleado: 0,
    estado: 'Activo',
  });

  async function getIngresos() {
    loading.value = true;
    try {
      const response = await fetch('/TiposDeIngresos');
      if (response.ok) {
        ingresos.value = await response.json();
      }
    } catch (error) {
      console.error(error);
    } finally {
      loading.value = false;
    }
  }

  async function guardarIngreso() {
    const url = modoEdicion.value ? `/TiposDeIngresos/${idEnEdicion.value}` : '/TiposDeIngresos';
    const method = modoEdicion.value ? 'PUT' : 'POST';
    const bodyData = { ...ingresoForm, id: modoEdicion.value ? idEnEdicion.value : 0 };

    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(bodyData),
      });

      if (response.ok) {
        cancelarEdicion();
        await getIngresos();
      } else {
        alert('Error al guardar ingreso.');
      }
    } catch (error) {
      console.error(error);
    }
  }

  async function eliminarIngreso(id: number) {
    if (!confirm('¿Seguro que desea eliminar este tipo de ingreso?')) return;
    try {
      const res = await fetch(`/TiposDeIngresos/${id}`, { method: 'DELETE' });
      if (res.ok) await getIngresos();
      else alert('No se pudo eliminar.');
    } catch (e) {
      console.error(e);
    }
  }

  function cargarEdicion(item: TipodeIngresos) {
    modoEdicion.value = true;
    idEnEdicion.value = item.id;
    ingresoForm.nombre = item.nombre;
    ingresoForm.idEmpleado = item.idEmpleado;
    ingresoForm.estado = item.estado;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function cancelarEdicion() {
    modoEdicion.value = false;
    idEnEdicion.value = null;
    ingresoForm.nombre = '';
    ingresoForm.idEmpleado = 0;
    ingresoForm.estado = 'Activo';
  }

  onMounted(() => {
    getIngresos();
  });
</script>

<style scoped>
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
