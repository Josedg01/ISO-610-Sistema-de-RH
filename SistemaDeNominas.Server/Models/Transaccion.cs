using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Transaccion
    {
        [Key]
        public int id { get; set; }

        public int idEmpleado { get; set; }

        [Required]
        public string Tipo { get; set; } // "Ingreso" o "Deduccion"

        public int ConceptoId { get; set; } // ID de TiposDeIngreso o TiposDeDeduccion

        public string? Descripcion { get; set; }

        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        public string Estado { get; set; } // "Pendiente" o "Procesada"
    }
}