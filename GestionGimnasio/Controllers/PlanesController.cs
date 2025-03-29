using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using Microsoft.Data.SqlClient;

namespace GestionGimnasio.Controllers
{
    public class PlanesController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public PlanesController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Planes
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Planes";
            ViewData["ActiveParent"] = "Pagps";

            ViewBag.PageTitle = "Planes";

            var gestionGimnasioContext = _context.tbPlanes.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation).Where(x=>x.Plane_Estado==true);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: Planes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Planes";
            ViewData["ActiveParent"] = "Pagps";

            ViewBag.PageTitle = "Planes > Detalles";

            if (id == null)
            {
                return NotFound();
            }

            var tbPlanes = await _context.tbPlanes
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Plane_Id == id);
            if (tbPlanes == null)
            {
                return NotFound();
            }

            return View(tbPlanes);
        }

        // GET: Planes/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Planes";
            ViewData["ActiveParent"] = "Pagps";

            ViewBag.PageTitle = "Planes > Crear";

            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Planes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Plane_Id,Plane_Nombre,Plane_Descripcion,Plane_DuracionDias,Plane_Precio,Plane_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbPlanes tbPlanes)
        {
            if (ModelState.IsValid)
            {
                _context.Database.ExecuteSqlRaw("Gnral.SP_Planes_Insertar @Plane_Nombre, @Plane_Descripcion, @Plane_DuracionDias, @Plane_Precio, @Usuar_Creacion, @Fecha_Creacion",
                    new SqlParameter("@Plane_Nombre", tbPlanes.Plane_Nombre),
                    new SqlParameter("@Plane_Descripcion", tbPlanes.Plane_Descripcion),
                    new SqlParameter("@Plane_DuracionDias", tbPlanes.Plane_DuracionDias),
                    new SqlParameter("@Plane_Precio", tbPlanes.Plane_Precio),
                    new SqlParameter("@Usuar_Creacion", 1),
                    new SqlParameter("@Fecha_Creacion", DateTime.Now)
                );
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPlanes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPlanes.Usuar_Modificacion);
            return View(tbPlanes);
        }

        // GET: Planes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Planes";
            ViewData["ActiveParent"] = "Pagps";

            ViewBag.PageTitle = "Planes > Editar";

            if (id == null)
            {
                return NotFound();
            }

            var tbPlanes = await _context.tbPlanes.FindAsync(id);
            if (tbPlanes == null)
            {
                return NotFound();
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPlanes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPlanes.Usuar_Modificacion);
            return PartialView("_Edit", tbPlanes);
            return View(tbPlanes);
        }

        // POST: Planes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Plane_Id,Plane_Nombre,Plane_Descripcion,Plane_DuracionDias,Plane_Precio,Plane_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbPlanes tbPlanes)
        {
            if (id != tbPlanes.Plane_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Planes_Actualizar @Plane_Id, @Plane_Nombre, @Plane_Descripcion, @Plane_DuracionDias, @Plane_Precio, @Usuar_Modificacion, @Fecha_Modificacion",
                        new SqlParameter("@Plane_Id", tbPlanes.Plane_Id),
                        new SqlParameter("@Plane_Nombre", tbPlanes.Plane_Nombre),
                        new SqlParameter("@Plane_Descripcion", tbPlanes.Plane_Descripcion),
                        new SqlParameter("@Plane_DuracionDias", tbPlanes.Plane_DuracionDias),
                        new SqlParameter("@Plane_Precio", tbPlanes.Plane_Precio),
                        new SqlParameter("@Usuar_Modificacion", 1),
                        new SqlParameter("@Fecha_Modificacion", DateTime.Now)
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbPlanesExists(tbPlanes.Plane_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPlanes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPlanes.Usuar_Modificacion);
            return View(tbPlanes);
        }

        // GET: Planes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _context.Database.ExecuteSqlRaw("Gnral.SP_Planes_Eliminar @Plane_Id",
                new SqlParameter("@Plane_Id", id)
            );
            return RedirectToAction(nameof(Index));
        }

        // POST: Planes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbPlanes = await _context.tbPlanes.FindAsync(id);
            if (tbPlanes != null)
            {
                _context.tbPlanes.Remove(tbPlanes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbPlanesExists(int id)
        {
            return _context.tbPlanes.Any(e => e.Plane_Id == id);
        }
    }
}
