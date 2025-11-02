using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;

namespace SistemaDeNominas.Server.Controllers
{
    // DTO (Data Transfer Object) para recibir la solicitud de cálculo
    public class GenerarNominaRequest
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class CalculoNominaController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CalculoNominaController(AppDbContext db)
        {
            _db = db;
        }

        // --- Endpoint para generar la nómina ---
        [HttpPost("generar")]
        public async Task<IActionResult> GenerarNomina([FromBody] GenerarNominaRequest request)
        {
            if (request.FechaInicio > request.FechaFin)
            {
                return BadRequest("La fecha de inicio no puede ser mayor a la fecha de fin.");
            }

            // Usamos una transacción de base de datos. Si algo falla, todo se revierte.
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear el encabezado de la nómina
                var nomina = new Nomina
                {
                    FechaInicio = request.FechaInicio,
                    FechaFin = request.FechaFin,
                    Estado = "Calculada"
                };
                _db.Nominas.Add(nomina);
                await _db.SaveChangesAsync(); // Guardamos para obtener el ID de la nómina

                // 2. Obtener datos necesarios
                var empleados = await _db.Empleados.ToListAsync();
                var transaccionesPendientes = await _db.Transacciones
                    .Where(t => t.Estado == "Pendiente" && t.Fecha >= request.FechaInicio && t.Fecha <= request.FechaFin)
                    .ToListAsync();

                // Estas son las deducciones automáticas (ej: 2.87% AFP, 3.04% ARS)
                var deduccionesAutomaticas = await _db.TiposDeDeducciones
                    .Where(d => d.Porcentaje != null && d.Porcentaje > 0 && d.Estado == "Activo")
                    .ToListAsync();

                decimal totalNominaNeto = 0;

                // 3. Iterar por cada empleado y calcular
                foreach (var empleado in empleados)
                {
                    decimal salarioBase = empleado.SalarioMensual; // OJO: Asumimos salario mensual. Ajustar si es quincenal.

                    var transaccionesEmpleado = transaccionesPendientes
                        .Where(t => t.idEmpleado == empleado.id)
                        .ToList();

                    // 4. Calcular Ingresos y Deducciones variables
                    decimal ingresosVariables = transaccionesEmpleado
                        .Where(t => t.Tipo == "Ingreso")
                        .Sum(t => t.Monto);

                    decimal deduccionesVariables = transaccionesEmpleado
                        .Where(t => t.Tipo == "Deduccion")
                        .Sum(t => t.Monto);

                    // 5. Calcular Salario Bruto
                    decimal salarioBruto = salarioBase + ingresosVariables;

                    // 6. Calcular Deducciones Automáticas (TSS, etc.)
                    decimal deduccionesAutoCalculadas = 0;
                    foreach (var deduccionAuto in deduccionesAutomaticas)
                    {
                        // Asumimos que el porcentaje es sobre el bruto (ej: 2.87% -> 0.0287)
                        deduccionesAutoCalculadas += salarioBruto * (deduccionAuto.Porcentaje.GetValueOrDefault(0) / 100);
                    }

                    // OJO: Faltaría el cálculo de ISR (Impuesto Sobre la Renta), que es más complejo.
                    // Por ahora, lo dejamos simple.

                    // 7. Calcular Totales
                    decimal totalDeducciones = deduccionesVariables + deduccionesAutoCalculadas;
                    decimal netoAPagar = salarioBruto - totalDeducciones;

                    // 8. Crear el detalle
                    var detalle = new NominaDetalle
                    {
                        idNomina = nomina.Id,
                        idEmpleado = empleado.id,
                        SalarioBase = salarioBase,
                        TotalIngresos = ingresosVariables,
                        TotalDeducciones = totalDeducciones,
                        NetoAPagar = netoAPagar
                    };
                    _db.NominaDetalles.Add(detalle);

                    // 9. Actualizar estado de transacciones
                    foreach (var tx in transaccionesEmpleado)
                    {
                        tx.Estado = "Procesada";
                        _db.Transacciones.Update(tx);
                    }

                    totalNominaNeto += netoAPagar;
                }

                // 10. Actualizar el total en el encabezado
                nomina.TotalCalculado = totalNominaNeto;
                _db.Nominas.Update(nomina);

                // 11. Guardar todos los cambios
                await _db.SaveChangesAsync();
                await transaction.CommitAsync(); // Confirmar la transacción

                return Ok(nomina); // Devolver la nómina generada
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Revertir todo si hay un error
                return StatusCode(500, $"Error interno al generar la nómina: {ex.Message}");
            }
        }

        // --- Endpoints para consultar nóminas generadas ---

        [HttpGet]
        public async Task<IActionResult> GetNominas()
        {
            var nominas = await _db.Nominas
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();
            return Ok(nominas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNominaDetalle(int id)
        {
            var nomina = await _db.Nominas
                .Include(n => n.Detalles) // Cargar los detalles
                    .ThenInclude(d => d.Empleado) // Cargar el empleado de cada detalle
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nomina == null)
            {
                return NotFound("Nómina no encontrada.");
            }

            return Ok(nomina);
        }
    }
}