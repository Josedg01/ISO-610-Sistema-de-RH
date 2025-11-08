<template>
  <div class="gestion-empleados">
    <h2>Gestión de Empleados</h2>

    <div class="card form-card">
      <h3>Registrar Nuevo Empleado</h3>
      <form @submit.prevent="crearEmpleado">
        <div class="form-group">
          <label for="nombre">Nombre:</label>
          <input type="text" id="nombre" v-model="nuevoEmpleado.Nombre" required>
        </div>
        <div class="form-group">
          <label for="cedula">Cédula:</label>
          <input type="text" id="cedula" v-model="nuevoEmpleado.Cedula" required>
        </div>
        <div class="form-group">
          <label for="salario">Salario Mensual:</label>
          <input type="number" id="salario" v-model="nuevoEmpleado.SalarioMensual" required>
        </div>
        <div class="form-group">
          <label for="idDepartamento">ID Depto:</label>
          <input type="number" id="idDepartamento" v-model="nuevoEmpleado.idDepartamento" required>
        </div>
        <div class="form-group">
          <label for="idPuesto">ID Puesto:</label>
          <input type="number" id="idPuesto" v-model="nuevoEmpleado.idPuesto" required>
        </div>
        <div class="form-group">
          <label for="idNomina">ID Nomina:</label>
          <input type="number" id="idNomina" v-model="nuevoEmpleado.idNomina" required>
        </div>
        <button type="submit" class="btn-primary">Guardar Empleado</button>
      </form>
    </div>

    <div class="card list-card">
      <h3>Listado de Empleados</h3>
      <div v-if="loading" class="loading">Cargando...</div>
      <table v-if="!loading && empleados.length > 0">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Cédula</th>
            <th>Salario Mensual</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="empleado in empleados" :key="empleado.id">
            <td>{{ empleado.id }}</td>
            <td>{{ empleado.nombre }}</td>
            <td>{{ empleado.cedula }}</td>
            <td>{{ empleado.salarioMensual.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' }) }}</td>
            <td>
              <button class="btn-secondary">Editar</button>
              <button class="btn-danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && empleados.length === 0">No hay empleados registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Interface basada en el modelo Empleado.cs
interface Empleado {
  id: number;
  Cedula: string;
  Nombre: string;
  idDepartamento: number;
  idPuesto: number;
  SalarioMensual: number;
  idNomina: number;
}

const empleados = ref<Empleado[]>([]);
const loading = ref(true);
const nuevoEmpleado = ref<Omit<Empleado, 'id'>>({
  Nombre: '',
  Cedula: '',
  SalarioMensual: 0,
  idDepartamento: 1, // Valor por defecto
  idPuesto: 1, // Valor por defecto
  idNomina: 1 // Valor por defecto
});

async function getEmpleados() {
  loading.value = true;
  try {
    const response = await fetch('/Empleados'); // Llama al API
    if (response.ok) {
      empleados.value = await response.json();
    } else {
      console.error('Error al cargar empleados:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  } finally {
    loading.value = false;
  }
}

async function crearEmpleado() {
  try {
    const response = await fetch('/Empleados', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(nuevoEmpleado.value),
    });

    if (response.ok) {
      // Limpiar formulario
      nuevoEmpleado.value = {
        Nombre: '',
        Cedula: '',
        SalarioMensual: 0,
        idDepartamento: 1,
        idPuesto: 1,
        idNomina: 1
      };
      // Recargar la lista
      await getEmpleados();
    } else {
      console.error('Error al crear empleado:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  }
}

// Cargar los datos cuando el componente se monte
onMounted(() => {
  getEmpleados();
});
</script>

<style scoped>
  .gestion-empleados {
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
