using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeNominas.Server.Models
{
    public class Nomina
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Calculada";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCalculado { get; set; }

        // Propiedad de navegación
        public virtual ICollection<NominaDetalle> Detalles { get; set; } = new List<NominaDetalle>();
    }
}