using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;
using Microsoft.AspNetCore.Authorization;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
    [Authorize]
    public class TipoInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;
        private readonly IConfiguration config;
        public TipoInmuebleController(IRepositorioTipoInmueble repositorio, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.config = config;
        }
        [HttpGet]
		public JsonResult Buscar(string dato)
		{
			var lista = repositorio.buscar(dato);
			return Json(lista);
		}
        public IActionResult Index()
        {
            var tipos = repositorio.ListarTipoInmueble();
            return View(tipos);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TipoInmueble entidad)
        {
            if (ModelState.IsValid)
            {
                int res = repositorio.Alta(entidad);
                if (res > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo crear el Tipo de Inmueble";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Update(int IdTipo)
        {
            var tipoInmueble = repositorio.TipoInmuebleId(IdTipo);
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }
        [HttpPost]
        public IActionResult Update(TipoInmueble entidad)
        {
            if (ModelState.IsValid)
            {
                int res = repositorio.Modificacion(entidad);
                if (res > 0)
                {
                    var pagos = repositorio.ListarTipoInmueble();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo modificar el Tipo de Inmueble";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Details(int IdTipo)
		{
			TipoInmueble p = repositorio.TipoInmuebleId(IdTipo);
			if (p == null)
			{
				return NotFound();
			}
			return View(p);
		}
		public IActionResult DarDeBaja(int IdTipo)
		{
			TipoInmueble p = repositorio.TipoInmuebleId(IdTipo);
			int res = repositorio.DarDeBaja(IdTipo);
			if (res > 0)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ViewBag.Error = "No se pudo eliminar el Tipo de Inmueble";
				return View();
			}
		}
    }
}