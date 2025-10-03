using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Models;
using System;

namespace SistemaDeNominas.Server.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Departamento> Departametos { get; set; }
        public DbSet<Puestos> Puestos { get; set; }

    }
}
