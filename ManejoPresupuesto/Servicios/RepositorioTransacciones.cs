using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int ID);
        Task Crear(Transaccion transaccion);
        Task<Transaccion> ObtenerPorID(int id, int id_usuario);
        Task<IEnumerable<Transaccion>> ObtenerPorIDCuenta(ObtenerTransaccionesPorCuenta modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorID_Usuario(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int id_usuario, int Año);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
    }


    public class RepositorioTransacciones : IRepositorioTransacciones
    {

        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }


        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("INSERTAR_TRANSACCIONES",
                new 
                {
                    transaccion.ID_USUARIO,
                    transaccion.FechaTransaccion,
                    transaccion.MONTO,
                    transaccion.ID_CATEGORIA,
                    transaccion.ID_CUENTA,
                    transaccion.NOTA
                },
                commandType: System.Data.CommandType.StoredProcedure);
                transaccion.ID = id;
                
        }

        //EDITAR = ACTUALIZAR TRANSACCION
        public async Task Actualizar(Transaccion transaccion, decimal MONTOANTERIOR, int ID_CUENTA_ANTERIOR)
        {
            using var connection = new SqlConnection(connectionString);
            /*
            await connection.ExecuteAsync("ACTUALIZAR_TRANSACCIONES",
                new
                {
                    transaccion.ID,
                    
                    transaccion.FechaTransaccion,
                    transaccion.MONTO,
                    transaccion.ID_CATEGORIA,
                    transaccion.ID_CUENTA,
                    transaccion.NOTA,
                    MONTOANTERIOR,
                    ID_CUENTA_ANTERIOR
                },
                commandType: System.Data.CommandType.StoredProcedure);
            */
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("ACTUALIZAR_TRANSACCIONES", connection);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", transaccion.ID);
                cmd.Parameters.AddWithValue("@FechaTransaccion", transaccion.FechaTransaccion);
                cmd.Parameters.AddWithValue("@MONTO", transaccion.MONTO);
                cmd.Parameters.AddWithValue("@ID_CATEGORIA", transaccion.ID_CATEGORIA);
                cmd.Parameters.AddWithValue("@ID_CUENTA", transaccion.ID_CUENTA);
                cmd.Parameters.AddWithValue("@NOTA", transaccion.NOTA);
                cmd.Parameters.AddWithValue("@MONTOANTERIOR", MONTOANTERIOR);
                cmd.Parameters.AddWithValue("@ID_CUENTA_ANTERIOR", ID_CUENTA_ANTERIOR);
                // PARAMETRO DE SALIDA
                //cmd.Parameters.Add("@ID_INSERTED", SqlDbType.Int).Direction = ParameterDirection.Output;
                // PARAMETRO DE SALIDA
                await cmd.ExecuteNonQueryAsync();

                //RETORNAMOS EL VALOR INSERTADO
                
            }
            catch (Exception ex)
            {
                //
            }
            


        }


        //Transaccion

        public async Task<Transaccion> ObtenerPorID(int id, int id_usuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT TRANSACCIONES.*, CAT.ID_OPERACION
                FROM TRANSACCIONES
                INNER JOIN CATEGORIAS CAT
                ON CAT.ID = TRANSACCIONES.ID_CATEGORIA
                WHERE TRANSACCIONES.ID = @id AND TRANSACCIONES.ID_USUARIO=@id_usuario",
                new { id, id_usuario });
        }


        //OBTENER POR CUENTA
        public async Task<IEnumerable<Transaccion>> ObtenerPorIDCuenta(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(
                @"SELECT T.ID, T.MONTO, T.FECHA_TRANSACCION, C.NOMBRE AS CATEGORIA,
                CU.NOMBRE AS CUENTA, C.ID_OPERACION
                FROM TRANSACCIONES T
                INNER JOIN CATEGORIAS C
                ON C.ID = T.ID_CATEGORIA
                INNER JOIN CUENTAS CU
                ON CU.ID = T.ID_CUENTA
                WHERE T.ID_CUENTA = @ID_CUENTA AND T.ID_USUARIO = @ID_USUARIO 
                AND FECHA_TRANSACCION BETWEEN @FechaInicio AND @FechaFin", modelo);
        }

        public async Task Borrar(int ID)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("TRANSACCIONES_BORRAR", 
                new
                {
                    ID
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }














        //OBTENER POR USUARIO ID

        public async Task<IEnumerable<Transaccion>> ObtenerPorID_Usuario(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(
                @"SELECT T.ID, T.MONTO, T.FECHA_TRANSACCION, C.NOMBRE AS CATEGORIA,
                CU.NOMBRE AS CUENTA, C.ID_OPERACION
                FROM TRANSACCIONES T
                INNER JOIN CATEGORIAS C
                ON C.ID = T.ID_CATEGORIA
                INNER JOIN CUENTAS CU
                ON CU.ID = T.ID_CUENTA
                WHERE T.ID_USUARIO = @ID_USUARIO 
                AND FECHA_TRANSACCION BETWEEN @FechaInicio AND @FechaFin
                ORDER BY T.FECHA_TRANSACCION DESC", modelo);
        }




        //OBTENER POR SEMANA (TRANSACCIONES)

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana
        (ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(
                @"SELECT datediff(d, @FechaInicio, FECHA_TRANSACCION) /7+1 as Semana,
                SUM(MONTO) as Monto, C.ID_OPERACION
                FROM Transacciones T
                INNER JOIN CATEGORIAS C
                ON C.ID = T.ID_CATEGORIA
                WHERE T.ID_USUARIO = @ID_USUARIO AND
                FECHA_TRANSACCION BETWEEN @FechaInicio AND @FechaFin
                GROUP BY datediff(d, @FechaInicio, FECHA_TRANSACCION) / 7, C.ID_OPERACION",
                
                modelo);
        }

        //// OBTENER POR MENSUAL (TRANSACCIONES)

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes
        (int id_usuario, int Año)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(
                @"SELECT MONTH(FECHA_TRANSACCION) as Mes,
                SUM(Monto) as Monto, C.ID_OPERACION as ID_OPERACION
                FROM Transacciones T INNER JOIN CATEGORIAS C ON C.ID = T.ID_CATEGORIA
                WHERE T.ID_USUARIO = @id_usuario AND YEAR(FECHA_TRANSACCION) = @Año
                GROUP BY Month(FECHA_TRANSACCION), C.ID_OPERACION;",

                new {id_usuario, Año });
        }



        //eDN CLAS
    }
}
