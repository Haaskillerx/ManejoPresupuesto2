namespace ManejoPresupuesto.Models
{
    public class Usuario
    {
        public int ID {get;set;}
        public string EMAIL { get;set;}
        public string EMAIL_NORMALIZADO { get;set;}
        public string PASSWORD { get;set;}

        public int STATUS { get;set;}
    }
}
