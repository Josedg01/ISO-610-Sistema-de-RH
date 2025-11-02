using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeNominas.Server.Models
{
    public class NominaDetalle
    {
        [Key]
        public int Id { get; set; }

        public int idNomina { get; set; }
        public int idEmpleado { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SalarioBase { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalIngresos { get; set; } // Ingresos variables (bonos, comisiones)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalDeducciones { get; set; } // Deducciones (préstamos, AFP, ARS)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetoAPagar { get; set; }

        // Propiedades de navegación
        [ForeignKey("idNomina")]
        public virtual Nomina? Nomina { get; set; }

        [ForeignKey("idEmpleado")]
        public virtual Empleado? Empleado { get; set; }
    }
}