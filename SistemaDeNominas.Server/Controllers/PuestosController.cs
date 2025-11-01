using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;

namespace SistemaDeNominas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PuestosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PuestosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.Puestos.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var puesto = await _db.Puestos.FindAsync(id);
            if (puesto == null) return NotFound("Puesto no encontrado.");
            return Ok(puesto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Puestos puesto)
        {
            _db.Puestos.Add(puesto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = puesto.id }, puesto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Puestos puesto)
        {
            if (id != puesto.id)
            {
                return BadRequest("El ID del puesto no coincide.");
            }

            var p = await _db.Puestos.FindAsync(id);
            if (p == null)
            {
                return NotFound("Puesto no encontrado.");
            }

            p.Nombre = puesto.Nombre;
            p.NivelDeRiesgo = puesto.NivelDeRiesgo;
            p.MinimoSalario = puesto.MinimoSalario;
            p.MaximoSalario = puesto.MaximoSalario;

            _db.Puestos.Update(p);
            await _db.SaveChangesAsync();
            return Ok(p);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Puestos.FindAsync(id);
            if (p == null)
            {
                return NotFound("Puesto no encontrado.");
            }

            _db.Puestos.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}