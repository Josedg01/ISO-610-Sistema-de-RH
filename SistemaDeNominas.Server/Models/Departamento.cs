using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Departamento
    {
        [Key]
        public int Identificador { get; set; }
        public string Nombre { get; set; }

        public string Ubi_Fisica { get; set; }

        public string Resp_de_Area { get; set; }  
    }
}
