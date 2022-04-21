using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id_cuenta);
        Task<int> Crear(TipoCuenta tipoCuenta);
        Task<int> Existe(string nombre, int usuarioID);
        Task<IEnumerable<TipoCuenta>> Obtener(int id_usuario);
        Task<TipoCuenta> ObtenerPorID(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados);
    }


    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {

        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }


        public async Task<int> Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            int valor_retornable = 0;
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("TIPOS_CUENTAS_INSERTAR", connection);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO", tipoCuenta.ID_USUARIO);
                cmd.Parameters.AddWithValue("@NOMBRE", tipoCuenta.NOMBRE);
                // PARAMETRO DE SALIDA
                cmd.Parameters.Add("@ID_INSERTED", SqlDbType.Int).Direction = ParameterDirection.Output;
                // PARAMETRO DE SALIDA
                await cmd.ExecuteNonQueryAsync();

                //RETORNAMOS EL VALOR INSERTADO
                int contractID = Convert.ToInt32(cmd.Parameters["@ID_INSERTED"].Value);
                tipoCuenta.ID = contractID;
                // MessageBox.Show(""+ contractID);
                //return contractID;
                valor_retornable = contractID; 

            }
            catch (Exception ex)
            {
                valor_retornable = 0; // RETORNA 0 MANEJALO COMO ERROR! EN EL CONTROLADOR
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally{connection.Close();}

            return valor_retornable;
            
        }



        public  async Task<int> Existe(string nombre, int usuarioID)
        {
            int existe = 0;
            string StrQuery = "SELECT 1 FROM TIPOS_CUENTAS WHERE NOMBRE = @NOMBRE AND ID_USUARIO = @ID_USUARIO;";
            using var connection =  new SqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();

                SqlCommand cmd = new SqlCommand(StrQuery, connection);
                //cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@NOMBRE", nombre);
                cmd.Parameters.AddWithValue("@ID_USUARIO", usuarioID);
                //EJECUTAR QUERY
                //await cmd.ExecuteReaderAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        existe = 1;
                    }
                    else
                    {
                        existe = 0;
                    }
                    /*
                    while (reader.Read())
                    {

                    }*/
                }
            }
            catch (Exception ex)
            {
                //REturn to ERROR PAGE!
                //CREA UNA!!! PENDIENTE DE CREACION
                //Redirect("/Author/Index");
                existe = 0;
            }
            finally
            {
                //TERMINA LA CONEXION
                connection.Close();
            }

            //var existe = await connection.QueryFirstOrDefaultAsync<int>(sql, new { nombre, usuarioID });


            //

            // var existe = await connection.QueryFirstOrDefaultAsync<int>(sql, new { nombre, usuarioID });

            return existe;
        }






        //LISTADO CUENTAS
        public async Task<IEnumerable<TipoCuenta>> Obtener(int id_usuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT ID, NOMBRE, ORDEN FROM TIPOS_CUENTAS WHERE
            ID_USUARIO =@id_usuario ORDER BY ORDEN;", new { id_usuario });

        }



        //ACTUALIZAR TIPO CUENTA
        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            var name = tipoCuenta.NOMBRE;
            var id = tipoCuenta.ID;

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TIPOS_CUENTAS set NOMBRE = @name 
            WHERE ID = @id", new { id, name });


        }


        public async Task<TipoCuenta> ObtenerPorID(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT ID, NOMBRE, ORDEN FROM TIPOS_CUENTAS
                                            WHERE ID = @id AND ID_USUARIO = @usuarioId", new { id, usuarioId });
        }





        public async Task Borrar(int id_cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TIPOS_CUENTAS WHERE ID=@id_cuenta", new { id_cuenta });

        }



        //METODOO PARA ORDENAR
        public async Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados)
        {
            var query = "UPDATE TIPOS_CUENTAS SET Orden = @Orden Where ID = @ID";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tiposCuentasOrdenados);


        }

    }
}
