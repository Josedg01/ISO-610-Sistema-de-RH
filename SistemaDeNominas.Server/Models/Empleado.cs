using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Empleado
    {
        [Key]
        public int id {  get; set; }

        public string Cedula   { get; set; }
        public string Nombre { get; set; }

        public int idDepartamento { get; set; }

        public int idPuesto {  get; set; }

        public int SalarioMensual { get; set; }

        public int idNomina { get; set;  }


    }
}
