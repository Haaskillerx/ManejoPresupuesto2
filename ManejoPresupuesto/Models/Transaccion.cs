using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {
        public int ID { get; set; }
        public int ID_USUARIO { get; set; }
        [Display(Name = "Fecha Transacción")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
        [Display(Name = "Monto:")]
        public decimal MONTO { get; set; }
        [Display(Name = "Categoria:")]
        [Range(1, maximum:int.MaxValue, ErrorMessage="Debe seleccionar una categoría")]
        public int ID_CATEGORIA { get; set; }
        [Display(Name = "Nota:")]
        [StringLength(maximumLength: 1000, ErrorMessage= "La nota no puede pasar de {1} caracteres.")]
        public string NOTA { get; set; }
        [Display(Name = "Cuenta:")]
        public int ID_CUENTA { get; set; }



        [Display(Name = "Tipo Operacion:")]
        public TipoOperacion ID_OPERACION { get; set; } = TipoOperacion.Ingreso;


        public string CUENTA { get; set; }
        public string CATEGORIA { get; set; }
    }
}
