using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TransaccionCreacionViewModel : Transaccion
    {
        public IEnumerable<SelectListItem> CUENTAS { get; set; }
        public IEnumerable<SelectListItem> CATEGORIAS { get; set; }
        /*
        [Display(Name = "Tipo Operacion:")]
        public TipoOperacion ID_OPERACION { get; set; } = TipoOperacion.Ingreso;
        */
    }
}
