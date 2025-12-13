<template>
  <div class="gestion-departamentos">
    <h2>Gestión de Departamentos</h2>

    <div class="card form-card">
      <h3>{{ modoEdicion ? 'Editar Departamento' : 'Registrar Nuevo Departamento' }}</h3>
      <form @submit.prevent="guardarDepartamento">
        <div class="form-group">
          <label for="nombre">Nombre:</label>
          <input type="text" id="nombre" v-model="deptoForm.nombre" required>
        </div>
        <div class="form-group">
          <label for="ubicacion">Ubicación Física:</label>
          <input type="text" id="ubicacion" v-model="deptoForm.ubicacionFisica" required>
        </div>
        <div class="form-group">
          <label for="idResponsable">ID Responsable Área:</label>
          <input type="number" id="idResponsable" v-model="deptoForm.idResponsableArea" required>
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
      <h3>Listado de Departamentos</h3>
      <div v-if="loading" class="loading">Cargando...</div>
      <table v-if="!loading && departamentos.length > 0">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Ubicación</th>
            <th>ID Responsable</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="depto in departamentos" :key="depto.id">
            <td>{{ depto.id }}</td>
            <td>{{ depto.nombre }}</td>
            <td>{{ depto.ubicacionFisica }}</td>
            <td>{{ depto.idResponsableArea }}</td>
            <td>
              <button class="btn-secondary btn-sm" @click="cargarEdicion(depto)">Editar</button>
              <button class="btn-danger btn-sm" @click="eliminarDepartamento(depto.id)">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && departamentos.length === 0">No hay departamentos registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, reactive } from 'vue'

  interface Departamento {
    id: number;
    nombre: string;
    ubicacionFisica: string;
    idResponsableArea: number;
  }

  const departamentos = ref<Departamento[]>([]);
  const loading = ref(true);
  const modoEdicion = ref(false);
  const idEnEdicion = ref<number | null>(null);

  const deptoForm = reactive({
    nombre: '',
    ubicacionFisica: '',
    idResponsableArea: 0,
  });

  async function getDepartamentos() {
    loading.value = true;
    try {
      const response = await fetch('/Departamentos');
      if (response.ok) {
        departamentos.value = await response.json();
      }
    } catch (error) {
      console.error(error);
    } finally {
      loading.value = false;
    }
  }

  async function guardarDepartamento() {
    const url = modoEdicion.value ? `/Departamentos/${idEnEdicion.value}` : '/Departamentos';
    const method = modoEdicion.value ? 'PUT' : 'POST';

    const bodyData = {
      ...deptoForm,
      id: modoEdicion.value ? idEnEdicion.value : 0
    };

    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(bodyData),
      });

      if (response.ok) {
        cancelarEdicion();
        await getDepartamentos();
      } else {
        const msg = await response.text();
        alert('Error: ' + msg);
      }
    } catch (error) {
      console.error(error);
    }
  }

  async function eliminarDepartamento(id: number) {
    if (!confirm('¿Seguro que desea eliminar este departamento?')) return;
    try {
      const res = await fetch(`/Departamentos/${id}`, { method: 'DELETE' });
      if (res.ok) await getDepartamentos();
      else alert('No se pudo eliminar el departamento.');
    } catch (e) {
      console.error(e);
    }
  }

  function cargarEdicion(item: Departamento) {
    modoEdicion.value = true;
    idEnEdicion.value = item.id;
    deptoForm.nombre = item.nombre;
    deptoForm.ubicacionFisica = item.ubicacionFisica;
    deptoForm.idResponsableArea = item.idResponsableArea;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function cancelarEdicion() {
    modoEdicion.value = false;
    idEnEdicion.value = null;
    deptoForm.nombre = '';
    deptoForm.ubicacionFisica = '';
    deptoForm.idResponsableArea = 0;
  }

  onMounted(() => {
    getDepartamentos();
  });
</script>

<style scoped>
  .gestion-departamentos {
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
