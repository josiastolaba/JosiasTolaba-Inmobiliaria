using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly IConfiguration config;
        public UsuarioController(IRepositorioUsuario repositorio, IConfiguration config, IWebHostEnvironment environment)
        {
            this.repositorio = repositorio;
            this.config = config;
        }
        public IActionResult Index()
        {
            var usuarios = repositorio.ListarUsuarios();
            return View(usuarios);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Usuario u)
        {
            if (ModelState.IsValid)
            {
                int res = repositorio.Alta(u);
                if (res > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo crear el usuario";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Update(int IdUsuario)
        {
            Usuario u = repositorio.UsuarioId(IdUsuario);
            if (u == null)
            {
                return NotFound();
            }
            return View(u);
        }
        [HttpPost]
        public IActionResult Update(Usuario u)
        {
            if (ModelState.IsValid)
            {
                int res = repositorio.Modificacion(u);
                if (res > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo modificar el Usuario";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Details(int IdUsuario)
        {
            Usuario u = repositorio.UsuarioId(IdUsuario);
            if (u == null)
            {
                return NotFound();
            }
            return View(u);
        }
        public IActionResult DarDeBaja(int IdUsuario)
        {
            Usuario p = repositorio.UsuarioId(IdUsuario);
            int res = repositorio.DarDeBaja(IdUsuario);
            if (res > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "No se pudo eliminar el Usuario";
                return View();
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(UsuarioLogin login)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: login.Contrasena,
						salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));

					var e = repositorio.ObtenerPorEmail(login.Email);
					if (e == null || e.Contrasena != hashed)
					{
						ModelState.AddModelError("", "El email o la clave no son correctos");
						return View();
					}

					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, e.Email),
						new Claim("FullName", e.Nombre + " " + e.Apellido),
						new Claim(ClaimTypes.Role, e.Rol.ToString()),
					};

					var claimsIdentity = new ClaimsIdentity(
							claims, CookieAuthenticationDefaults.AuthenticationScheme);

					await HttpContext.SignInAsync(
							CookieAuthenticationDefaults.AuthenticationScheme,
							new ClaimsPrincipal(claimsIdentity));
					return Redirect("/Home");
				}
				return View();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}
    }
}