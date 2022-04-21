using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServiciosUsuarios servicioUsuarios;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IMapper mapper;
        private readonly IServicioReportes serviciosReportes;
        private readonly IRepositorioCuentas repositorioCuentas;


        public TransaccionesController(IRepositorioCuentas repositorioCuentas,
           IRepositorioTransacciones repositorioTransacciones,
           IServiciosUsuarios servicioUsuarios,
           IRepositorioCategorias repositorioCategorias,
           IMapper mapper,
           IServicioReportes serviciosReportes)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCategorias = repositorioCategorias;
            this.mapper = mapper;
            this.serviciosReportes = serviciosReportes;
            this.repositorioCuentas = repositorioCuentas;
            this.serviciosReportes = serviciosReportes;
        }



        public IActionResult Semanal()
        {
            return View();
        }

        public IActionResult Mensual()
        {
            return View();
        }

        public IActionResult Excel()
        {
            return View();
        }

        public IActionResult Calendario()
        {
            return View();
        }


        public async Task<IActionResult> Index(int mes, int año)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            var modelo = await serviciosReportes
                .ObtenerReporteTransaccionesDetalladas(id_usuario,mes, año, ViewBag);

            return View(modelo);
        }


        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.CUENTAS = await ObtenerCuentas(id_usuario);
            modelo.CATEGORIAS = await ObtenerCategorias(id_usuario, modelo.ID_OPERACION);
            return View(modelo);
        }


        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            if(!ModelState.IsValid)
            {
                modelo.CUENTAS = await ObtenerCuentas(id_usuario);
                modelo.CATEGORIAS = await ObtenerCategorias(id_usuario, modelo.ID_OPERACION);
                return View(modelo);

            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.ID_CUENTA, id_usuario);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorID(modelo.ID_CATEGORIA, id_usuario);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }

            modelo.ID_USUARIO = id_usuario;

            if(modelo.ID_OPERACION == TipoOperacion.Gasto)
            {
                modelo.MONTO *= -1;
            }

            await repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");

        }

        public async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int id_usuario)
        {
            var cuentas = await repositorioCuentas.Buscar(id_usuario);
            return cuentas.Select(x=> new SelectListItem(x.NOMBRE, x.ID.ToString() ));
        }

        


        //
        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int id_usuario, 
            TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategorias.Obtener(id_usuario, tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.NOMBRE, x.ID.ToString()));
        }

        //POST FETCH ASYNC JAVASCCRIPT
        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(id_usuario, tipoOperacion);
            return Ok(categorias);
        }





        // METODOS DE ACTUALIZAR

        [HttpGet]
        public async Task<IActionResult> Actualizar(int id, string urlRetorno = null)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorID(id, id_usuario);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncotrado2", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);
            modelo.MontoAnterior = modelo.MONTO;

            if(modelo.ID_OPERACION == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.MONTO * -1;
            }

            modelo.CuentaAnteriorId = transaccion.ID_CUENTA;
            modelo.CATEGORIAS = await ObtenerCategorias(id_usuario, transaccion.ID_OPERACION);
            modelo.CUENTAS = await ObtenerCuentas(id_usuario);
            modelo.urlRetorno = urlRetorno;

            return View(modelo);


        }

        [HttpPost]

        public async Task<IActionResult> Actualizar(TransaccionActualizacionViewModel modelo)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            if(!ModelState.IsValid)
            {
                modelo.CATEGORIAS = await ObtenerCategorias(id_usuario, modelo.ID_OPERACION);
                modelo.CUENTAS = await ObtenerCuentas(id_usuario);
                return View(modelo);
            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.ID_CUENTA, id_usuario);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorID(modelo.ID_CATEGORIA, id_usuario);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }


            var transaccion = mapper.Map<Transaccion>(modelo);
           // modelo.MontoAnterior = modelo.MONTO;

            if(modelo.ID_OPERACION ==  TipoOperacion.Gasto)
            {
                transaccion.MONTO *= -1;
            }

            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            if(string.IsNullOrEmpty(modelo.urlRetorno))
            {
                return RedirectToAction("Index");
            }else
            {
                return LocalRedirect(modelo.urlRetorno);
            }

            
        }



        //ELIMINAR REGISTROS-TRANSACCIONES


        
        [HttpPost]

        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            var transaccion = repositorioTransacciones.ObtenerPorID(id, id_usuario);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }

            await repositorioTransacciones.Borrar(id);



            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }

        }
        

    }
}
