<template>
  <div class="gestion-transacciones">
    <h2>Registro de Transacciones (Ingresos/Deducciones)</h2>

    <div class="card form-card">
      <h3>Registrar Nueva Transacción</h3>
      <form @submit.prevent="crearTransaccion">

        <div class="form-group">
          <label for="empleado">Empleado:</label>
          <select id="empleado" v-model="nuevaTx.idEmpleado" required>
            <option disabled value="">Seleccione un empleado</option>
            <option v-for="emp in empleados" :key="emp.id" :value="emp.id">
              {{ emp.Nombre }} ({{ emp.Cedula }})
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="tipo">Tipo de Transacción:</label>
          <select id="tipo" v-model="nuevaTx.Tipo" required @change="limpiarConcepto">
            <option disabled value="">Seleccione el tipo</option>
            <option value="Ingreso">Ingreso</option>
            <option value="Deduccion">Deducción</option>
          </select>
        </div>

        <div class="form-group" v-if="nuevaTx.Tipo === 'Ingreso'">
          <label for="conceptoIngreso">Concepto (Ingreso):</label>
          <select id="conceptoIngreso" v-model="nuevaTx.ConceptoId" required>
            <option disabled value="">Seleccione un ingreso</option>
            <option v-for="ing in tiposIngresos" :key="ing.id" :value="ing.id">
              {{ ing.Nombre }}
            </option>
          </select>
        </div>

        <div class="form-group" v-if="nuevaTx.Tipo === 'Deduccion'">
          <label for="conceptoDeduccion">Concepto (Deducción):</label>
          <select id="conceptoDeduccion" v-model="nuevaTx.ConceptoId" required>
            <option disabled value="">Seleccione una deducción</option>
            <option v-for="ded in tiposDeducciones" :key="ded.id" :value="ded.id">
              {{ ded.Nombre }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="monto">Monto (RD$):</label>
          <input type="number" step="0.01" id="monto" v-model.number="nuevaTx.Monto" required>
        </div>

        <div class="form-group">
          <label for="fecha">Fecha:</label>
          <input type="date" id="fecha" v-model="nuevaTx.Fecha" required>
        </div>

        <div class="form-group form-group-full">
          <label for="descripcion">Descripción (Opcional):</label>
          <input type="text" id="descripcion" v-model="nuevaTx.Descripcion">
        </div>

        <button typeS="submit" class="btn-primary" :disabled="!nuevaTx.Tipo || !nuevaTx.ConceptoId">
          Guardar Transacción
        </button>
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
            <td>{{ new Date(tx.Fecha).toLocaleDateString() }}</td>
            <td>{{ getNombreEmpleado(tx.idEmpleado) }}</td>
            <td>{{ tx.Tipo }}</td>
            <td>{{ tx.Descripcion || 'N/A' }}</td>
            <td>{{ formatCurrency(tx.Monto) }}</td>
            <td>{{ tx.Estado }}</td>
            <td>
              <button class="btn-danger" @click="eliminarTransaccion(tx.id)">Eliminar</button>
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
  Tipo: string;
  ConceptoId: number;
  Descripcion?: string;
  Monto: number;
  Fecha: string; // Usamos string para el input date
  Estado: string;
}
interface Empleado {
  id: number;
  Nombre: string;
  Cedula: string;
}
interface Concepto {
  id: number;
  Nombre: string;
}

// --- Estado (Reactivity) ---
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

const nuevaTx = ref<Omit<Transaccion, 'id' | 'Estado'>>({
  idEmpleado: 0,
  Tipo: '',
  ConceptoId: 0,
  Descripcion: '',
  Monto: 0,
  Fecha: new Date().toISOString().split('T')[0], // Fecha de hoy por defecto
});

// --- Funciones de Carga (onMounted) ---
onMounted(async () => {
  await Promise.all([
    getEmpleados(),
    getTiposIngresos(),
    getTiposDeducciones(),
    getTransacciones()
  ]);
});

async function getEmpleados() {
  loading.empleados = true;
  try {
    const res = await fetch('/Empleados');
    if (res.ok) empleados.value = await res.json();
  } catch (e) { console.error(e); }
  finally { loading.empleados = false; }
}

async function getTiposIngresos() {
  loading.ingresos = true;
  try {
    const res = await fetch('/TiposDeIngresos');
    if (res.ok) tiposIngresos.value = await res.json();
  } catch (e) { console.error(e); }
  finally { loading.ingresos = false; }
}

async function getTiposDeducciones() {
  loading.deducciones = true;
  try {
    const res = await fetch('/TiposDeDeducciones');
    if (res.ok) tiposDeducciones.value = await res.json();
  } catch (e) { console.error(e); }
  finally { loading.deducciones = false; }
}

async function getTransacciones() {
  loading.txs = true;
  try {
    // Solo cargamos las pendientes para que la lista sea manejable
    const res = await fetch('/Transacciones');
    if (res.ok) {
      transacciones.value = (await res.json()).filter((t: Transaccion) => t.Estado === 'Pendiente');
    }
  } catch (e) { console.error(e); }
  finally { loading.txs = false; }
}

// --- Funciones de UI ---

function limpiarConcepto() {
  nuevaTx.value.ConceptoId = 0;
}

function getNombreEmpleado(id: number): string {
  const emp = empleados.value.find(e => e.id === id);
  return emp ? emp.Nombre : `ID: ${id}`;
}

function formatCurrency(value: number) {
  return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
}

function resetForm() {
   nuevaTx.value = {
    idEmpleado: 0,
    Tipo: '',
    ConceptoId: 0,
    Descripcion: '',
    Monto: 0,
    Fecha: new Date().toISOString().split('T')[0],
  };
}

// --- Funciones CRUD ---

async function crearTransaccion() {
  try {
    const response = await fetch('/Transacciones', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(nuevaTx.value),
    });

    if (response.ok) {
      resetForm();
      await getTransacciones(); // Recargar la lista
    } else {
      console.error('Error al crear transacción:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  }
}

async function eliminarTransaccion(id: number) {
  if (!confirm('¿Está seguro de que desea eliminar esta transacción?')) {
    return;
  }

  try {
    const response = await fetch(`/Transacciones/${id}`, {
      method: 'DELETE',
    });

    if (response.ok) {
      await getTransacciones(); // Recargar la lista
    } else {
       const error = await response.text();
      console.error('Error al eliminar transacción:', error);
      alert(`Error: ${error}`);
    }
  } catch (error) {
    console.error('Error de red:', error);
  }
}</script>

<style scoped>
  /* Estilos consistentes */
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
    grid-column: 1 / -1; /* Ocupa todo el ancho */
  }

  .form-group {
    display: flex;
    flex-direction: column;
  }

  .form-group label {
    margin-bottom: 0.5rem;
    font-weight: 500;
  }

  .form-group input,
  .form-group select {
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

    button:disabled {
      background-color: #ccc;
      cursor: not-allowed;
    }

  .btn-primary {
    background-color: hsla(160, 100%, 37%, 1);
    color: white;
    grid-column: 1 / 1;
    margin-top: 1rem;
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
  }
</style>
