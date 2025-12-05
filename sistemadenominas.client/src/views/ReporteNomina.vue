<template>
  <div class="reporte-nomina">
    <div class="actions no-print">
      <button @click="goBack" class="btn-secondary">← Volver</button>
      <button @click="printReport" class="btn-primary">Imprimir Reporte</button>
    </div>

    <div v-if="loading" class="loading">Generando reporte...</div>

    <div v-else-if="nomina" class="document-sheet" id="print-area">
      <!-- Encabezado del Reporte -->
      <div class="doc-header">
        <h1>REPORTE DE NÓMINA GENERAL</h1>
        <div class="meta-info">
          <p><strong>Período:</strong> {{ formatDate(nomina.fechaInicio) }} al {{ formatDate(nomina.fechaFin) }}</p>
          <p><strong>Fecha de Emisión:</strong> {{ new Date().toLocaleDateString() }}</p>
          <p><strong>Estado:</strong> {{ nomina.estado }}</p>
          <p><strong>Folio:</strong> #{{ String(nomina.id).padStart(6, '0') }}</p>
        </div>
      </div>

      <hr class="divider">

      <!-- Tabla de Detalle -->
      <table class="doc-table">
        <thead>
          <tr>
            <th>Emp.</th>
            <th>Departamento / Puesto</th>
            <th class="text-right">Salario Base</th>
            <th class="text-right">Ingresos (+)</th>
            <th class="text-right">Deducciones (-)</th>
            <th class="text-right">Neto a Pagar</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="detalle in nomina.detalles" :key="detalle.id">
            <td>
              <strong>{{ detalle.empleado?.nombre }}</strong><br>
              <small class="text-muted">{{ detalle.empleado?.cedula }}</small>
            </td>
            <td>
              {{ getNombreDepartamento(detalle.empleado?.idDepartamento) }}<br>
              <small class="text-muted">{{ getNombrePuesto(detalle.empleado?.idPuesto) }}</small>
            </td>
            <td class="text-right">{{ formatCurrency(detalle.salarioBase) }}</td>
            <td class="text-right text-success">{{ formatCurrency(detalle.totalIngresos) }}</td>
            <td class="text-right text-danger">{{ formatCurrency(detalle.totalDeducciones) }}</td>
            <td class="text-right font-bold">{{ formatCurrency(detalle.netoAPagar) }}</td>
          </tr>
        </tbody>
        <tfoot>
          <tr>
            <td colspan="2" class="text-right"><strong>TOTALES GENERALES:</strong></td>
            <td class="text-right">{{ formatCurrency(totales.base) }}</td>
            <td class="text-right">{{ formatCurrency(totales.ingresos) }}</td>
            <td class="text-right">{{ formatCurrency(totales.deducciones) }}</td>
            <td class="text-right total-final">{{ formatCurrency(totales.neto) }}</td>
          </tr>
        </tfoot>
      </table>

      <!-- Firmas (Footer para impresión) -->
      <div class="firmas">
        <div class="firma-box">
          <hr>
          <p>Elaborado por</p>
        </div>
        <div class="firma-box">
          <hr>
          <p>Revisado por</p>
        </div>
        <div class="firma-box">
          <hr>
          <p>Autorizado por</p>
        </div>
      </div>
    </div>

    <div v-else class="error">No se pudo cargar la información de la nómina.</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const loading = ref(true);
const nomina = ref<any>(null);
const departamentos = ref<any[]>([]);
const puestos = ref<any[]>([]);

// --- Carga de Datos ---
onMounted(async () => {
  const idNomina = route.params.id;
  if (!idNomina) return;

  await Promise.all([
    fetchNomina(idNomina),
    fetchCatalogos()
  ]);
  loading.value = false;
});

async function fetchNomina(id: any) {
  try {
    const res = await fetch(`/CalculoNomina/${id}`);
    if (res.ok) nomina.value = await res.json();
  } catch (e) { console.error(e); }
}

async function fetchCatalogos() {
  try {
    const [resDepto, resPuesto] = await Promise.all([
      fetch('/Departamentos'),
      fetch('/Puestos')
    ]);
    if (resDepto.ok) departamentos.value = await resDepto.json();
    if (resPuesto.ok) puestos.value = await resPuesto.json();
  } catch (e) { console.error(e); }
}

// --- Helpers ---
function getNombreDepartamento(id?: number) {
  return departamentos.value.find(d => d.id === id)?.nombre || 'N/A';
}

function getNombrePuesto(id?: number) {
  return puestos.value.find(p => p.id === id)?.nombre || 'N/A';
}

function formatCurrency(value: number) {
  return (value || 0).toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
}

function formatDate(dateString: string) {
  if (!dateString) return '';
  return new Date(dateString).toLocaleDateString('es-DO', { year: 'numeric', month: 'long', day: 'numeric' });
}

const totales = computed(() => {
  if (!nomina.value || !nomina.value.detalles) return { base: 0, ingresos: 0, deducciones: 0, neto: 0 };
  return nomina.value.detalles.reduce((acc: any, d: any) => ({
    base: acc.base + d.salarioBase,
    ingresos: acc.ingresos + d.totalIngresos,
    deducciones: acc.deducciones + d.totalDeducciones,
    neto: acc.neto + d.netoAPagar
  }), { base: 0, ingresos: 0, deducciones: 0, neto: 0 });
});

// --- Acciones ---
function goBack() {
  router.push('/nomina');
}

function printReport() {
  window.print();
}
</script>

<style scoped>
.reporte-nomina {
  max-width: 1000px;
  margin: 0 auto;
  padding: 1rem;
}

.actions {
  display: flex;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.document-sheet {
  background: white;
  padding: 2rem;
  border: 1px solid #ddd;
  box-shadow: 0 0 15px rgba(0,0,0,0.1);
  min-height: 29.7cm; /* A4 height approx */
}

.doc-header {
  text-align: center;
  margin-bottom: 2rem;
}

.doc-header h1 {
  font-size: 1.8rem;
  margin-bottom: 1rem;
  color: #2c3e50;
  text-transform: uppercase;
}

.meta-info p {
  margin: 0.2rem 0;
  font-size: 0.95rem;
}

.doc-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
  font-size: 0.9rem;
}

.doc-table th, .doc-table td {
  border: 1px solid #eee;
  padding: 0.75rem;
  vertical-align: middle;
}

.doc-table thead th {
  background-color: #f8f9fa;
  font-weight: bold;
  border-bottom: 2px solid #ddd;
}

.text-right { text-align: right; }
.text-muted { color: #666; font-size: 0.85em; }
.text-success { color: #2e7d32; }
.text-danger { color: #c62828; }
.font-bold { font-weight: bold; }

tfoot td {
  border-top: 2px solid #333;
  font-weight: bold;
  background-color: #f8f9fa;
}

.total-final {
  font-size: 1.1em;
  color: #2c3e50;
  border-bottom: 3px double #333;
}

.firmas {
  margin-top: 4rem;
  display: flex;
  justify-content: space-between;
  page-break-inside: avoid;
}

.firma-box {
  width: 30%;
  text-align: center;
}

.firma-box hr {
  border: 0;
  border-top: 1px solid #333;
  margin-bottom: 0.5rem;
}

.btn-primary {
  background-color: hsla(160, 100%, 37%, 1);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
}

/* --- ESTILOS DE IMPRESIÓN --- */
@media print {
  /* Ocultar elementos de navegación y botones */
  .no-print, :global(header), :global(nav) {
    display: none !important;
  }

  /* Ajustar el contenedor para impresión */
  .reporte-nomina {
    margin: 0;
    padding: 0;
    max-width: none;
  }

  .document-sheet {
    border: none;
    box-shadow: none;
    padding: 0;
  }

  body {
    background: white;
    font-size: 12pt;
  }

  /* Asegurar colores de fondo e impresión nítida */
  .doc-table th {
    background-color: #f0f0f0 !important;
    -webkit-print-color-adjust: exact;
  }
}
</style>