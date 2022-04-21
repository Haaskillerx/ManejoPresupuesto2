using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private IServiciosUsuarios servicioUsuarios;
        private IRepositorioTiposCuentas repositorioTiposCuentas;
        private IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;
        private IRepositorioTransacciones repositorioTransacciones;
        private IServicioReportes servicioReportes;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServiciosUsuarios servicioUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IMapper mapper,
            IRepositorioTransacciones repositorioTransacciones,
            IServicioReportes servicioReportes)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
        }


        //INDEX
        public async Task<IActionResult> Index()
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await repositorioCuentas.Buscar(id_usuario);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()

                }).ToList();

            return View(modelo);
        }









        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            //var tiposCuentas = await repositorioTiposCuentas.Obtener(id_usuario);
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(id_usuario);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = repositorioTiposCuentas.ObtenerPorID(cuenta.ID, id_usuario);
            if (tipoCuenta is null)
            {
                //Vista - Controlador
                return RedirectToAction("NoEncotrado2", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(id_usuario);
                return View(cuenta);
            }

            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");

        }

        //Encapsular metodo LIN-Q
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int id_usuario)
        {
            var tiposCuentas = await repositorioTiposCuentas.Obtener(id_usuario);
            return tiposCuentas.Select(x => new SelectListItem(x.NOMBRE, x.ID.ToString()));
        }





        //EDITAR BORRAR
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, id_usuario);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }


            //MAPEO MANUAL
            /*
            var modelo = new CuentaCreacionViewModel()
            {
                ID = cuenta.ID,
                NOMBRE = cuenta.NOMBRE,
                ID_TIPOCUENTA = cuenta.ID_TIPOCUENTA,
                DESCRIPCION = cuenta.DESCRIPCION,
                BALANCE = cuenta.BALANCE

            };
            */

            //MAPEO AUTOMAPPER
            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(id_usuario);
            return View(modelo);


        }


        //EDITAR POST
        [HttpPost]

        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.ID, id_usuario);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }


            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorID(cuentaEditar.ID_TIPOCUENTA,
                id_usuario);


            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }


            await repositorioCuentas.Actualizar(cuentaEditar);

            return RedirectToAction("Index");

        }


        //ELIMINAR

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, id_usuario);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }


            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, id_usuario);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }
            await repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");
        }

        //END












        public async Task<IActionResult> Detalle(int id, int mes, int año)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, id_usuario);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }

            ViewBag.Cuenta = cuenta.NOMBRE;

            var modelo = await servicioReportes.
                ObtenerReporteTransaccionesDetalladasPorCuenta(id_usuario, id, mes, año, ViewBag);


            return View(modelo);
        }

















    }
}
