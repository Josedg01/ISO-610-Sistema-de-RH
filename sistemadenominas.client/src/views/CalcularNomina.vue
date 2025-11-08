<template>
  <div class="calcular-nomina">
    <h2>Proceso de Cálculo de Nómina</h2>

    <div classs="card form-card">
      <h3>Generar Nueva Nómina</h3>
      <form @submit.prevent="generarNomina" class="form-generar">
        <div class="form-group">
          <label for="fechaInicio">Fecha de Inicio:</label>
          <input type="date" id="fechaInicio" v-model="rangoFechas.FechaInicio" required>
        </div>
        <div class="form-group">
          <label for="fechaFin">Fecha de Fin:</label>
          <input type="date" id="fechaFin" v-model="rangoFechas.FechaFin" required>
        </div>
        <button type="submit" class="btn-primary" :disabled="loading.calculando">
          {{ loading.calculando ? 'Calculando...' : 'Generar Nómina' }}
        </button>
      </form>
    </div>

    <div class="card list-card">
      <h3>Historial de Nóminas Generadas</h3>
      <div v-if="loading.historial" class="loading">Cargando historial...</div>
      <table v-if="!loading.historial && nominas.length > 0">
        <thead>
          <tr>
            <th>ID</th>
            <th>Período</th>
            <th>Fecha Creación</th>
            <th>Total Calculado</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="nomina in nominas" :key="nomina.Id">
            <td>{{ nomina.id }}</td>
            <td>{{ formatDate(nomina.fechaInicio) }} - {{ formatDate(nomina.fechaFin) }}</td>
            <td>{{ new Date(nomina.fechaCreacion).toLocaleString() }}</td>
            <td>{{ formatCurrency(nomina.totalCalculado) }}</td>
            <td>{{ nomina.estado }}</td>
            <td>
              <button class="btn-secondary" @click="verDetalle(nomina.Id)">Ver Detalle</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading.historial && nominas.length === 0">No hay nóminas generadas.</p>
    </div>

    <div class="card list-card" v-if="nominaSeleccionada">
      <h3>Detalle de Nómina #{{ nominaSeleccionada.id }}</h3>
      <div v-if="loading.detalle" class="loading">Cargando detalle...</div>
      <table v-if="!loading.detalle && nominaSeleccionada.Detalles.length > 0">
        <thead>
          <tr>
            <th>ID Empleado</th>
            <th>Nombre</th>
            <th>Salario Base</th>
            <th>+ Ingresos Var.</th>
            <th>- Deducciones Var.</th>
            <th>= Neto a Pagar</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="detalle in nominaSeleccionada.Detalles" :key="detalle.Id">
            <td>{{ detalle.idEmpleado }}</td>
            <td>{{ detalle.empleado ? detalle.empleado.nombre : 'N/A' }}</td>
            <td>{{ formatCurrency(detalle.salarioBase) }}</td>
            <td>{{ formatCurrency(detalle.totalIngresos) }}</td>
            <td>{{ formatCurrency(detalle.totalDeducciones) }}</td>
            <td><strong>{{ formatCurrency(detalle.netoAPagar) }}</strong></td>
          </tr>
        </tbody>
      </table>
    </div>

  </div>
</template>

<script setup lang="ts">

import { ref, onMounted, reactive } from 'vue'

// --- Interfaces ---
interface Nomina {
  Id: number;
  FechaInicio: string;
  FechaFin: string;
  FechaCreacion: string;
  Estado: string;
  TotalCalculado: number;
  Detalles?: NominaDetalle[]; // Opcional
}
interface NominaDetalle {
  Id: number;
  idNomina: number;
  idEmpleado: number;
  SalarioBase: number;
  TotalIngresos: number;
  TotalDeducciones: number;
  NetoAPagar: number;
  Empleado?: { Nombre: string }; // Incluimos el nombre
}
interface NominaCompleta extends Nomina {
  Detalles: NominaDetalle[];
}

// --- Estado (Reactivity) ---
const nominas = ref<Nomina[]>([]); // Historial
const nominaSeleccionada = ref<NominaCompleta | null>(null);

const rangoFechas = ref({
  FechaInicio: '',
  FechaFin: ''
});

const loading = reactive({
  calculando: false,
  historial: true,
  detalle: false
});

// --- Funciones de Carga ---
onMounted(() => {
  getNominas();
});

async function getNominas() {
  loading.historial = true;
  nominaSeleccionada.value = null; // Limpiar detalle al recargar historial
  try {
    const res = await fetch('/CalculoNomina');
    if (res.ok) {
      nominas.value = await res.json();
    }
  } catch (e) { console.error(e); }
  finally { loading.historial = false; }
}

// --- Funciones de UI (Helpers) ---
function formatCurrency(value: number) {
  return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
}
function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString();
}

// --- Funciones de Proceso ---
async function generarNomina() {
  if (!rangoFechas.value.FechaInicio || !rangoFechas.value.FechaFin) {
    alert('Debe seleccionar ambas fechas.');
    return;
  }

  loading.calculando = true;
  nominaSeleccionada.value = null; // Ocultar detalle anterior

  try {
    const response = await fetch('/CalculoNomina/generar', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(rangoFechas.value),
    });

    if (response.ok) {
      const nominaGenerada = await response.json();
      alert(`¡Nómina #${nominaGenerada.Id} generada exitosamente!`);
      rangoFechas.value = { FechaInicio: '', FechaFin: '' }; // Limpiar formulario
      await getNominas(); // Recargar el historial
      await verDetalle(nominaGenerada.Id); // Mostrar el detalle de la nueva nómina
    } else {
      const error = await response.text();
      alert(`Error al generar nómina: ${error}`);
    }
  } catch (error) {
    console.error('Error de red:', error);
  } finally {
    loading.calculando = false;
  }
}

async function verDetalle(idNomina: number) {
  loading.detalle = true;
  nominaSeleccionada.value = null;
  try {
    const res = await fetch(`/CalculoNomina/${idNomina}`);
    if (res.ok) {
      nominaSeleccionada.value = await res.json();
    } else {
      alert('Error al cargar el detalle.');
    }
  } catch (e) {
    console.error(e);
  } finally {
    loading.detalle = false;
  }
}</script>

<style scoped>
  /* Estilos consistentes */
  .calcular-nomina {
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

  .form-generar {
    display: flex;
    gap: 1rem;
    align-items: flex-end;
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

  button {
    padding: 0.75rem 1rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
  }

  button:disabled {
    background-color: #ccc;
  }

  .btn-primary {
    background-color: hsla(160, 100%, 37%, 1);
    color: white;
  }

  .btn-secondary {
    background-color: #f0f0f0;
    color: #333;
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
