using INMOBILIARIA_JosiasTolaba.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_.Net_Core.Controllers
{
	[Authorize]
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

		[HttpGet]
		public JsonResult Buscar(string dato)
		{
			var lista = repositorio.buscar(dato);
			return Json(lista);
		}

		public IActionResult contratosPorInquilino(int IdInquilino)
        {
			var lista = repositorio.contratosPorInquilino(IdInquilino);
			ViewBag.IdInquilino = IdInquilino;
			return View(lista);
        }

		 public IActionResult Index(int pagina = 1) //MODIFICADO, SE LE AGREGO EL PAGINADO
        {
            int paginaTam = 5;
            int totalContratos = repositorio.contar();

            int offset = (pagina - 1) * paginaTam;
            var contratos = repositorio.obtenerPaginados(offset, paginaTam);

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalContratos/ paginaTam);
            ViewBag.PaginaActual = pagina;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TablaPaginadaUsuarios", contratos);
            }
            return View(contratos);
        }
		[HttpGet]
		public JsonResult BuscarContratos(string term)
		{
			var contratos = repositorio.ListarContratos()
				.Where(c => c.Estado)
				.Where(c => string.IsNullOrEmpty(term)
						|| c.Propiedad.Direccion.Contains(term)
						|| c.Habitante.Nombre.Contains(term))
				.Select(c => new
				{
					id = c.IdContrato,
					text = $"{c.Propiedad.Direccion} - {c.Habitante.Nombre} {c.Habitante.Apellido}"
				})
				.ToList();

			return Json(contratos);
		}
		// GET: Contratos/Details/5
		public IActionResult Details(int IdContrato)
		{
    		var contrato = repositorio.IdContrato(IdContrato);
    		if (contrato == null)
        	return NotFound();

    		return View(contrato);
		}


		// GET: Contratos/Create
		public ActionResult Create(int? idInmueble)
		{
			try
			{
				ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
				ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
				if (idInmueble != null)
				{
					ViewBag.IdInmueble = idInmueble;
				}
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
					var userIdClaim = User.FindFirst("UserId")?.Value;
					int? idUsuarioLogueado = null;
					if (int.TryParse(userIdClaim, out int idParsed))
					{
						idUsuarioLogueado = idParsed;
					}
					entidad.QuienCreo = idUsuarioLogueado;
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
		public ActionResult Edit(int IdContrato)
		{
			var entidad = repositorio.IdContrato(IdContrato);
			ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
			ViewBag.Inmuebles = repoInmueble.ListarInmuebles(); ;
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Contratos/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int IdContrato, Contrato entidad)
		{
			try
			{
				entidad.IdContrato = IdContrato;
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
		[Authorize(Policy = "Administrador")]
		public ActionResult Delete(int IdContrato)
		{
			var entidad = repositorio.IdContrato(IdContrato);
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Contratos/Eliminar/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int IdContrato, Contrato entidad)
		{
			try
			{
				var userIdClaim = User.FindFirst("UserId")?.Value;
					int? idUsuarioLogueado = null;
					if (int.TryParse(userIdClaim, out int idParsed))
					{
						idUsuarioLogueado = idParsed;
					}
					int QuienElimino = (int)idUsuarioLogueado;
				repositorio.DarDeBaja(IdContrato, QuienElimino);
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
