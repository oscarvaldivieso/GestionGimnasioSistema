using GestionGimnasio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionGimnasio.Controllers
{
    public class RutinasClientesEjercicios : Controller
    {
        private readonly GestionGimnasioContext _context;

        public RutinasClientesEjercicios(GestionGimnasioContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Rutinas por Cliente";
            ViewData["ActiveParent"] = "Gimnasio";
            ViewBag.PageTitle = "Rutinas por cliente";

            var gestionGimnasioContext = _context.tbClientes.Include(t => t.EsCiv).Include(t => t.Munic_CodigoNavigation).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X => X.Clien_Estado == true).ToListAsync());
        }


        public async Task<IActionResult> AsignarRutinas()
        {
            return View();
        }
    }
}
