using System.Security.Claims;

namespace ManejoPresupuesto.Servicios
{

    public interface IServiciosUsuarios
    {
        int ObtenerUsuarioId();
    }

 
    public class ServicioUsuarios: IServiciosUsuarios
    {
        private  RepositorioUsuarios repositorioUsuarios;
        private HttpContext httpContext;

        //

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor,
            IRepositorioUsuarios repositorioUsuarios)
        {
            this.httpContext = httpContextAccessor.HttpContext;
            this.repositorioUsuarios = (RepositorioUsuarios)repositorioUsuarios;
        }
        public int ObtenerUsuarioId()
        {
            if(httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                int id = 0;

                //Tomar el Correo
                var correo = httpContext.User.FindFirstValue(ClaimTypes.Email);

                var idusuario = repositorioUsuarios.ID_USUARIO(correo);

                //if (int.TryParse(idClaim.Value, out id))
                if(idusuario.Result != 0)
                if(0 != 1)
                {

                    //id = int.Parse(idClaim.Value);
                    id = idusuario.Result;

                }
                else
                {
                    // id = int.Parse(idClaim.Value);
                }
                
                
                return id;
            }else
            {
                throw new ApplicationException("El usuario no esta autenticado.");
            }
          

        }




        //
    }
}
