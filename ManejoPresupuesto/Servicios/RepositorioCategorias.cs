using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id_cuenta);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int id_usuario);
        Task<IEnumerable<Categoria>> Obtener(int id_usuario, TipoOperacion tipoOperacion);
        Task<Categoria> ObtenerPorID(int id, int id_usuario);
    }
    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
            "INSERT INTO CATEGORIAS(NOMBRE, ID_OPERACION, ID_USUARIO) VALUES(@NOMBRE, @ID_OPERACION, @ID_USUARIO); " +
            "SELECT SCOPE_IDENTITY(); ",categoria);
            //asignamos valor
            categoria.ID = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int id_usuario, TipoOperacion tipoOperacion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(
            "SELECT * FROM CATEGORIAS WHERE ID_USUARIO = @id_usuario AND ID_OPERACION=@tipoOperacion", 
            new { id_usuario, tipoOperacion });

        }



        //
        public async Task<IEnumerable<Categoria>> Obtener(int id_usuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(
            "SELECT * FROM CATEGORIAS WHERE ID_USUARIO = @id_usuario", new { id_usuario });
        }

        //OBTENER POR ID
        public async Task<Categoria> ObtenerPorID(int id, int id_usuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(
            "SELECT * FROM CATEGORIAS WHERE ID=@id AND ID_USUARIO=@id_usuario", 
            new { id, id_usuario });
        }


        //EDITAR
        //EDITAR ACTUALIZAR

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"UPDATE CATEGORIAS SET NOMBRE = @NOMBRE, ID_OPERACION = @ID_OPERACION
                WHERE ID=@ID", categoria);
        }


        //BORRAR



        public async Task Borrar(int id_cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"DELETE CATEGORIAS WHERE ID=@id_cuenta", new { id_cuenta });
        }

    }
}
