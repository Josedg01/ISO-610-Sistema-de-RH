using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Puestos
    {
        [Key]

        public int id { get; set; }  

        public string Nombre { get; set; } 

        public int NivelDeRiesgo { get; set; }

        public decimal MinimoSalario { get; set; }

        public decimal MaximoSalario { get; set; }
    }
}
