using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Empleado
    {
        [Key]
        public int identificador {  get; set; }

        public int Cedula   { get; set; }
        public string Nombre { get; set; }

        public string Departamento { get; set; }

        public string Puesto {  get; set; }

        public int Salario { get; set; }

        public int Ident_Nomina { get; set;  }


    }
}
