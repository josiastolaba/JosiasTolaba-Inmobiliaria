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
		public IActionResult Index()
		{
			var inquilinos = repositorio.ListarInquilinos();
			return View(inquilinos);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Inquilino i)
		{
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
					return View();
				}
			}
			else
			{
				return View();
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
					return View();
				}
			}
			else
			{
				return View();
			}
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