using System.ComponentModel.DataAnnotations;

namespace SistemaDeNominas.Server.Models
{
    public class TipodeIngresos
    {
        [Key]
        public int Identificador {  get; set; }
        public string Nombre { get; set; }
        public int Dep_de_Salario {  get; set; }
        public string Estado {  get; set; }
    }
}
