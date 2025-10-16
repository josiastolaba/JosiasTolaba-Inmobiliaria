using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;
using Microsoft.AspNetCore.Authorization;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioContrato repoContrato;

        private readonly IRepositorioInquilino repoInquilino;
        private readonly IConfiguration config;
        public PagoController(IRepositorioPago repositorio, IRepositorioInquilino repoInquilino, IRepositorioContrato repoContrato, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.repoContrato = repoContrato;
            this.repoInquilino = repoInquilino;
            this.config = config;
        }



        [HttpGet]
        public IActionResult Buscar()
        {
            return View(); // vista "Buscar.cshtml"
        }
        
        [HttpGet]
public JsonResult BuscarInquilino(string dato)
{
    if (string.IsNullOrWhiteSpace(dato) || dato.Length < 3)
        return Json(new List<object>()); // no buscar si hay menos de 3 letras

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


        [HttpGet]
        public IActionResult BuscarPagos(DateTime? fechaDesde, DateTime? fechaHasta, String filtroInquilino)
        {
            var lista = repositorio.buscarAvanzado(fechaDesde, fechaHasta, filtroInquilino);
            return PartialView("_TablaPagos", lista);
        }
        
        public IActionResult PagosPorContrato(int IdContrato)
{
    var lista = repositorio.pagoPorContrato(IdContrato);
    ViewBag.IdContrato = IdContrato;

    if (lista.Any())
    {
        ViewBag.InquilinoNombre = lista.First().Contrato?.Habitante?.Nombre + " " +
                                  lista.First().Contrato?.Habitante?.Apellido;
    }

    return View(lista);
}

       public IActionResult Index(int pagina = 1) //MODIFICADO, SE LE AGREGO EL PAGINADO
        {
            int paginaTam = 5;
            int totalPagos = repositorio.contar();

            int offset = (pagina - 1) * paginaTam;
            var pagos = repositorio.obtenerPaginados(offset, paginaTam);

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalPagos/ paginaTam);
            ViewBag.PaginaActual = pagina;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TablaPaginadaPagos", pagos);
            }
            return View(pagos);
        }
        public IActionResult Create(int? IdContrato)
        {
            ViewBag.Contratos = repoContrato.ListarContratos();
            if (IdContrato != null)
            {
                ViewBag.Id = IdContrato;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Create(Pago p)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
					int? idUsuarioLogueado = null;
					if (int.TryParse(userIdClaim, out int idParsed))
					{
						idUsuarioLogueado = idParsed;
					}
					p.QuienCreo = idUsuarioLogueado;
                int res = repositorio.Alta(p);
                if (res > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Contrato = repoContrato.ListarContratos();
                    ViewBag.Error = "No se pudo crear el pago";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public IActionResult Update(int IdPago)
        {
            Pago p = repositorio.PagoId(IdPago);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        [HttpPost]
        public IActionResult Update(Pago p)
        {
            if (ModelState.IsValid)
            {
                int res = repositorio.Modificacion(p);
                if (res > 0)
                {
                    var pagos = repositorio.ListarPagos();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo modificar el Pago";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Details(int IdPago)
        {
            Pago p = repositorio.PagoId(IdPago);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }
        [Authorize(Policy = "Administrador")]
        public IActionResult DarDeBaja(int IdPago)
        {
            Pago p = repositorio.PagoId(IdPago);
            var userIdClaim = User.FindFirst("UserId")?.Value;
					int? idUsuarioLogueado = null;
					if (int.TryParse(userIdClaim, out int idParsed))
					{
						idUsuarioLogueado = idParsed;
					}
					int QuienElimino = (int)idUsuarioLogueado;
            int res = repositorio.DarDeBaja(IdPago,QuienElimino);
            if (res > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "No se pudo eliminar el Pago";
                return View();
            }
        }
    }
}