using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class CuentaCreacionViewModel: Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
        public IEnumerable<SelectListItem> NOMBRE_CUENTA { get; set; }
    }
}
