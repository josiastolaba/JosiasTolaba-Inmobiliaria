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

        /*CONTROLADOR PARA BUSCAR*/
		[HttpGet]
		public JsonResult Buscar(string dato)
		{
            var lista = repositorio.buscar(dato);
			return Json(lista);
		}
        public IActionResult Index(int pagina = 1) //MODIFICADO, SE LE AGREGO EL PAGINADO
        {
            int paginaTam = 5;
            int totalUsuarios = repositorio.contar();

            int offset = (pagina - 1) * paginaTam;
            var usuarios = repositorio.obtenerPaginados(offset, paginaTam);

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalUsuarios / paginaTam);
            ViewBag.PaginaActual = pagina;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TablaPaginadaUsuarios", usuarios);
            }
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