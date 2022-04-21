using ManejoPresupuesto.Models.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta // : IValidatableObject
    {
        public int ID { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "La longitud del campo {0} debe estar entre {2} y {1}.")]
        [Display(Name = "Nombre del tipo cuenta:")]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string NOMBRE { get; set; }
        
        public int ID_USUARIO { get; set; }
        public int ORDEN { get; set; }


        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           if (NOMBRE != null && NOMBRE.Length > 0)
            {
                var primeraLetra = NOMBRE[0].ToString();
                if(primeraLetra != primeraLetra.ToUpper() )
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", 
                        new[] { nameof(NOMBRE) } );
                }
            }
        }
        */




        /* Pruebas de otras validaciones por defecto */
        //EMAIL
        /*
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido.")]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        //EDAD
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(minimum: 18, maximum: 130, ErrorMessage = "El valor debe estar entre {1} y {2}." )]
        [Display(Name = "Edad:")]
        public string Edad { get; set; }

        //URL
        [Url(ErrorMessage = "El campo debe ser una URL válida.")]
        [Display(Name = "URL:")]
        public string URL { get; set; }


        //TarjetaDeCredito
        [CreditCard(ErrorMessage ="La tarjeta de crédito no es válida.")]
        [Display(Name = "Tarjeta de Credito:")]
        public string TarjetaDeCredito { get; set; }
        */

    }
}
