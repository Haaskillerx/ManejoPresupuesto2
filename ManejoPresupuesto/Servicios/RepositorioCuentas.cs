using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id_cuenta);
        Task<IEnumerable<Cuenta>> Buscar(int id_usuario);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int id_usuario);
    }


    public class RepositorioCuentas: IRepositorioCuentas
    {

        //CONEXION
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }
        //CONEXION

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO CUENTAS (NOMBRE, ID_TIPOCUENTA, DESCRIPCION, BALANCE)
                VALUES (@NOMBRE, @ID_TIPOCUENTA, @DESCRIPCION, @BALANCE); 
                
                SELECT SCOPE_IDENTITY();", cuenta);
            
            //asignamos valor
            cuenta.ID = id;
        }


        //GET INNER JOINR

        public async Task<IEnumerable<Cuenta>> Buscar(int id_usuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(
                @"SELECT C.ID, C.NOMBRE, C.BALANCE, TC.NOMBRE AS TipoCuenta
                FROM CUENTAS C INNER JOIN TIPOS_CUENTAS TC ON TC.ID = C.ID_TIPOCUENTA WHERE TC.ID_USUARIO = @id_usuario
                ORDER BY TC.ORDEN;", new{ id_usuario });

         
            
        }


        //EDITAR BORRAR

        public async Task<Cuenta> ObtenerPorId(int id, int id_usuario)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"SELECT C.ID, C.NOMBRE, C.BALANCE, TC.ID FROM CUENTAS C INNER JOIN TIPOS_CUENTAS TC ON
                TC.ID=C.ID_TIPOCUENTA 
                WHERE TC.ID_USUARIO = @id_usuario AND C.ID = @id", new {id, id_usuario});
        }




        //EDITAR ACTUALIZAR

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"UPDATE CUENTAS SET NOMBRE = @NOMBRE, BALANCE = @BALANCE, DESCRIPCION=@DESCRIPCION,
                ID_TIPOCUENTA=@ID_TIPOCUENTA WHERE ID=@ID", cuenta);
        }



        //EDITAR ACTUALIZAR

        public async Task Borrar(int id_cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"DELETE CUENTAS WHERE ID=@ID", id_cuenta);
        }


    }
}
