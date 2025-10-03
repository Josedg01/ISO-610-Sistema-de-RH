using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Departamento
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }

        public string UbicacionFisica { get; set; }

        public int idResponsableArea { get; set; }  
    }
}
