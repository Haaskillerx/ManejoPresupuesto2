using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManejoPresupuesto.Controllers
{
    public class UsuariosController : Controller
    {
        private UserManager<Usuario> userManager;
        private SignInManager<Usuario> signInManager;

        public UsuariosController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {

            if(User.Identity.IsAuthenticated)
            {
                //Solamente si el usuario esta autenticado
                /* var claims = User.Claims.ToList();
                var id_usuarioREal = claims.Where(x => x.Type == ClaimTypes.NameIdentifier)
                    .FirstOrDefault();
                id_usuarioREal = id_usuarioREal.Value;
                */
                var claims = User.Claims.ToList();
                var id_usuario_real = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = id_usuario_real.Value;

            }else
            {

            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new Usuario() { EMAIL = modelo.Email };
            var resultado = await userManager
                .CreateAsync(usuario, password: modelo.Password);

            if(resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Transacciones");
            }else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View(modelo);
            }

            //return RedirectToAction("Index", "Transacciones");
        }





        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }


            var result = await signInManager.PasswordSignInAsync(modelo.Email,
            modelo.Password, modelo.RememberMe, false);


            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");
            }else
            {
            ModelState.AddModelError(String.Empty, "Error.");
            return View(modelo);
            }
            

        }





        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index","Transacciones");
        }









        //END CLASS
    }
}
