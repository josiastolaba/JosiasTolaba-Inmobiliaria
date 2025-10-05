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

		public IActionResult Index(int pagina = 1) //MODIFICADO, SE LE AGREGO EL PAGINADO
        {
            int paginaTam = 5;
            int totalContratos = repositorio.contar();

            int offset = (pagina - 1) * paginaTam;
            var contratos = repositorio.obtenerPaginados(offset, paginaTam);

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalContratos / paginaTam);
            ViewBag.PaginaActual = pagina;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TablaPaginadaContratos", contratos);
            }
            return View(contratos);
        }

		public ActionResult Details(int id)
		{
			var entidad = repositorio.IdContrato(id);
			return View(entidad);
		}

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
			TempData["Mensaje"] = "Contrato creado correctamente";
			return RedirectToAction(nameof(Index));
		}
		else
		{
			// Si hay errores de validación, recargo los combos
			ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
			ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
			return View(entidad);
		}
	}
	catch (Exception ex)
	{
		ViewBag.Error = ex.Message;
		ViewBag.StackTrace = ex.StackTrace;
		ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
		ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
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

		if (ModelState.IsValid)
		{
			repositorio.Modificacion(entidad);
			TempData["Mensaje"] = "Contrato modificado correctamente";
			return RedirectToAction(nameof(Index));
		}
		else
		{
			// Si la validación falla, se recargan las listas y se vuelve a mostrar la vista
			ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
			ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
			return View(entidad);
		}
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
		public ActionResult Delete(int id)
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
		public ActionResult Delete(int id, Contrato entidad)
		{
			try
			{
				repositorio.DarDeBaja(id); 
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
