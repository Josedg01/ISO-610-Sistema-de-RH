using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;
using System;

namespace SistemaDeNominas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmpleadosController : ControllerBase
    {

        private readonly AppDbContext _db;
        public EmpleadosController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Empleados.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _db.Empleados.FindAsync(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] int? idDepartamento, [FromQuery] int? idPuesto)
        {
            var query = _db.Empleados.AsQueryable();

            // Aplicamos filtro de Departamento si se envio
            if (idDepartamento.HasValue)
            {
                query = query.Where(e => e.idDepartamento == idDepartamento.Value);
            }

            // Aplicamos filtro de Puesto si se envio
            if (idPuesto.HasValue)
            {
                query = query.Where(e => e.idPuesto == idPuesto.Value);
            }

            var empleados = await query.ToListAsync();
            return Ok(empleados);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            _db.Empleados.Add(empleado);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = empleado.id }, empleado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Empleado empleado)
        {
            if (id != empleado.id)
            {
                return BadRequest("El ID del empleado no coincide.");
            }

            var e = await _db.Empleados.FindAsync(id);
            if (e == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            // Actualizar propiedades
            e.Nombre = empleado.Nombre;
            e.Cedula = empleado.Cedula;
            e.idDepartamento = empleado.idDepartamento;
            e.idPuesto = empleado.idPuesto;
            e.SalarioMensual = empleado.SalarioMensual;
            e.idNomina = empleado.idNomina;

            _db.Empleados.Update(e);
            await _db.SaveChangesAsync();
            return Ok(e);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _db.Empleados.FindAsync(id);
            if (e == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            _db.Empleados.Remove(e);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
