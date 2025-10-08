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
        [Authorize(Policy = "Administrador")]
        public IActionResult Index()
        {
            var usuarios = repositorio.ListarUsuarios();
            return View(usuarios);
        }

        [HttpPost]
public IActionResult SubirAvatar(int idUsuario, IFormFile avatar)
{
    try
    {
        if (avatar != null && avatar.Length > 0)
        {
            string wwwPath = env.WebRootPath;
            string path = Path.Combine(wwwPath, "imagenes/usuarios");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = $"usuario_{idUsuario}_{Path.GetFileName(avatar.FileName)}";
            string filePath = Path.Combine(path, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                avatar.CopyTo(stream);
            }

            string relativePath = $"/imagenes/usuarios/{fileName}";

            // Guardamos la ruta en la BD
            var usuario = repositorio.UsuarioId(idUsuario);
            usuario.Avatar = relativePath;
            repositorio.Modificacion(usuario);

            return Json(new { ok = true, nuevaRuta = relativePath });
        }
        return Json(new { ok = false, mensaje = "No se recibió archivo" });
    }
    catch (Exception ex)
    {
        return Json(new { ok = false, mensaje = ex.Message });
    }
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
            if (ModelState.IsValid)
            {
                if (AvatarFile != null && AvatarFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(env.WebRootPath, "imagenes/usuarios");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string safeEmail = u.Email.Replace("@", "_at_").Replace(".", "_");
                    string extension = Path.GetExtension(AvatarFile.FileName);
                    string fileName = $"usuario_{u.Email}_{Path.GetFileName(AvatarFile.FileName)}";
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
                    return View();
                }
            }
            else
            {
                return View();
            }


        }
        public IActionResult Update(int IdUsuario)
        {
            Usuario u = repositorio.UsuarioId(IdUsuario);
            if (u == null)
            {
                return NotFound();
            }
            return View(u);
        }
        [HttpPost]
        public IActionResult Update(Usuario u)
        {
            if (ModelState.IsValid)
            {
                int res = repositorio.Modificacion(u);
                if (res > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "No se pudo modificar el Usuario";
                    return View();
                }
            }
            else
            {
                return View();
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
