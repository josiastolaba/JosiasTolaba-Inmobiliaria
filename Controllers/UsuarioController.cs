using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly IConfiguration config;
        public UsuarioController(IRepositorioUsuario repositorio, IConfiguration config)
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
    }
}