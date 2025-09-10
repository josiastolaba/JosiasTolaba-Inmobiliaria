using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IConfiguration config;
        public InmuebleController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietario, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietario;
            this.config = config;
        }
        public IActionResult Index()
        {
            var inmuebles = repositorio.ListarInmuebles();
            return View(inmuebles);
        }
        public IActionResult Create()
        {
            ViewBag.Propietarios = repoPropietario.ListarPropietarios();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Inmueble i)
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
                    ViewBag.Error = "No se pudo crear el Inmueble";
                    ViewBag.Propietarios = repoPropietario.ListarPropietarios();
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Update(int IdInmueble)
        {
            Inmueble i = repositorio.InmuebleId(IdInmueble);
            ViewBag.Propietarios = repoPropietario.ListarPropietarios();
            if (i == null)
            {
                return NotFound();
            }
            return View(i);
        }
        [HttpPost]
        public IActionResult Update(Inmueble i)
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
                    ViewBag.Error = "No se pudo modificar el Inmueble";
                    return View();
                }
            }
            else
            {
                return View(i);
            }
        }
        
        public IActionResult Details(int IdInmueble)
        {
            Inmueble i = repositorio.InmuebleId(IdInmueble);
            Propietario p = repoPropietario.PropietarioId(i.IdPropietario);
            ViewBag.Propietario = p;
            if (i == null)
            {
                return NotFound();
            }
            return View(i);
        }

        public IActionResult DarDeBaja(int IdInmueble)
        {
            Inmueble p = repositorio.InmuebleId(IdInmueble);
            int res = repositorio.DarDeBaja(IdInmueble);
            if (res > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "No se pudo eliminar el Inmueble";
                return View();
            }
        }
    }
}