using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int ID { get; set; }
        [Required(ErrorMessage= "El campo {0} es requerido.")]
        [StringLength(maximumLength: 50, ErrorMessage = "No puede ser mayor a {1} caracteres.")]
        public string NOMBRE { get; set; }
        [Display(Name = "Tipo Operacion:")]
        public TipoOperacion ID_OPERACION { get; set; }

        public int ID_USUARIO { get; set; }
    }
}
