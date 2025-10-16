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
		public IActionResult BuscarContratosAvanzado()
		{
    	return View(); // Carga la vista principal completa
		}

		[HttpGet]
		public IActionResult BuscarContratosAvanzadoTabla(DateTime? fechaDesde, DateTime? fechaHasta, int? idInquilino)
		{
   		 var lista = repositorio.buscarAvanzadoContratos(fechaDesde, fechaHasta, idInquilino);
    	return PartialView("_TablaContratos", lista);
		}

		[HttpGet]
		public JsonResult BuscarInquilino(string dato) //COMPLEMENTO PARA EL BUSCADOR AVANZADO
		{
    	if (string.IsNullOrWhiteSpace(dato) || dato.Length < 3)
        	return Json(new List<object>());

    	var lista = repoInquilino.buscar(dato)
        	.Select(i => new
        	{
            idInquilino = i.IdInquilino,
            nombreCompleto = $"{i.Nombre} {i.Apellido}",
            dni = i.Dni
        	})
        	.ToList();

    	return Json(lista);
		}



		public IActionResult contratosPorInquilino(int IdInquilino)
		{
			var lista = repositorio.contratosPorInquilino(IdInquilino);
			ViewBag.IdInquilino = IdInquilino;
			return View(lista);
		}
		
		 public IActionResult ContratosPorInmueble(int IdInmueble)
{
    Console.WriteLine($"Buscando contratos del inmueble {IdInmueble}"); 
    var lista = repositorio.contratoPorInmueble(IdInmueble);
    ViewBag.IdInmueble = IdInmueble;
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
			//Auditoría: obtiene el ID del usuario logueado
			var userIdClaim = User.FindFirst("UserId")?.Value;
			int? idUsuarioLogueado = null;
			if (int.TryParse(userIdClaim, out int idParsed))
			{
				idUsuarioLogueado = idParsed;
			}
			entidad.QuienCreo = idUsuarioLogueado;

			// Intenta crear el contrato (puede lanzar InvalidOperationException)
			repositorio.Alta(entidad);

			return RedirectToAction(nameof(Index));
		}
		else
		{
			ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
			ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
			ViewBag.Error = "Los datos ingresados no son válidos.";
			return View(entidad);
		}
	}
	catch (InvalidOperationException ex)
	{
		//Caso específico: superposición de fechas
		ViewBag.Error = ex.Message;
		ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
		ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
		return View(entidad);
	}
	catch (Exception ex)
	{
		//Otros errores
		ViewBag.Error = "Ocurrió un error al crear el contrato: " + ex.Message;
		ViewBag.StackTrace = ex.StackTrace;
		ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
		ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
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
        if (ModelState.IsValid)
        {
            //Auditoría: usuario que modifica
            var userIdClaim = User.FindFirst("UserId")?.Value;
            int? idUsuarioLogueado = null;
            if (int.TryParse(userIdClaim, out int idParsed))
            {
                idUsuarioLogueado = idParsed;
            }

            entidad.IdContrato = IdContrato;
            entidad.QuienCreo = idUsuarioLogueado;

            //Intentar modificación
            repositorio.Modificacion(entidad);
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
            ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
            return View(entidad);
        }
    }
    catch (InvalidOperationException ex)
    {
        //Error controlado desde el repositorio (superposición de fechas)
        ViewBag.Error = ex.Message;
        ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
        ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
        return View(entidad);
    }
    catch (Exception ex)
    {
        //Otros errores inesperados
        ViewBag.Error = "Ocurrió un error al modificar el contrato.";
        ViewBag.StackTrace = ex.StackTrace;
        ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
        ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
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
		[Authorize(Policy = "Administrador")]
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
		public IActionResult Renovar(int IdContrato)
		{
			ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
			ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
			if (IdContrato == 0)
				return View(nameof(Index));
			var contratoOriginal = repositorio.IdContrato(IdContrato);
			int diferenciaDias = (contratoOriginal.FechaFin - contratoOriginal.FechaInicio).Days;
			var contratoRenovado = new Contrato
			{
				FechaInicio = contratoOriginal.FechaFin.AddDays(1),
				FechaFin = contratoOriginal.FechaFin.AddDays(diferenciaDias + 1),
				MontoMensual = contratoOriginal.MontoMensual,
				Propiedad = contratoOriginal.Propiedad,
				Habitante = contratoOriginal.Habitante,
				Estado = true
			};
			ViewBag.IdInmueble = contratoOriginal.Propiedad?.IdInmueble;
			return View(contratoRenovado);
		}
		[HttpPost]
		public IActionResult Renovar(Contrato c)
		{
			if (ModelState.IsValid)
			{
				var userIdClaim = User.FindFirst("UserId")?.Value;
				int? idUsuarioLogueado = null;
				if (int.TryParse(userIdClaim, out int idParsed))
				{
					idUsuarioLogueado = idParsed;
				}
				c.QuienCreo = idUsuarioLogueado;
				repositorio.Alta(c);
				TempData["Id"] = c.IdContrato;
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ViewBag.Inquilinos = repoInquilino.ListarInquilinos();
				ViewBag.Inmuebles = repoInmueble.ListarInmuebles();
				return View(c);
			}
		}
		public IActionResult Terminar(int IdContrato)
		{
			var contrato = repositorio.IdContrato(IdContrato);
			DateTime hoy = DateTime.Today;
			int totalDias = (contrato.FechaFin - contrato.FechaInicio).Days;
			int diasTranscurridos = (hoy - contrato.FechaInicio).Days;
			ViewBag.PasoMasDeLaMitad = diasTranscurridos > totalDias / 2;
			if (contrato == null)
				return NotFound();
			return View(contrato);
		}
		[HttpPost]
		public IActionResult Terminar(int IdContrato, Contrato entidad)
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
				repositorio.TerminarContrato(IdContrato, QuienElimino);
				TempData["Mensaje"] = "Contrato terminado correctamente";
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
