using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;


namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController: Controller
    {

        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServiciosUsuarios servicioUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServiciosUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }


        //listadocuentas
        public async Task<IActionResult> Index()
        {
            //Utilizando el servicio de usuarios para extraer el usuario. con INTERFAZE.
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            var tiposCuentas = await repositorioTiposCuentas.Obtener(id_usuario);
            return View(tiposCuentas);
        }







        public IActionResult Crear()
        {
            //var modelo = new TipoCuenta() { NOMBRE = "Arturo" };
            
            return View();
        }



        [HttpPost]

        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if(!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.ID_USUARIO = servicioUsuarios.ObtenerUsuarioId();




            int yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.NOMBRE, tipoCuenta.ID_USUARIO);

            if(yaExisteTipoCuenta == 1)
            {
                ModelState.AddModelError(nameof(tipoCuenta.NOMBRE),
                    $"El nombre {tipoCuenta.NOMBRE} ya existe ");
                return View(tipoCuenta);
            }

            int validador = await repositorioTiposCuentas.Crear(tipoCuenta);
            
            if(validador == 0)
            {
                //return RedirectToAction("NoEncontrado2", "Home");
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }

            //await repositorioTiposCuentas.Crear(tipoCuenta);

           
           /// return View();
        }

        //debajo de crear asynctask action result
        //ESTE es cuadno se presiona desde otro vinculo un boton por ejemplo y se manda a la pagina con un
        // parametro por ejemplo en este caso
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorID(id, id_usuario);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }
            return View(tipoCuenta);
        }

        // POST EDITAR!
         [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorID(tipoCuenta.ID, id_usuario);

            if(tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Actualizar(tipoCuenta);

            // return View(tipoCuenta);
            return RedirectToAction("Index");
        }
        

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            int id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, id_usuario);

            if(yaExisteTipoCuenta == 1)
            {
                return Json("El nombre "+ nombre +" ya existe ");
                //Redirect("/Index");
            }
            else if (yaExisteTipoCuenta == 0)
            {
                Redirect("/Index");
            }

            return Json(true);
        }





        // BORRAR!!!!!!!!!
        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorID(id, id_usuario);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {

            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorID(id, id_usuario);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        // JSON FETCH API ORDENAR CON JQUERY UI
        [HttpPost]
        public async Task<IActionResult> Ordenar( [FromBody] int[] ids )
        {

            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(id_usuario);
            var idsTiposCuentas = tiposCuentas.Select(x => x.ID);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList(); // LINQ

            if(idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();// FORBID PROHIBIDO
            }
            //LINQ
            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
            new TipoCuenta() { ID = valor, ORDEN = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok(); // RETURN 200 OK.
        }


    }
}
