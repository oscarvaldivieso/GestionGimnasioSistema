using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionGimnasio.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GestionGimnasio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GestionGimnasioContext _context;

    public HomeController(ILogger<HomeController> logger, GestionGimnasioContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.PageTitle = "Home";
        return View();
    }

    // GET: /Home/Login -> Muestra la página de login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        //var InicioSesion = _context.tbUsuarios.FromSqlRaw("Acces.SP_Usuarios_InicioSesion @param1, @param2",
        //                        new SqlParameter("@param1", username),
        //                        new SqlParameter("@param2", password));

        //return View("Index");

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            TempData["MensajeError"] = "Debe llenar todos los campos";
            return RedirectToAction("Login");
        }

        try
        {
            var InicioSesion = _context.LoginOutput.FromSqlRaw("Acces.SP_Usuarios_InicioSesion @usuario, @contrasena",
                                        new SqlParameter("@usuario", username),
                                        new SqlParameter("@contrasena", password)).ToList();


            if (InicioSesion.Count > 0)
            {
                foreach (var item in InicioSesion)
                {
                    HttpContext.Session.SetInt32("Id", item.Id);
                    HttpContext.Session.SetString("Usuario", item.Usuario);
                    HttpContext.Session.SetString("Nombre_Empleado", item.Nombre_Empleado);
                    HttpContext.Session.SetInt32("Rol", item.Rol);
                    HttpContext.Session.SetInt32("Empleado_Id", item.Empleado_Id);
                    HttpContext.Session.SetString("Admin", item.Admin.ToString());
                }

                var idsSeleccionados = _context.PantallasRoles.FromSqlRaw(
               "Acces.SP_PantallasRoles @RoleId",
               new SqlParameter("@RoleId", HttpContext.Session.GetInt32("Rol"))
               ).ToList();

                string cadenaPantallas = "";
                foreach (var item in idsSeleccionados)
                {
                    cadenaPantallas += item.Pantalla + ",";
                }
                ViewData["Pantallas"] = idsSeleccionados;
                HttpContext.Session.SetString("Pantallas", cadenaPantallas);

                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensajeError"] = "Usuario o contraseña incorrectos";
            }
        }
        catch(SqlException)
        {
            TempData["MensajeError"] = "Usuario o contraseña incorrectos";
        }
        ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
        return View();

    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
