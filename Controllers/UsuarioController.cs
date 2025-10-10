using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA_JosiasTolaba.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace INMOBILIARIA_JosiasTolaba.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly IConfiguration config;

        private readonly IWebHostEnvironment env;
        public UsuarioController(IRepositorioUsuario repositorio, IConfiguration config, IWebHostEnvironment env)
        {
            this.repositorio = repositorio;
            this.config = config;
             this.env = env;
        }
     public IActionResult Index(int pagina = 1) //MODIFICADO, SE LE AGREGO EL PAGINADO
        {
            int paginaTam = 5;
            int totalUsuarios = repositorio.contar();

            int offset = (pagina - 1) * paginaTam;
            var usuarios = repositorio.obtenerPaginados(offset, paginaTam);

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalUsuarios / paginaTam);
            ViewBag.PaginaActual = pagina;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TablaPaginadaUsuarios", usuarios);
            }
            return View(usuarios);
        }


        [Authorize(Policy = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        public IActionResult Create(Usuario u, IFormFile? AvatarFile)
        {
            if (repositorio.existeDni(u.Dni))
            {
                ModelState.AddModelError("Dni", $"El DNI {u.Dni} ya está registrado");
            }
            
            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(env.WebRootPath, "imagenes/usuarios");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // --- Manejo de imagen subida o por defecto ---
                if (AvatarFile != null && AvatarFile.Length > 0)
                {
                    // Limpia el email para usarlo en el nombre del archivo
                    string safeEmail = u.Email.Replace("@", "_at_").Replace(".", "_");
                    string extension = Path.GetExtension(AvatarFile.FileName);
                    string fileName = $"usuario_{safeEmail}_{Guid.NewGuid()}{extension}";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        AvatarFile.CopyTo(stream);
                    }

                    u.Avatar = $"/imagenes/usuarios/{fileName}";
                }
                else
                {
                    u.Avatar = "/imagenes/usuarios/default-avatar.png";
                }

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: u.Contrasena,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                u.Contrasena = hashed;

                int res = repositorio.Alta(u);
                if (res > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo crear el usuario";
                    return View(u);
                }
            }
            else
            {
                return View(u);
            }
        }


[HttpPost]
public IActionResult SubirAvatar(int idUsuario, IFormFile avatar)
{
    try
    {
        if (avatar == null || avatar.Length == 0)
            return Json(new { ok = false, mensaje = "No se recibió ningún archivo." });

        string uploadsFolder = Path.Combine(env.WebRootPath, "imagenes/usuarios");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string fileName = $"usuario_{idUsuario}_{Path.GetFileName(avatar.FileName)}";
        string filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            avatar.CopyTo(stream);
        }

        string rutaWeb = $"/imagenes/usuarios/{fileName}";

        // Actualizar en la base de datos
        repositorio.subirAvatar(idUsuario, rutaWeb);

        return Json(new { ok = true, nuevaRuta = rutaWeb });
    }
    catch (Exception ex)
    {
        return Json(new { ok = false, mensaje = "Error al subir la imagen: " + ex.Message });
    }
}

        public IActionResult Update(int IdUsuario)
        {
            Usuario u = repositorio.UsuarioId(IdUsuario);
            u.Contrasena = "";
            if (u == null)
            {
                return NotFound();
            }
            return View(u);
        }
        [HttpPost]
        public IActionResult Update(Usuario u, IFormFile? AvatarFile)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            int.TryParse(userIdClaim, out int idUsuarioLogueado);

            var esAdmin = User.IsInRole("Administrador");

            if (!esAdmin && u.IdUsuario != idUsuarioLogueado)
            {
                return Forbid();
            }
            if (repositorio.existeOtroDni(u.Dni, u.IdUsuario))
            {
                ModelState.AddModelError("Dni", $"El DNI {u.Dni} ya pertence a otro propietario");
            }
            ModelState.Remove("Contrasena");
            if (ModelState.IsValid)
            {
                var existente = repositorio.UsuarioId(u.IdUsuario);

                // Si la contraseña está vacía se mantiene la anterior
                if (string.IsNullOrWhiteSpace(u.Contrasena))
                {
                    u.Contrasena = existente.Contrasena;
                }
                else
                {
                    // Si el usuario escribió una nueva, la cambiamos
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Contrasena,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8
                    ));
                    u.Contrasena = hashed;
                }

                // Manejo de imagen
                if (AvatarFile != null && AvatarFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(env.WebRootPath, "imagenes/usuarios");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string extension = Path.GetExtension(AvatarFile.FileName);
                    string fileName = $"usuario_{u.IdUsuario}_{Path.GetFileName(AvatarFile.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        AvatarFile.CopyTo(stream);
                    }

                    u.Avatar = $"/imagenes/usuarios/{fileName}";
                }
                else
                {
                    // Mantener la imagen actual
                    u.Avatar = existente.Avatar;
                }

                int res = repositorio.Modificacion(u);
                if (res > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo modificar el usuario";
                    return View(u);
                }
            }
            else
            {
                return View(u);
            }
        }

        public IActionResult Details(int IdUsuario)
        {
            Usuario u = repositorio.UsuarioId(IdUsuario);
            if (u == null)
            {
                return NotFound();
            }
            return View(u);
        }
        [Authorize(Policy = "Administrador")]
        public IActionResult DarDeBaja(int IdUsuario)
        {
            Usuario p = repositorio.UsuarioId(IdUsuario);
            int res = repositorio.DarDeBaja(IdUsuario);
            if (res > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "No se pudo eliminar el Usuario";
                return View();
            }
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UsuarioLogin login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Contrasena,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    var usuario = repositorio.ObtenerPorEmail(login.Email);
                    if (usuario == null || usuario.Contrasena != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        return View();
                    }
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                        new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                        new Claim("UserId", usuario.IdUsuario.ToString())
                    };
                    var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));
                    return Redirect("/Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
