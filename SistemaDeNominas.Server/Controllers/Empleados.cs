using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Data;
using SistemaDeNominas.Server.Models;
using System;

namespace SistemaDeNominas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Empleados : ControllerBase
    {

        private readonly AppDbContext _db;
        public Empleados(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Empleados.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _db.Empleados.FindAsync(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            var verificar = await _db.Empleados.FirstOrDefaultAsync(e => e.Cedula == empleado.Cedula);

            if (verificar != null)
            {
                return Conflict("Ya el usuario existe");
            }

            _db.Empleados.Add(empleado);
            await _db.SaveChangesAsync();
            // EF ya actualizó empleado.Identificador con el valor generado en la DB
            return CreatedAtAction(nameof(Get), new { id = empleado.id }, empleado);


        }

       













    }
}
