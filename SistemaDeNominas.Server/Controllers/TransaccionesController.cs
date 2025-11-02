using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;

namespace SistemaDeNominas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TransaccionesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Ordenamos por fecha descendente para ver las más nuevas primero
            return Ok(await _db.Transacciones.OrderByDescending(t => t.Fecha).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tx = await _db.Transacciones.FindAsync(id);
            if (tx == null) return NotFound("Transacción no encontrada.");
            return Ok(tx);
        }

        // --- Endpoint Especial ---
        [HttpGet("PorEmpleado/{idEmpleado}")]
        public async Task<IActionResult> GetPorEmpleado(int idEmpleado)
        {
            var txs = await _db.Transacciones
                .Where(t => t.idEmpleado == idEmpleado && t.Estado == "Pendiente")
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();

            return Ok(txs);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Transaccion tx)
        {
            if (tx.Fecha == DateTime.MinValue)
            {
                tx.Fecha = DateTime.Today; // Asignar fecha actual si no se provee
            }
            tx.Estado = "Pendiente"; // Forzar estado pendiente al crear

            _db.Transacciones.Add(tx);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = tx.id }, tx);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Transaccion tx)
        {
            if (id != tx.id)
            {
                return BadRequest("El ID de la transacción no coincide.");
            }

            var t = await _db.Transacciones.FindAsync(id);
            if (t == null)
            {
                return NotFound("Transacción no encontrada.");
            }

            t.idEmpleado = tx.idEmpleado;
            t.Tipo = tx.Tipo;
            t.ConceptoId = tx.ConceptoId;
            t.Descripcion = tx.Descripcion;
            t.Monto = tx.Monto;
            t.Fecha = tx.Fecha;
            t.Estado = tx.Estado; // Permitir cambiar estado (ej: a "Procesada")

            _db.Transacciones.Update(t);
            await _db.SaveChangesAsync();
            return Ok(t);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.Transacciones.FindAsync(id);
            if (t == null)
            {
                return NotFound("Transacción no encontrada.");
            }

            // Regla de negocio: No borrar transacciones ya procesadas
            if (t.Estado == "Procesada")
            {
                return BadRequest("No se pueden eliminar transacciones que ya han sido procesadas en una nómina.");
            }

            _db.Transacciones.Remove(t);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}