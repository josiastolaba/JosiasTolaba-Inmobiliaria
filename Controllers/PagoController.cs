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
        private readonly IConfiguration config;
        public PagoController(IRepositorioPago repositorio, IRepositorioContrato repoContrato, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.repoContrato = repoContrato;
            this.config = config;
        }
        public JsonResult Buscar(string dato)
        {
            var lista = repositorio.buscar(dato);
            return Json(lista);
        }
        public IActionResult Index()
        {
            var pagos = repositorio.ListarPagos();
            return View(pagos);
        }
        public IActionResult Create(int? id)
        {
            ViewBag.Contrato = repoContrato.ListarContratos();
            if (id != null)
            {
                ViewBag.Id = id;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Create(Pago p)
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
            int res = repositorio.DarDeBaja(IdPago);
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