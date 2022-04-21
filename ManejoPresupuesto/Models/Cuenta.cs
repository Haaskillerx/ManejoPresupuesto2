using ManejoPresupuesto.Models.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Cuenta
    {

        public int ID { get; set; }

        [Required(ErrorMessage =  "El campo {0} es requerido.")]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMayuscula]

        public string NOMBRE { get; set; }

        [Display(Name = "Tipo Cuenta")]
        public int ID_TIPOCUENTA { get; set; }

        public decimal BALANCE { get; set; }
        [StringLength(maximumLength: 1000)]

        public string DESCRIPCION { get; set; }

        public string TipoCuenta { get; set; }


    }
}
