using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
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
        public IActionResult Index()
        {
            var pagos = repositorio.ListarPagos();
            return View(pagos);
        } 
        public IActionResult Create()
		{
            ViewBag.Contrato = repoContrato.ListarContratos();
			return View();
		}
    }
}