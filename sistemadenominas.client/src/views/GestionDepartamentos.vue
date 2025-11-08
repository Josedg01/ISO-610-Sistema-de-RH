<template>
  <div class="gestion-departamentos">
    <h2>Gestión de Departamentos</h2>

    <div class="card form-card">
      <h3>Registrar Nuevo Departamento</h3>
      <form @submit.prevent="crearDepartamento">
        <div class="form-group">
          <label for="nombre">Nombre:</label>
          <input type="text" id="nombre" v-model="nuevoDepto.Nombre" required>
        </div>
        <div class="form-group">
          <label for="ubicacion">Ubicación Física:</label>
          <input type="text" id="ubicacion" v-model="nuevoDepto.UbicacionFisica" required>
        </div>
        <div class="form-group">
          <label for="idResponsable">ID Responsable Área:</label>
          <input type="number" id="idResponsable" v-model="nuevoDepto.idResponsableArea" required>
        </div>
        <button type="submit" class="btn-primary">Guardar Departamento</button>
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
              <button class="btn-secondary">Editar</button>
              <button class="btn-danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && departamentos.length === 0">No hay departamentos registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Interface basada en el modelo Departamento.cs
interface Departamento {
  id: number;
  Nombre: string;
  UbicacionFisica: string;
  idResponsableArea: number;
}

const departamentos = ref<Departamento[]>([]);
const loading = ref(true);
const nuevoDepto = ref<Omit<Departamento, 'id'>>({
  Nombre: '',
  UbicacionFisica: '',
  idResponsableArea: 0,
});

async function getDepartamentos() {
  loading.value = true;
  try {
    const response = await fetch('/Departamentos'); // Llama al API
    if (response.ok) {
      departamentos.value = await response.json();
    } else {
      console.error('Error al cargar departamentos:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  } finally {
    loading.value = false;
  }
}

async function crearDepartamento() {
  try {
    const response = await fetch('/Departamentos', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(nuevoDepto.value),
    });

    if (response.ok) {
      // Limpiar formulario
      nuevoDepto.value = {
        Nombre: '',
        UbicacionFisica: '',
        idResponsableArea: 0,
      };
      // Recargar la lista
      await getDepartamentos();
    } else {
      console.error('Error al crear departamento:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  }
}

onMounted(() => {
  getDepartamentos();
});
</script>

<style scoped>
  /* Estilos copiados de GestionEmpleados.vue */
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
