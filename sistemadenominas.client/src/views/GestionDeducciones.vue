<template>
  <div class="gestion-deducciones">
    <h2>Gestión de Tipos de Deducciones</h2>

    <div class="card form-card">
      <h3>Registrar Nueva Deducción</h3>
      <form @submit.prevent="crearDeduccion">
        <div class="form-group">
          <label for="nombre">Nombre (Ej: AFP, ARS):</label>
          <input type="text" id="nombre" v-model="nuevaDeduccion.Nombre" required>
        </div>
        <div class="form-group">
          <label for="descripcion">Descripción:</label>
          <input type="text" id="descripcion" v-model="nuevaDeduccion.Descripcion">
        </div>
        <div class="form-group">
          <label for="montoFijo">Monto Fijo (RD$):</label>
          <input type="number" step="0.01" id="montoFijo" v-model.number="nuevaDeduccion.MontoFijo">
        </div>
        <div class="form-group">
          <label for="porcentaje">Porcentaje (%):</label>
          <input type="number" step="0.01" id="porcentaje" v-model.number="nuevaDeduccion.Porcentaje">
        </div>
         <div class="form-group">
          <label for="estado">Estado:</label>
          <input type="text" id="estado" v-model="nuevaDeduccion.Estado" required>
        </div>
        <button type="submit" class="btn-primary">Guardar Deducción</button>
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
            <td>{{ deduccion.Nombre }}</td>
            <td>{{ formatCurrency(deduccion.MontoFijo) }}</td>
            <td>{{ deduccion.Porcentaje ? deduccion.Porcentaje + '%' : 'N/A' }}</td>
            <td>{{ deduccion.Estado }}</td>
            <td>
              <button class="btn-secondary">Editar</button>
              <button class="btn-danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && deducciones.length === 0">No hay deducciones registradas.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Interface basada en el modelo TipodeDeduccion.cs
interface TipodeDeduccion {
  id: number;
  Nombre: string;
  Descripcion?: string;
  MontoFijo?: number;
  Porcentaje?: number;
  Estado: string;
}

const deducciones = ref<TipodeDeduccion[]>([]);
const loading = ref(true);
const nuevaDeduccion = ref<Omit<TipodeDeduccion, 'id'>>({
  Nombre: '',
  Descripcion: '',
  MontoFijo: undefined,
  Porcentaje: undefined,
  Estado: 'Activo', // Valor por defecto
});

// Helper para formatear moneda
function formatCurrency(value?: number) {
  if (value === null || value === undefined) return 'N/A';
  return value.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' });
}

async function getDeducciones() {
  loading.value = true;
  try {
    const response = await fetch('/TiposDeDeducciones'); // Llama al nuevo API
    if (response.ok) {
      deducciones.value = await response.json();
    } else {
      console.error('Error al cargar deducciones:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  } finally {
    loading.value = false;
  }
}

async function crearDeduccion() {
  try {
    const payload = {
      ...nuevaDeduccion.value,
      MontoFijo: nuevaDeduccion.value.MontoFijo || null,
      Porcentaje: nuevaDeduccion.value.Porcentaje || null,
    };
    
    const response = await fetch('/TiposDeDeducciones', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (response.ok) {
      // Limpiar formulario
      nuevaDeduccion.value = {
        Nombre: '',
        Descripcion: '',
        MontoFijo: undefined,
        Porcentaje: undefined,
        Estado: 'Activo',
      };
      // Recargar la lista
      await getDeducciones();
    } else {
      console.error('Error al crear deducción:', response.statusText);
    }
  } catch (error) {
    console.error('Error de red:', error);
  }
}

// Cargar los datos cuando el componente se monte
onMounted(() => {
  getDeducciones();
});
</script>

<style scoped>
  /* Estilos (copiados de tus otros componentes de gestión) */
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
