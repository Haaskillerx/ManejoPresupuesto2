using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario user);
        Task<int> ID_USUARIO(string emailNormalizado);
    }
    public class RepositorioUsuarios: IRepositorioUsuarios
    {

        private readonly string connectionString;
        public RepositorioUsuarios(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            //usuario.EmailNormalizado = usuario.FindByEmailAsync().ToUpper();
            using var con = new SqlConnection(connectionString);
            var id = await con.QuerySingleAsync<int>(
                @"INSERT INTO USUARIOS (EMAIL, EMAIL_NORMALIZADO, PASSWORD)
                VALUES (@EMAIL, @EMAIL_NORMALIZADO, @PASSWORD);
                SELECT SCOPE_IDENTITY();"
                , usuario);

            var id_usuario = id;
            //Crear usuario con el storeprocedure
            await con.ExecuteAsync("CrearDatosUsuarioNuevo", new { id_usuario },
                commandType: System.Data.CommandType.StoredProcedure);

            return id;
        }


        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(@"
            SELECT * FROM USUARIOS WHERE EMAIL_NORMALIZADO = @emailNormalizado ",
            new { emailNormalizado });
        }


        public async Task<int> ID_USUARIO(string emailNormalizado)
        {

            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<int>(@"
            SELECT ID FROM USUARIOS WHERE EMAIL_NORMALIZADO = @emailNormalizado ",
            new { emailNormalizado });

        }


        //END CLASS
    }
}
