using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;

namespace SistemaDeNominas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartamentosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DepartamentosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Usamos "Departamentos" como se definió en AppDbContext
            return Ok(await _db.Departamentos.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var depto = await _db.Departamentos.FindAsync(id);
            if (depto == null) return NotFound("Departamento no encontrado.");
            return Ok(depto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Departamento depto)
        {
            _db.Departamentos.Add(depto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = depto.id }, depto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Departamento depto)
        {
            if (id != depto.id)
            {
                return BadRequest("El ID del departamento no coincide.");
            }

            var d = await _db.Departamentos.FindAsync(id);
            if (d == null)
            {
                return NotFound("Departamento no encontrado.");
            }

            d.Nombre = depto.Nombre;
            d.UbicacionFisica = depto.UbicacionFisica;
            d.idResponsableArea = depto.idResponsableArea;

            _db.Departamentos.Update(d);
            await _db.SaveChangesAsync();
            return Ok(d);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Departamentos.FindAsync(id);
            if (d == null)
            {
                return NotFound("Departamento no encontrado.");
            }

            _db.Departamentos.Remove(d);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}