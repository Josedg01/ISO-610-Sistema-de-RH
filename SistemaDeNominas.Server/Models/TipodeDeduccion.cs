using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class TipodeDeduccion
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        // Un monto fijo (ej: $500 de ARS)
        public decimal? MontoFijo { get; set; }

        // Un porcentaje (ej: 2.87% de AFP)
        public decimal? Porcentaje { get; set; }

        [Required]
        public string Estado { get; set; }
    }
}