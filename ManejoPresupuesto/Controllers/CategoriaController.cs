using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServiciosUsuarios servicioUsuarios;
        

        public CategoriaController(IRepositorioCategorias repositorioCategorias,
            IServiciosUsuarios servicioUsuarios)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }
        public async Task<IActionResult> Index()
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await repositorioCategorias.Obtener(id_usuario);

            return View(categorias);
            
        }


        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            if(!ModelState.IsValid)
            {
                return View(categoria);
            }

            categoria.ID_USUARIO = id_usuario;
            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }



        //EDITAR
        
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorID(id, id_usuario);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }


            //MAPEO AUTOMAPPER
            //var modelo = mapper.Map<Categoria>(categoria);

           // modelo.TiposCuentas = await ObtenerTiposCuentas(id_usuario);
            return View(categoria);


        }
        


        //EDITAR POST
        
        [HttpPost]

        public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaEditar);
            }

            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorID(categoriaEditar.ID, id_usuario);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }

            categoriaEditar.ID_USUARIO = id_usuario;
            await repositorioCategorias.Actualizar(categoriaEditar);
            return RedirectToAction("Index");

        }
        



        //BORRAR
        
        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorID(id, id_usuario);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }


            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var id_usuario = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorID(id, id_usuario);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado2", "Home");
            }
            await repositorioCategorias.Borrar(id);
            return RedirectToAction("Index");
        }
        
    }
}
