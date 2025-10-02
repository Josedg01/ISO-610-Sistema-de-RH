using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class Puestos
    {
        [Key]

        public int Identificador { get; set; }  

        public int Nombre { get; set; } 

        public int Nivel_Riesgo { get; set; }   

        public float Niv_Mn_Salario     { get; set; }

        public float Niv_Mx_Salario { get; set; }
    }
}
