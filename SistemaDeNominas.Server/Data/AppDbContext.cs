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
        public DbSet<TipodeIngresos> TiposDeIngresos { get; set; }
        public DbSet<TipodeDeduccion> TiposDeDeducciones { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Nomina> Nominas { get; set; }
        public DbSet<NominaDetalle> NominaDetalles { get; set; }

    }
}
