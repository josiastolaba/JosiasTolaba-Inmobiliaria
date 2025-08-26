using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
	public class PropietarioController : Controller
	{
		private readonly IRepositorioPropietario repositorio;
		private readonly IConfiguration config;
		public PropietarioController(IRepositorioPropietario repositorio, IConfiguration config)
		{
			this.repositorio = repositorio;
			this.config = config;
		}
		public IActionResult Index()
		{
			var propietarios = repositorio.ListarPropietarios();
			return View(propietarios);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Propietario p)
		{
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
					return View();
				}
			}
			else
			{
				return View();
			}
		}
    }
}