<template>
  <div class="gestion-deducciones">
    <h2>Gestión de Tipos de Deducciones</h2>

    <div class="card form-card">
      <h3>{{ modoEdicion ? 'Editar Deducción' : 'Registrar Nueva Deducción' }}</h3>
      <form @submit.prevent="guardarDeduccion">
        <div class="form-group">
          <label for="nombre">Nombre (Ej: AFP, ARS):</label>
          <input type="text" id="nombre" v-model="deduccionForm.nombre" required>
        </div>
        <div class="form-group">
          <label for="descripcion">Descripción:</label>
          <input type="text" id="descripcion" v-model="deduccionForm.descripcion">
        </div>
        <div class="form-group">
          <label for="montoFijo">Monto Fijo (RD$):</label>
          <input type="number" step="0.01" id="montoFijo" v-model.number="deduccionForm.montoFijo">
        </div>
        <div class="form-group">
          <label for="porcentaje">Porcentaje (%):</label>
          <input type="number" step="0.01" id="porcentaje" v-model.number="deduccionForm.porcentaje">
        </div>
        <div class="form-group">
          <label for="estado">Estado:</label>
          <input type="text" id="estado" v-model="deduccionForm.estado" required>
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
      <h3>Listado de Deducciones</h3>
      <div v-if="loading" class="loading">Cargando...</div>
      <table v-if="!loading && deducciones.length > 0">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Monto Fijo</th>
            <th>Porcentaje</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="deduccion in deducciones" :key="deduccion.id">
            <td>{{ deduccion.id }}</td>
            <td>{{ deduccion.nombre }}</td>
            <td>{{ formatCurrency(deduccion.montoFijo) }}</td>
            <td>{{ deduccion.porcentaje ? deduccion.porcentaje + '%' : 'N/A' }}</td>
            <td>{{ deduccion.estado }}</td>
            <td>
              <button class="btn-secondary btn-sm" @click="cargarEdicion(deduccion)">Editar</button>
              <button class="btn-danger btn-sm" @click="eliminarDeduccion(deduccion.id)">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && deducciones.length === 0">No hay deducciones registradas.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, reactive } from 'vue'

  interface TipodeDeduccion {
    id: number;
    nombre: string;
    descripcion?: string;
    montoFijo?: number;
    porcentaje?: number;
    estado: string;
  }

  const deducciones = ref<TipodeDeduccion[]>([]);
  const loading = ref(true);
  const modoEdicion = ref(false);
  const idEnEdicion = ref<number | null>(null);

  const deduccionForm = reactive({
    nombre: '',
    descripcion: '',
    montoFijo: undefined as number | undefined,
    porcentaje: undefined as number | undefined,
    estado: 'Activo',
  });

  function formatCurrency(value?: number) {
    if (value === null || value === undefined) return 'N/A';
    return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
  }

  async function getDeducciones() {
    loading.value = true;
    try {
      const response = await fetch('/TiposDeDeducciones');
      if (response.ok) {
        deducciones.value = await response.json();
      }
    } catch (error) {
      console.error(error);
    } finally {
      loading.value = false;
    }
  }

  async function guardarDeduccion() {
    const url = modoEdicion.value ? `/TiposDeDeducciones/${idEnEdicion.value}` : '/TiposDeDeducciones';
    const method = modoEdicion.value ? 'PUT' : 'POST';

    const payload = {
      ...deduccionForm,
      id: modoEdicion.value ? idEnEdicion.value : 0,
      montoFijo: deduccionForm.montoFijo || null,
      porcentaje: deduccionForm.porcentaje || null,
    };

    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        cancelarEdicion();
        await getDeducciones();
      } else {
        alert('Error al guardar deducción.');
      }
    } catch (error) {
      console.error(error);
    }
  }

  async function eliminarDeduccion(id: number) {
    if (!confirm('¿Eliminar esta deducción?')) return;
    try {
      const res = await fetch(`/TiposDeDeducciones/${id}`, { method: 'DELETE' });
      if (res.ok) await getDeducciones();
      else alert('Error al eliminar.');
    } catch (e) {
      console.error(e);
    }
  }

  function cargarEdicion(item: TipodeDeduccion) {
    modoEdicion.value = true;
    idEnEdicion.value = item.id;
    deduccionForm.nombre = item.nombre;
    deduccionForm.descripcion = item.descripcion || '';
    deduccionForm.montoFijo = item.montoFijo;
    deduccionForm.porcentaje = item.porcentaje;
    deduccionForm.estado = item.estado;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function cancelarEdicion() {
    modoEdicion.value = false;
    idEnEdicion.value = null;
    deduccionForm.nombre = '';
    deduccionForm.descripcion = '';
    deduccionForm.montoFijo = undefined;
    deduccionForm.porcentaje = undefined;
    deduccionForm.estado = 'Activo';
  }

  onMounted(() => {
    getDeducciones();
  });
</script>

<style scoped>
  .gestion-deducciones {
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
