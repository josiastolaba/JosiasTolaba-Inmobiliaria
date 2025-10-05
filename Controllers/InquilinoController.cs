using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
	public class InquilinoController : Controller
	{
		private readonly IRepositorioInquilino repositorio;
		private readonly IConfiguration config;
		public InquilinoController(IRepositorioInquilino repositorio, IConfiguration config)
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
		public IActionResult Index(int pagina = 1) //MODIFICADO, SE LE AGREGO EL PAGINADO
        {
            int paginaTam = 5;
            int totalInquilinos = repositorio.contar();

            int offset = (pagina - 1) * paginaTam;
            var inquilinos = repositorio.obtenerPaginados(offset, paginaTam);

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalInquilinos / paginaTam);
            ViewBag.PaginaActual = pagina;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TablaPaginadaInquilinos", inquilinos);
            }
            return View(inquilinos);
        }
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Inquilino i)
		{
			if (repositorio.existeDni(i.Dni))
			{
				ModelState.AddModelError("Dni", $"El DNI {i.Dni} ya está registrado");
			}
			if (ModelState.IsValid)
			{
				int res = repositorio.Alta(i);
				if (res > 0)
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ViewBag.Error = "No se pudo crear el Inquilino";
					return View(i);
				}
			}
			else
			{
				return View(i);
			}
		}
		public IActionResult Update(int IdInquilino)
		{
			Inquilino i = repositorio.InquilinoId(IdInquilino);
			if (i == null)
			{
				return NotFound();
			}
			return View(i);
		}
		[HttpPost]
		public IActionResult Update(Inquilino i)
		{
			if (repositorio.existeOtroDni(i.Dni, i.IdInquilino))
			{
				ModelState.AddModelError("Dni", $"El DNI {i.Dni} ya pertence a otro propietario");
			}
			if (ModelState.IsValid)
			{
				int res = repositorio.Modificacion(i);
				if (res > 0)
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ViewBag.Error = "No se pudo modificar el Inquilino";
					return View(i);
				}
			}
			else
			{
				return View(i);
			}
		}
		
		public IActionResult Details(int IdInquilino)
        {
            Inquilino i = repositorio.InquilinoId(IdInquilino);
            if (i == null)
            {
                return NotFound();
            }
            return View(i);
        }
		public IActionResult DarDeBaja(int IdInquilino)
		{
			Inquilino p = repositorio.InquilinoId(IdInquilino);
			int res = repositorio.DarDeBaja(IdInquilino);
			if (res > 0)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ViewBag.Error = "No se pudo eliminar el Inquilino";
				return View();
			}
		}
    }
}