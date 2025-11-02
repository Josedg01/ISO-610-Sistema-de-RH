using Microsoft.EntityFrameworkCore;
using SistemaDeNominas.Server.Models;

namespace SistemaDeNominas.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Puestos> Puestos { get; set; }
        public DbSet<TipodeIngresos> TiposDeIngresos { get; set; }
        public DbSet<TipodeDeduccion> TiposDeDeducciones { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Nomina> Nominas { get; set; }
        public DbSet<NominaDetalle> NominaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapea las propiedades DbSet a los nombres de tabla SQL correctos
            modelBuilder.Entity<TipodeIngresos>().ToTable("TiposDeIngreso");
            modelBuilder.Entity<TipodeDeduccion>().ToTable("TiposDeDeduccion");
        }
    }
}