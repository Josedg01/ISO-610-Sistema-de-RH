using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;

namespace SistemaDeNominas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TiposDeDeduccionesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TiposDeDeduccionesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.TiposDeDeducciones.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var deduccion = await _db.TiposDeDeducciones.FindAsync(id);
            if (deduccion == null) return NotFound("Tipo de Deducción no encontrado.");
            return Ok(deduccion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TipodeDeduccion deduccion)
        {
            _db.TiposDeDeducciones.Add(deduccion);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = deduccion.id }, deduccion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TipodeDeduccion deduccion)
        {
            if (id != deduccion.id)
            {
                return BadRequest("El ID del tipo de deducción no coincide.");
            }

            var d = await _db.TiposDeDeducciones.FindAsync(id);
            if (d == null)
            {
                return NotFound("Tipo de Deducción no encontrado.");
            }

            d.Nombre = deduccion.Nombre;
            d.Descripcion = deduccion.Descripcion;
            d.MontoFijo = deduccion.MontoFijo;
            d.Porcentaje = deduccion.Porcentaje;
            d.Estado = deduccion.Estado;

            _db.TiposDeDeducciones.Update(d);
            await _db.SaveChangesAsync();
            return Ok(d);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.TiposDeDeducciones.FindAsync(id);
            if (d == null)
            {
                return NotFound("Tipo de Deducción no encontrado.");
            }

            _db.TiposDeDeducciones.Remove(d);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}