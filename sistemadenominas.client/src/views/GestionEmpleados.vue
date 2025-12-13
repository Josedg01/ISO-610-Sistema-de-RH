<template>
  <div class="gestion-empleados">
    <h2>Gestión de Empleados</h2>

    <div class="card form-card">
      <h3>{{ modoEdicion ? 'Editar Empleado' : 'Registrar Nuevo Empleado' }}</h3>
      <form @submit.prevent="guardarEmpleado">
        <div class="form-group">
          <label for="nombre">Nombre:</label>
          <input type="text" id="nombre" v-model="empleadoForm.nombre" required>
        </div>
        <div class="form-group">
          <label for="cedula">Cédula:</label>
          <input type="text" id="cedula" v-model="empleadoForm.cedula" required>
        </div>
        <div class="form-group">
          <label for="salario">Salario Mensual:</label>
          <input type="number" id="salario" v-model="empleadoForm.salarioMensual" required>
        </div>
        <div class="form-group">
          <label for="idDepartamento">ID Depto:</label>
          <input type="number" id="idDepartamento" v-model="empleadoForm.idDepartamento" required>
        </div>
        <div class="form-group">
          <label for="idPuesto">ID Puesto:</label>
          <input type="number" id="idPuesto" v-model="empleadoForm.idPuesto" required>
        </div>
        <div class="form-group">
          <label for="idNomina">ID Nomina:</label>
          <input type="number" id="idNomina" v-model="empleadoForm.idNomina" required>
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
            <td>{{ formatCurrency(empleado.salarioMensual) }}</td>
            <td>
              <button class="btn-secondary btn-sm" @click="cargarEdicion(empleado)">Editar</button>
              <button class="btn-danger btn-sm" @click="eliminarEmpleado(empleado.id)">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && empleados.length === 0">No hay empleados registrados.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, reactive } from 'vue'

  interface Empleado {
    id: number;
    cedula: string;
    nombre: string;
    idDepartamento: number;
    idPuesto: number;
    salarioMensual: number;
    idNomina: number;
  }

  // Estado
  const empleados = ref<Empleado[]>([]);
  const loading = ref(true);
  const modoEdicion = ref(false);
  const idEnEdicion = ref<number | null>(null);

  // Formulario reactivo
  const empleadoForm = reactive({
    nombre: '',
    cedula: '',
    salarioMensual: 0,
    idDepartamento: 1,
    idPuesto: 1,
    idNomina: 1
  });

  // --- Funciones CRUD ---

  async function getEmpleados() {
    loading.value = true;
    try {
      const response = await fetch('/Empleados');
      if (response.ok) {
        empleados.value = await response.json();
      }
    } catch (error) {
      console.error('Error:', error);
    } finally {
      loading.value = false;
    }
  }

  async function guardarEmpleado() {
    const url = modoEdicion.value ? `/Empleados/${idEnEdicion.value}` : '/Empleados';
    const method = modoEdicion.value ? 'PUT' : 'POST';

    // Si es PUT, necesitamos enviar el ID en el cuerpo también según tu controlador
    const bodyData = {
      ...empleadoForm,
      id: modoEdicion.value ? idEnEdicion.value : 0
    };

    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(bodyData),
      });

      if (response.ok) {
        cancelarEdicion(); // Resetea el form
        await getEmpleados(); // Recarga la lista
      } else {
        const errorText = await response.text();
        alert('Error al guardar: ' + errorText);
      }
    } catch (error) {
      console.error('Error de red:', error);
    }
  }

  async function eliminarEmpleado(id: number) {
    if (!confirm('¿Está seguro de eliminar este empleado? Esta acción no se puede deshacer.')) return;

    try {
      const response = await fetch(`/Empleados/${id}`, { method: 'DELETE' });
      if (response.ok) {
        await getEmpleados();
      } else {
        alert('Error al eliminar empleado.');
      }
    } catch (error) {
      console.error('Error:', error);
    }
  }

  // --- Helpers de UI ---

  function cargarEdicion(emp: Empleado) {
    modoEdicion.value = true;
    idEnEdicion.value = emp.id;
    // Copiamos datos al form
    empleadoForm.nombre = emp.nombre;
    empleadoForm.cedula = emp.cedula;
    empleadoForm.salarioMensual = emp.salarioMensual;
    empleadoForm.idDepartamento = emp.idDepartamento;
    empleadoForm.idPuesto = emp.idPuesto;
    empleadoForm.idNomina = emp.idNomina;

    // Scroll hacia el formulario
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function cancelarEdicion() {
    modoEdicion.value = false;
    idEnEdicion.value = null;
    // Reset form
    empleadoForm.nombre = '';
    empleadoForm.cedula = '';
    empleadoForm.salarioMensual = 0;
    empleadoForm.idDepartamento = 1;
    empleadoForm.idPuesto = 1;
    empleadoForm.idNomina = 1;
  }

  function formatCurrency(value: number) {
    return (value || 0).toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
  }

  onMounted(() => {
    getEmpleados();
  });
</script>

<style scoped>
  /* Reutilizamos estilos previos y agregamos button-group */
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
