using INMOBILIARIA_JosiasTolaba.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_.Net_Core.Controllers
{
	public class ContratoController : Controller
	{
		private readonly IRepositorioContrato repositorio;
		private readonly IConfiguration config;
		private readonly IRepositorioInquilino repoInquilino;
		private readonly IRepositorioInmueble repoInmueble;

		public ContratoController(IRepositorioContrato repositorio, IRepositorioInquilino repoInquilino, IRepositorioInmueble repoInmueble, IConfiguration config)
		{
			this.repositorio = repositorio;
			this.repoInquilino = repoInquilino;
			this.repoInmueble = repoInmueble;
			this.config = config;
		}

		// GET: Contratos
		public ActionResult Index()
		{
			var lista = repositorio.ListarContratos();
			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			return View(lista);
		}

		// GET: Contratos/Details/5
		public ActionResult Details(int id)
		{
			var entidad = repositorio.IdContrato(id);
			return View(entidad);
		}

		// GET: Contratos/Create
		public ActionResult Create()
		{
			try
			{
				ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
				ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
				return View();
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// POST: Contratos/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Contrato entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					repositorio.Alta(entidad); 
					TempData["Id"] = entidad.IdContrato;
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
					ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
					return View(entidad);
				}
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				ViewBag.StackTrace = ex.StackTrace;
				return View(entidad);
			}
		}

		// GET: Contratos/Edit/5
		public ActionResult Edit(int id)
		{
			var entidad = repositorio.IdContrato(id);
			ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
			ViewBag.Inmuebles = repoInmueble.ListarInmuebles();;
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Contratos/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, Contrato entidad)
		{
			try
			{
				entidad.IdContrato = id;
				repositorio.Modificacion(entidad);
				TempData["Mensaje"] = "Contrato modificado correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
				ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
				ViewBag.Error = ex.Message;
				ViewBag.StackTrace = ex.StackTrace;
				return View(entidad);
			}
		}

		// GET: Contratos/Eliminar/5
		public ActionResult Eliminar(int id)
		{
			var entidad = repositorio.IdContrato(id);
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Contratos/Eliminar/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Eliminar(int id, Contrato entidad)
		{
			try
			{
				repositorio.Baja(id); 
				TempData["Mensaje"] = "Contrato dado de baja correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				ViewBag.StackTrace = ex.StackTrace;
				return View(entidad);
			}
		}
	}
}
