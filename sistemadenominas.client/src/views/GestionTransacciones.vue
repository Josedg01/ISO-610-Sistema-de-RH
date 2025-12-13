<template>
  <div class="gestion-transacciones">
    <h2>Registro de Transacciones (Ingresos/Deducciones)</h2>

    <div class="card form-card">
      <h3>{{ modoEdicion ? 'Editar Transacción' : 'Registrar Nueva Transacción' }}</h3>
      <form @submit.prevent="guardarTransaccion">

        <div class="form-group">
          <label for="empleado">Empleado:</label>
          <select id="empleado" v-model="transaccionForm.idEmpleado" required>
            <option disabled value="0">Seleccione un empleado</option>
            <option v-for="emp in empleados" :key="emp.id" :value="emp.id">
              {{ emp.nombre }} ({{ emp.cedula }})
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="tipo">Tipo de Transacción:</label>
          <select id="tipo" v-model="transaccionForm.tipo" required @change="limpiarConcepto">
            <option disabled value="">Seleccione el tipo</option>
            <option value="Ingreso">Ingreso</option>
            <option value="Deduccion">Deducción</option>
          </select>
        </div>

        <div class="form-group" v-if="transaccionForm.tipo === 'Ingreso'">
          <label for="conceptoIngreso">Concepto (Ingreso):</label>
          <select id="conceptoIngreso" v-model="transaccionForm.conceptoId" required>
            <option disabled value="0">Seleccione un ingreso</option>
            <option v-for="ing in tiposIngresos" :key="ing.id" :value="ing.id">
              {{ ing.nombre }}
            </option>
          </select>
        </div>

        <div class="form-group" v-if="transaccionForm.tipo === 'Deduccion'">
          <label for="conceptoDeduccion">Concepto (Deducción):</label>
          <select id="conceptoDeduccion" v-model="transaccionForm.conceptoId" required>
            <option disabled value="0">Seleccione una deducción</option>
            <option v-for="ded in tiposDeducciones" :key="ded.id" :value="ded.id">
              {{ ded.nombre }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="monto">Monto (RD$):</label>
          <input type="number" step="0.01" id="monto" v-model.number="transaccionForm.monto" required>
        </div>

        <div class="form-group">
          <label for="fecha">Fecha:</label>
          <input type="date" id="fecha" v-model="transaccionForm.fecha" required>
        </div>

        <div class="form-group form-group-full">
          <label for="descripcion">Descripción (Opcional):</label>
          <input type="text" id="descripcion" v-model="transaccionForm.descripcion">
        </div>

        <div class="button-group form-group-full">
          <button type="submit" class="btn-primary" :disabled="!transaccionForm.tipo || !transaccionForm.conceptoId">
            {{ modoEdicion ? 'Actualizar' : 'Guardar Transacción' }}
          </button>
          <button type="button" v-if="modoEdicion" @click="cancelarEdicion" class="btn-secondary">
            Cancelar
          </button>
        </div>
      </form>
    </div>

    <div class="card list-card">
      <h3>Últimas Transacciones Registradas (Pendientes)</h3>
      <div v-if="loading.txs" class="loading">Cargando transacciones...</div>
      <table v-if="!loading.txs && transacciones.length > 0">
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Empleado</th>
            <th>Tipo</th>
            <th>Descripción</th>
            <th>Monto</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tx in transacciones" :key="tx.id">
            <td>{{ new Date(tx.fecha).toLocaleDateString() }}</td>
            <td>{{ getNombreEmpleado(tx.idEmpleado) }}</td>
            <td>{{ tx.tipo }}</td>
            <td>{{ tx.descripcion || 'N/A' }}</td>
            <td>{{ formatCurrency(tx.monto) }}</td>
            <td>{{ tx.estado }}</td>
            <td>
              <button class="btn-secondary btn-sm" @click="cargarEdicion(tx)">Editar</button>
              <button class="btn-danger btn-sm" @click="eliminarTransaccion(tx.id)">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading.txs && transacciones.length === 0">No hay transacciones pendientes.</p>
    </div>

  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, reactive } from 'vue'

  // --- Interfaces ---
  interface Transaccion {
    id: number;
    idEmpleado: number;
    tipo: string; // Usamos minúscula para consistencia con json del server
    conceptoId: number;
    descripcion?: string;
    monto: number;
    fecha: string;
    estado: string;
  }
  interface Empleado { id: number; nombre: string; cedula: string; }
  interface Concepto { id: number; nombre: string; }

  // --- Estado ---
  const empleados = ref<Empleado[]>([]);
  const tiposIngresos = ref<Concepto[]>([]);
  const tiposDeducciones = ref<Concepto[]>([]);
  const transacciones = ref<Transaccion[]>([]);

  const loading = reactive({
    empleados: true,
    ingresos: true,
    deducciones: true,
    txs: true
  });

  const modoEdicion = ref(false);
  const idEnEdicion = ref<number | null>(null);

  const transaccionForm = reactive({
    idEmpleado: 0,
    tipo: '',
    conceptoId: 0,
    descripcion: '',
    monto: 0,
    fecha: new Date().toISOString().split('T')[0],
    estado: 'Pendiente'
  });

  onMounted(async () => {
    await Promise.all([
      getEmpleados(),
      getTiposIngresos(),
      getTiposDeducciones(),
      getTransacciones()
    ]);
  });

  async function getEmpleados() {
    try {
      const res = await fetch('/Empleados');
      if (res.ok) empleados.value = await res.json();
    } catch (e) { console.error(e); }
    finally { loading.empleados = false; }
  }

  async function getTiposIngresos() {
    try {
      const res = await fetch('/TiposDeIngresos');
      if (res.ok) tiposIngresos.value = await res.json();
    } catch (e) { console.error(e); }
    finally { loading.ingresos = false; }
  }

  async function getTiposDeducciones() {
    try {
      const res = await fetch('/TiposDeDeducciones');
      if (res.ok) tiposDeducciones.value = await res.json();
    } catch (e) { console.error(e); }
    finally { loading.deducciones = false; }
  }

  async function getTransacciones() {
    loading.txs = true;
    try {
      const res = await fetch('/Transacciones');
      if (res.ok) {
        // Filtrar pendientes y mapear propiedades a minúscula si es necesario
        const data = await res.json();
        transacciones.value = data.filter((t: any) => t.estado === 'Pendiente').map((t: any) => ({
          id: t.id,
          idEmpleado: t.idEmpleado,
          tipo: t.tipo || t.Tipo, // Manejar variaciones de case
          conceptoId: t.conceptoId || t.ConceptoId,
          descripcion: t.descripcion,
          monto: t.monto,
          fecha: t.fecha,
          estado: t.estado
        }));
      }
    } catch (e) { console.error(e); }
    finally { loading.txs = false; }
  }

  function limpiarConcepto() {
    transaccionForm.conceptoId = 0;
  }

  function getNombreEmpleado(id: number): string {
    const emp = empleados.value.find(e => e.id === id);
    return emp ? emp.nombre : `ID: ${id}`;
  }

  function formatCurrency(value: number) {
    return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
  }

  // --- Funciones CRUD ---

  async function guardarTransaccion() {
    const url = modoEdicion.value ? `/Transacciones/${idEnEdicion.value}` : '/Transacciones';
    const method = modoEdicion.value ? 'PUT' : 'POST';

    const payload = {
      ...transaccionForm,
      id: modoEdicion.value ? idEnEdicion.value : 0
    };

    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        cancelarEdicion();
        await getTransacciones();
      } else {
        const msg = await response.text();
        alert('Error: ' + msg);
      }
    } catch (error) {
      console.error('Error de red:', error);
    }
  }

  async function eliminarTransaccion(id: number) {
    if (!confirm('¿Eliminar esta transacción?')) return;
    try {
      const response = await fetch(`/Transacciones/${id}`, { method: 'DELETE' });
      if (response.ok) await getTransacciones();
      else {
        const error = await response.text();
        alert(`Error: ${error}`);
      }
    } catch (error) {
      console.error(error);
    }
  }

  function cargarEdicion(item: Transaccion) {
    modoEdicion.value = true;
    idEnEdicion.value = item.id;

    transaccionForm.idEmpleado = item.idEmpleado;
    transaccionForm.tipo = item.tipo;
    transaccionForm.conceptoId = item.conceptoId;
    transaccionForm.descripcion = item.descripcion || '';
    transaccionForm.monto = item.monto;
    transaccionForm.estado = item.estado;

    // Formatear fecha para input type="date" (YYYY-MM-DD)
    if (item.fecha) {
      transaccionForm.fecha = item.fecha.split('T')[0];
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function cancelarEdicion() {
    modoEdicion.value = false;
    idEnEdicion.value = null;
    transaccionForm.idEmpleado = 0;
    transaccionForm.tipo = '';
    transaccionForm.conceptoId = 0;
    transaccionForm.descripcion = '';
    transaccionForm.monto = 0;
    transaccionForm.fecha = new Date().toISOString().split('T')[0];
    transaccionForm.estado = 'Pendiente';
  }
</script>

<style scoped>
  .gestion-transacciones {
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
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1rem;
    margin-top: 1rem;
  }

  .form-group-full {
    grid-column: 1 / -1;
  }

  .form-group {
    display: flex;
    flex-direction: column;
  }

    .form-group label {
      margin-bottom: 0.5rem;
      font-weight: 500;
    }

    .form-group input, .form-group select {
      padding: 0.75rem;
      border: 1px solid var(--color-border);
      border-radius: 4px;
      background: var(--color-background);
      color: var(--color-text);
    }

  .button-group {
    display: flex;
    gap: 1rem;
    margin-top: 1rem;
  }

  button {
    padding: 0.75rem 1rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
  }

    button:disabled {
      background-color: #ccc;
      cursor: not-allowed;
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
