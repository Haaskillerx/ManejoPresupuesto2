using AutoMapper;
using ClosedXML.Excel;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

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



        public async Task<IActionResult> Semanal(int mes, int año)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana =
            await serviciosReportes.ObtenerReporteSemanal(id_usuario, mes, año, ViewBag);

            //

            var agrupado = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x =>
             new ResultadoObtenerPorSemana()
             {
                 Semana = x.Key,
                 //Ingresos
                 Ingresos = x.Where(x=> x.ID_OPERACION == TipoOperacion.Ingreso)
                 .Select(x => x.Monto).FirstOrDefault(),
                 //Gastos
                 Gastos = x.Where(x=> x.ID_OPERACION == TipoOperacion.Gasto)
                 .Select(x=> x.Monto).FirstOrDefault()
             }).ToList();

            //Algoritmo
            if(año == 0 || mes == 0)
            {
                var hoy = DateTime.Today;
                año = hoy.Year;
                mes = hoy.Month;
            }

            var fechaReferencia = new DateTime(año, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);

            var diasSegmentados = diasDelMes.Chunk(7).ToList();

            //Iterar
            for(int i =0; i< diasSegmentados.Count; i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(año, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(año, mes, diasSegmentados[i].Last());
                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                if(grupoSemana is null)
                {
                    agrupado.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        FechaFin = fechaFin,
                        FechaInicio = fechaInicio
                    });
                }else
                {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }
            }

            agrupado = agrupado.OrderByDescending(x=>x.Semana).ToList();

            var modelo = new ReporteSemanalViewModel();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaReferencia = fechaReferencia;
            

            return View(modelo);
        }

        public async Task<IActionResult> Mensual(int año)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            if(año == 0)
            {
                año = DateTime.Today.Year;
            }
            var transaccionesPorMes = await repositorioTransacciones.ObtenerPorMes(id_usuario, año);

            var transaccionesAgrupadas = transaccionesPorMes.GroupBy(x => x.Mes)
                .Select(x => new ResultadoObtenerPorMes()
                {
                    Mes = x.Key,
                    //Ingreso
                    Ingreso = x.Where(x=>x.ID_OPERACION == TipoOperacion.Ingreso)
                    .Select(x=> x.Monto).FirstOrDefault(),
                    //GASTO
                    Gasto = x.Where(x=>x.ID_OPERACION == TipoOperacion.Gasto)
                    .Select(x=>x.Monto).FirstOrDefault()
                }).ToList();

            for(int mes = 1; mes <= 12; mes++)
            {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(año, mes, 1);
                if(transaccion is null)
                {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes()
                    {
                        Mes = mes,
                        FechaReferencia = fechaReferencia
                    });
                }else
                {
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }

            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();

            var modelo = new ReporteMensualViewModel();
            modelo.Año = año;
            modelo.TransaccionesPorMes = transaccionesAgrupadas;
            
            return View(modelo);
        }

        public IActionResult Excel()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcelPorAño(int año)
        {
            var fechaInicio = new DateTime(año, 1, 1);
            var fechaFin = fechaInicio.AddYears(1).AddDays(-1);
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorID_Usuario(
               new ParametroObtenerTransaccionesPorUsuario
               {
                   ID_USUARIO = id_usuario,
                   FechaInicio = fechaInicio,
                   FechaFin = fechaFin
               });

            var nombreArchivo = $"Manejo Presupuesto - {fechaInicio.ToString("yyyy")}.xlsx";
            return GenerarExcel(nombreArchivo, transacciones);

        }

        [HttpGet]
        public async Task<IActionResult> ExportarTodo()
        {
            //100 años atras
            var fechaInicio = DateTime.Today.AddYears(-100);
            var fechaFin = DateTime.Today.AddYears(1000);
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorID_Usuario(
               new ParametroObtenerTransaccionesPorUsuario
               {
                   ID_USUARIO = id_usuario,
                   FechaInicio = fechaInicio,
                   FechaFin = fechaFin
               });

            var nombreArchivo = $"Manejo Presupuesto - {fechaInicio.ToString("dd-MM-yyyy")}.xlsx";
            return GenerarExcel(nombreArchivo, transacciones);

        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelPorMes(int mes, int año)
        {
            var fechaInicio = new DateTime(año, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorID_Usuario(
                new ParametroObtenerTransaccionesPorUsuario
                {
                    ID_USUARIO = id_usuario,
                    FechaFin = fechaFin,
                    FechaInicio=fechaInicio
                });
            var nombreArchivo = $"Manejo Presupuesto - {fechaInicio.ToString("MMM yyyy")}.xlsx";

            return GenerarExcel(nombreArchivo, transacciones);

            
            
        }

        private FileResult GenerarExcel (string nombreArchivo, IEnumerable<Transaccion> transacciones)
        {
            DataTable dt = new DataTable("Transacciones");
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Fecha"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Ingreso/Gasto"),

            });

            foreach(var transa in transacciones)
            {
                dt.Rows.Add(transa.FechaTransaccion,
                    transa.CUENTA,
                    transa.CATEGORIA,
                    transa.NOTA,
                    transa.MONTO,
                    transa.ID_OPERACION);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);

                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    nombreArchivo);
                }
            }
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
