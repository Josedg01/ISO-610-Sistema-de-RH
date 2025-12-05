<template>
  <div class="consulta-empleados">
    <h2>Consulta de Empleados por Criterios</h2>

    <div class="card form-card">
      <h3>Filtros de Búsqueda</h3>
      <form @submit.prevent="buscarEmpleados" class="form-filtros">
        
        <div class="form-group">
          <label for="filtroDepto">Departamento:</label>
          <select id="filtroDepto" v-model="filtros.idDepartamento">
            <option :value="null">-- Todos los Departamentos --</option>
            <!-- Ahora usamos .nombre (estándar) -->
            <option v-for="d in departamentos" :key="d.id" :value="d.id">
              {{ d.nombre }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="filtroPuesto">Puesto:</label>
          <select id="filtroPuesto" v-model="filtros.idPuesto">
            <option :value="null">-- Todos los Puestos --</option>
            <!-- Ahora usamos .nombre (estándar) -->
            <option v-for="p in puestos" :key="p.id" :value="p.id">
              {{ p.nombre }}
            </option>
          </select>
        </div>

        <button type="submit" class="btn-primary">
          <span v-if="loading.buscando">Buscando...</span>
          <span v-else>Buscar</span>
        </button>
      </form>
    </div>

    <div class="card list-card">
      <h3>Resultados de la Búsqueda ({{ resultados.length }})</h3>
      
      <div v-if="loading.datos" class="loading">Cargando datos iniciales...</div>
      
      <table v-if="!loading.datos && resultados.length > 0">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Cédula</th>
            <th>Departamento</th>
            <th>Puesto</th>
            <th>Salario</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="emp in resultados" :key="emp.id">
            <!-- Usamos propiedades estándar en minúscula -->
            <td>{{ emp.nombre }}</td>
            <td>{{ emp.cedula }}</td>
            <td>{{ getNombreDepartamento(emp.idDepartamento) }}</td>
            <td>{{ getNombrePuesto(emp.idPuesto) }}</td>
            <td>{{ formatCurrency(emp.salarioMensual) }}</td>
          </tr>
        </tbody>
      </table>
      
      <p v-if="!loading.datos && !loading.buscando && resultados.length === 0" class="no-results">
        No se encontraron empleados con los criterios seleccionados.
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'

// --- Interfaces Estándar (camelCase) ---
// Gracias al cambio en el servidor, ahora siempre recibiremos esto así
interface Empleado {
  id: number;
  cedula: string;
  nombre: string;
  idDepartamento: number;
  idPuesto: number;
  salarioMensual: number;
}

interface Departamento { id: number; nombre: string; }
interface Puesto { id: number; nombre: string; }

// --- Estado ---
const departamentos = ref<Departamento[]>([]);
const puestos = ref<Puesto[]>([]);
const resultados = ref<Empleado[]>([]);

const filtros = reactive({
  idDepartamento: null as number | null,
  idPuesto: null as number | null
});

const loading = reactive({
  datos: true,
  buscando: false
});

// --- Carga Inicial ---
onMounted(async () => {
  await Promise.all([getDepartamentos(), getPuestos()]);
  await buscarEmpleados();
  loading.datos = false;
});

// --- Funciones API ---
async function getDepartamentos() {
  try {
    const res = await fetch('/Departamentos');
    if (res.ok) departamentos.value = await res.json();
  } catch (e) { console.error(e); }
}

async function getPuestos() {
  try {
    const res = await fetch('/Puestos');
    if (res.ok) puestos.value = await res.json();
  } catch (e) { console.error(e); }
}

async function buscarEmpleados() {
  loading.buscando = true;
  resultados.value = [];
  
  const params = new URLSearchParams();
  if (filtros.idDepartamento) params.append('idDepartamento', filtros.idDepartamento.toString());
  if (filtros.idPuesto) params.append('idPuesto', filtros.idPuesto.toString());

  try {
    const res = await fetch(`/Empleados/buscar?${params.toString()}`);
    if (res.ok) {
      resultados.value = await res.json();
      console.log("Datos recibidos:", resultados.value);
    }
  } catch (e) {
    console.error("Error buscando empleados", e);
  } finally {
    loading.buscando = false;
  }
}

// --- Helpers de UI ---
function getNombreDepartamento(id: number) {
  const d = departamentos.value.find(x => x.id === id);
  return d ? d.nombre : 'Desconocido';
}

function getNombrePuesto(id: number) {
  const p = puestos.value.find(x => x.id === id);
  return p ? p.nombre : 'Desconocido';
}

function formatCurrency(value: number) {
  if (value === undefined || value === null) return 'RD$ 0.00';
  return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
}
</script>

<style scoped>
.consulta-empleados {
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

.form-filtros {
  display: flex;
  gap: 1.5rem;
  align-items: flex-end;
  flex-wrap: wrap;
}

.form-group {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-width: 200px;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: var(--color-text-light-2);
}

.form-group select {
  padding: 0.75rem;
  border: 1px solid var(--color-border);
  border-radius: 4px;
  background: var(--color-background);
  color: var(--color-text);
}

button {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  height: 46px; 
}

.btn-primary {
  background-color: hsla(160, 100%, 37%, 1);
  color: white;
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

.no-results {
  margin-top: 1rem;
  color: var(--color-text-light-2);
  font-style: italic;
}
</style>