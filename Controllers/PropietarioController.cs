using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;
using Microsoft.AspNetCore.Authorization;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
	[Authorize]
	public class PropietarioController : Controller
	{
		private readonly IRepositorioPropietario repositorio;
		private readonly IConfiguration config;
		public PropietarioController(IRepositorioPropietario repositorio, IConfiguration config)
		{
			this.repositorio = repositorio;
			this.config = config;
		}/*CONTROLADOR PARA BUSCAR*/

		[HttpGet]
		public JsonResult Buscar(string dato)
		{
			var lista = repositorio.buscar(dato);
			return Json(lista);
		}
		public IActionResult Index(int pagina = 1) //MODIFICADO, SE LE AGREGO EL PAGINADO
        {
            int paginaTam = 5;
            int totalPropietarios = repositorio.contar();

            int offset = (pagina - 1) * paginaTam;
            var propietarios = repositorio.obtenerPaginados(offset, paginaTam);

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalPropietarios / paginaTam);
            ViewBag.PaginaActual = pagina;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TablaPaginadaPropietarios", propietarios);
            }
            return View(propietarios);
        }
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Propietario p)
		{
			if (repositorio.existeDni(p.Dni))
			{
				ModelState.AddModelError("Dni", $"El DNI {p.Dni} ya está registrado");
			}
			if (ModelState.IsValid)
			{
				int res = repositorio.Alta(p);
				if (res > 0)
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ViewBag.Error = "No se pudo crear el propietario";
					return View();
				}
			}
			else
			{
				return View();
			}
		}
		public IActionResult Update(int IdPropietario)
		{
			Propietario p = repositorio.PropietarioId(IdPropietario);
			if (p == null)
			{
				return NotFound();
			}
			return View(p);
		}
		[HttpPost]
		public IActionResult Update(Propietario p)
		{
			if (repositorio.existeOtroDni(p.Dni, p.IdPropietario))
			{
				ModelState.AddModelError("Dni", $"El DNI {p.Dni} ya pertence a otro propietario");
			}


			if (ModelState.IsValid)
			{
				int res = repositorio.Modificacion(p);
				if (res > 0)
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ViewBag.Error = "No se pudo modificar el Propietario";
					return View(p);
				}
			}
			else
			{
				return View(p);
			}
		}
		public IActionResult Details(int IdPropietario)
		{
			Propietario p = repositorio.PropietarioId(IdPropietario);
			if (p == null)
			{
				return NotFound();
			}
			return View(p);
		}
		
	[Authorize(Policy = "Administrador")]
public IActionResult DarDeBaja(int IdPropietario)
{
    int res = repositorio.DarDeBaja(IdPropietario);

    if (res > 0)
    {
        TempData["Mensaje"] = "El propietario fue dado de baja correctamente.";
    }
    else
    {
        TempData["Error"] = "No se pudo dar de baja al propietario.";
    }

    return RedirectToAction(nameof(Index));
}

	}
}