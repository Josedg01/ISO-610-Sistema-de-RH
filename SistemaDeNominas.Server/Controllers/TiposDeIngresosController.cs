using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;

namespace SistemaDeNominas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TiposDeIngresosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TiposDeIngresosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Usamos el DbSet "TiposDeIngresos" que definimos en AppDbContext
            return Ok(await _db.TiposDeIngresos.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ingreso = await _db.TiposDeIngresos.FindAsync(id);
            if (ingreso == null) return NotFound("Tipo de Ingreso no encontrado.");
            return Ok(ingreso);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TipodeIngresos ingreso)
        {
            _db.TiposDeIngresos.Add(ingreso);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ingreso.id }, ingreso);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TipodeIngresos ingreso)
        {
            if (id != ingreso.id)
            {
                return BadRequest("El ID del tipo de ingreso no coincide.");
            }

            var i = await _db.TiposDeIngresos.FindAsync(id);
            if (i == null)
            {
                return NotFound("Tipo de Ingreso no encontrado.");
            }

            i.Nombre = ingreso.Nombre;
            i.idEmpleado = ingreso.idEmpleado;
            i.Estado = ingreso.Estado;

            _db.TiposDeIngresos.Update(i);
            await _db.SaveChangesAsync();
            return Ok(i);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var i = await _db.TiposDeIngresos.FindAsync(id);
            if (i == null)
            {
                return NotFound("Tipo de Ingreso no encontrado.");
            }

            _db.TiposDeIngresos.Remove(i);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}