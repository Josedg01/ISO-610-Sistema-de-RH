<template>
  <div class="calcular-nomina">
    <h2>Proceso de Cálculo de Nómina</h2>

    <div class="card form-card">
      <h3>Generar Nueva Nómina</h3>
      <!-- Se cambia @submit por v-on:submit para evitar errores de parser -->
      <form v-on:submit.prevent="generarNomina" class="form-generar">
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
          <tr v-for="nomina in nominas" :key="nomina.id">
            <td>{{ nomina.id }}</td>
            <td>{{ formatDate(nomina.fechaInicio) }} - {{ formatDate(nomina.fechaFin) }}</td>
            <td>{{ new Date(nomina.fechaCreacion).toLocaleString() }}</td>
            <td>{{ formatCurrency(nomina.totalCalculado) }}</td>
            <td>
              <span :class="{'badge-success': nomina.estado === 'Pagada', 'badge-info': nomina.estado === 'Calculada'}">
                {{ nomina.estado }}
              </span>
            </td>
            <td class="actions-cell">
              <button class="btn-secondary btn-sm" v-on:click="verDetalle(nomina.id)">Detalle</button>
              <!-- BOTÓN NUEVO PARA IR AL REPORTE -->
              <button class="btn-report btn-sm" v-on:click="verReporte(nomina.id)">📄 Reporte</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading.historial && nominas.length === 0">No hay nóminas generadas.</p>
    </div>

    <!-- Sección de Detalle Rápido (Vista Previa) -->
    <div class="card list-card" v-if="nominaSeleccionada">
      <div class="detalle-header">
        <h3>Vista Previa: Detalle Nómina #{{ nominaSeleccionada.id }}</h3>
        <button class="btn-close" v-on:click="nominaSeleccionada = null">X</button>
      </div>
      
      <div v-if="loading.detalle" class="loading">Cargando detalle...</div>
      <table v-if="!loading.detalle && nominaSeleccionada.detalles && nominaSeleccionada.detalles.length > 0">
        <thead>
          <tr>
            <th>Empleado</th>
            <th>Salario Base</th>
            <th>+ Ingresos</th>
            <th>- Deducciones</th>
            <th>= Neto</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="detalle in nominaSeleccionada.detalles" :key="detalle.id">
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
import { useRouter } from 'vue-router'

const router = useRouter();

// --- Interfaces ---
interface Nomina {
  id: number;
  fechaInicio: string;
  fechaFin: string;
  fechaCreacion: string;
  estado: string;
  totalCalculado: number;
  detalles?: NominaDetalle[]; 
}
interface NominaDetalle {
  id: number;
  idNomina: number;
  idEmpleado: number;
  salarioBase: number;
  totalIngresos: number;
  totalDeducciones: number;
  netoAPagar: number;
  empleado?: { nombre: string }; 
}

// --- Estado ---
const nominas = ref<Nomina[]>([]);
const nominaSeleccionada = ref<Nomina | null>(null);

const rangoFechas = ref({
  FechaInicio: '',
  FechaFin: ''
});

const loading = reactive({
  calculando: false,
  historial: true,
  detalle: false
});

// --- Carga ---
onMounted(() => {
  getNominas();
});

async function getNominas() {
  loading.historial = true;
  nominaSeleccionada.value = null;
  try {
    const res = await fetch('/CalculoNomina');
    if (res.ok) {
      nominas.value = await res.json();
    }
  } catch (e) { console.error(e); }
  finally { loading.historial = false; }
}

// --- Helpers ---
function formatCurrency(value: number) {
  if(value === undefined || value === null) return 'RD$ 0.00';
  return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
}
function formatDate(dateString: string) {
  if(!dateString) return '';
  return new Date(dateString).toLocaleDateString();
}

// --- Lógica Principal ---
async function generarNomina() {
  if (!rangoFechas.value.FechaInicio || !rangoFechas.value.FechaFin) {
    alert('Debe seleccionar ambas fechas.');
    return;
  }

  loading.calculando = true;
  nominaSeleccionada.value = null;

  try {
    const response = await fetch('/CalculoNomina/generar', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(rangoFechas.value),
    });

    if (response.ok) {
      const nominaGenerada = await response.json();
      alert(`¡Nómina #${nominaGenerada.id} generada exitosamente!`);
      // Limpiar y recargar
      rangoFechas.value = { FechaInicio: '', FechaFin: '' };
      await getNominas();
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
}

// FUNCIÓN DE NAVEGACIÓN AL REPORTE
function verReporte(idNomina: number) {
  router.push(`/reporte-nomina/${idNomina}`);
}
</script>

<style scoped>
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
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  }

  .form-generar {
    display: flex;
    gap: 1rem;
    align-items: flex-end;
    flex-wrap: wrap;
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
    transition: background 0.3s;
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
    background-color: #f0f0f0;
    color: #333;
  }
  
  .btn-report {
    background-color: #2c3e50;
    color: white;
    margin-left: 0.5rem;
  }
  
  .btn-sm {
    padding: 0.4rem 0.8rem;
    font-size: 0.9rem;
  }

  .btn-close {
    background: transparent;
    color: #999;
    font-size: 1.2rem;
    padding: 0;
  }

  .detalle-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
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

  .actions-cell {
    white-space: nowrap;
  }

  .badge-success { color: green; font-weight: bold; }
  .badge-info { color: #007bff; font-weight: bold; }
</style>