using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public interface IServicioReportes
    {
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int id_usuario, int mes, int anio, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladasPorCuenta(int id_usuario, int id_cuenta, int mes, int anio, dynamic ViewBag);
    }

    public class ServiciosReportes: IServicioReportes
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IHttpContextAccessor httpContext;

        //Inyeccion de dependencias
        //Inyeccion de dependencias
        //Inyeccion de dependencias
        public ServiciosReportes(IRepositorioTransacciones repositorioTransacciones,
            IHttpContextAccessor httpContextAccesor)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.httpContext = httpContextAccesor;
        }
        //Inyeccion de dependencias
        //Inyeccion de dependencias
        //Inyeccion de dependencias


        // Reporte utilizando CUENTAS CONTROLLER //
        // Reporte utilizando CUENTAS CONTROLLER //
        // Reporte utilizando CUENTAS CONTROLLER //
        public async Task<ReporteTransaccionesDetalladas>
            ObtenerReporteTransaccionesDetalladasPorCuenta(int id_usuario, int id_cuenta, int mes, 
            int anio, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechas(mes, anio);



            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                ID_USUARIO = id_usuario,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };
            var transacciones = await repositorioTransacciones.ObtenerPorID_Usuario(parametro);
            ReporteTransaccionesDetalladas modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);

            //VIEWBAGS
            AsignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        private static ReporteTransaccionesDetalladas GenerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones)
        {
            //MODELO
            var modelo = new ReporteTransaccionesDetalladas();


            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;
            return modelo;
        }

        // Reporte utilizando CUENTAS CONTROLLER //
        // Reporte utilizando CUENTAS CONTROLLER //
        // Reporte utilizando CUENTAS CONTROLLER //

        //****************************************************************************************/
        /********************* SEPARADOR *********************************************************/
        //****************************************************************************************/

        /******************************** GENERAR FECHAS *********************/
        /******************************** GENERAR FECHAS *********************/
        /******************************** GENERAR FECHAS *********************/
        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechas(int mes, int año)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || año <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(año, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            // return fechaFin;
            return (fechaInicio, fechaFin);
        }
        /******************************** GENERAR FECHAS *********************/
        /******************************** GENERAR FECHAS *********************/
        /******************************** GENERAR FECHAS *********************/


        //****************************************************************************************/
        //***** REPORTE TRANSACCIONES DETALLADAS (Transacciones Controller) **********************/ 
        //****************************************************************************************/

        public async Task<ReporteTransaccionesDetalladas>
            ObtenerReporteTransaccionesDetalladas(int id_usuario, int mes,
            int anio, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechas(mes, anio);



            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                ID_USUARIO = id_usuario,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };
            var transacciones = await repositorioTransacciones.ObtenerPorID_Usuario(parametro);

            //MODELO
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);
            AsignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        private void AsignarValoresAlViewBag(dynamic ViewBag, DateTime fechaInicio)
        {
            //VIEWBAGS
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.HttpContext.Request.Path +
                httpContext.HttpContext.Request.QueryString;
        }


        //****************************************************************************************/
        //***** REPORTE TRANSACCIONES DETALLADAS (Transacciones Controller) **********************/ 
        //****************************************************************************************/



        //END CLASS

    }
}
