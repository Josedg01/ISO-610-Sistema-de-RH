using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Puestos
    {
        [Key]

        public int id { get; set; }  

        public string Nombre { get; set; } 

        public int NivelDeRiesgo { get; set; }   

        public float MinimoSalario { get; set; }

        public float MaximoSalario { get; set; }
    }
}
