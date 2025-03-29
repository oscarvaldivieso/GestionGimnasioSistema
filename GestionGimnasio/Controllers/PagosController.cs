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
    public class PagosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public PagosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Pagos
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Pagos";
            ViewData["ActiveParent"] = "Pagos";

            ViewBag.PageTitle = "Pagos";

            var clientes = _context.tbClientes.Where(x=>x.Clien_Estado==true);
            ViewData["Clien_Id"] = new SelectList(clientes, "Clien_id", "Clien_Identidad");
            var metodosDePago = _context.tbMetodosDePago.Where(x => x.MetPa_Estado == true);
            ViewData["MetPa_Id"] = new SelectList(metodosDePago, "MetPa_Id", "MetPa_Descripcion");
            var planes = _context.tbPlanes.Where(x => x.Plane_Estado == true);
            ViewData["Plane_Id"] = new SelectList(planes, "Plane_Id", "Plane_Nombre");
            var gestionGimnasioContext = _context.tbPagos.Include(t => t.Clien).Include(t => t.MetPa).Include(t => t.Plane).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation).Where(p=>p.Pagos_Estado==true);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Pagos";
            ViewData["ActiveParent"] = "Pagos";

            ViewBag.PageTitle = "Pagos > Detalles";

            if (id == null)
            {
                return NotFound();
            }

            var tbPagos = await _context.tbPagos
                .Include(t => t.Clien)
                .Include(t => t.MetPa)
                .Include(t => t.Plane)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Pagos_Id == id);
            if (tbPagos == null)
            {
                return NotFound();
            }

            return View(tbPagos);
        }

        // GET: Pagos/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Pagos";
            ViewData["ActiveParent"] = "Pagos";

            ViewBag.PageTitle = "Pagos > Crear";
            ViewData["Clien_Id"] = new SelectList(_context.tbClientes, "Clien_id", "Clien_Direccion");
            ViewData["MetPa_Id"] = new SelectList(_context.tbMetodosDePago, "MetPa_Id", "MetPa_Descripcion");
            ViewData["Plane_Id"] = new SelectList(_context.tbPlanes, "Plane_Id", "Plane_Descripcion");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Pagos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Pagos_Id,Clien_Id,Plane_Id,MetPa_Id,Pagos_Fecha,Pagos_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbPagos tbPagos)
        {
            if (ModelState.IsValid)
            {
                _context.Database.ExecuteSqlRaw("Gnral.SP_Pagos_Insertar @clie, @plane, @metodo, @pagofecha, @usua, @fechacreacion",
                                new SqlParameter("@clie", tbPagos.Clien_Id),
                                new SqlParameter("@plane", tbPagos.Plane_Id),
                                new SqlParameter("@metodo", tbPagos.MetPa_Id),
                                new SqlParameter("@pagofecha", tbPagos.Pagos_Fecha),
                                new SqlParameter("@usua", 1),
                                new SqlParameter("@fechacreacion", DateTime.Now));
                return RedirectToAction(nameof(Index));
            }
            ViewData["Clien_Id"] = new SelectList(_context.tbClientes, "Clien_id", "Clien_Direccion", tbPagos.Clien_Id);
            ViewData["MetPa_Id"] = new SelectList(_context.tbMetodosDePago, "MetPa_Id", "MetPa_Descripcion", tbPagos.MetPa_Id);
            ViewData["Plane_Id"] = new SelectList(_context.tbPlanes, "Plane_Id", "Plane_Descripcion", tbPagos.Plane_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPagos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPagos.Usuar_Modificacion);
            return View(tbPagos);
        }

        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Pagos";
            ViewData["ActiveParent"] = "Pagos";

            ViewBag.PageTitle = "Pagos > Editar";

            if (id == null)
            {
                return NotFound();
            }

            var tbPagos = await _context.tbPagos.FindAsync(id);
            if (tbPagos == null)
            {
                return NotFound();
            }
            ViewData["Clien_Id"] = new SelectList(_context.tbClientes, "Clien_id", "Clien_Direccion", tbPagos.Clien_Id);
            ViewData["MetPa_Id"] = new SelectList(_context.tbMetodosDePago, "MetPa_Id", "MetPa_Descripcion", tbPagos.MetPa_Id);
            ViewData["Plane_Id"] = new SelectList(_context.tbPlanes, "Plane_Id", "Plane_Descripcion", tbPagos.Plane_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPagos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPagos.Usuar_Modificacion);
            return View(tbPagos);
        }

        // POST: Pagos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Pagos_Id,Clien_Id,Plane_Id,MetPa_Id,Pagos_Fecha,Pagos_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbPagos tbPagos)
        {
            if (id != tbPagos.Pagos_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Pagos_Actualizar @id, @clie, @plane, @metodo, @pagofecha, @usua, @fechamodificacion",
                                new SqlParameter("@id", tbPagos.Pagos_Id),
                                new SqlParameter("@clie", tbPagos.Clien_Id),
                                new SqlParameter("@plane", tbPagos.Plane_Id),
                                new SqlParameter("@metodo", tbPagos.MetPa_Id),
                                new SqlParameter("@pagofecha", tbPagos.Pagos_Fecha),
                                new SqlParameter("@usua", 1),
                                new SqlParameter("@fechamodificacion", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbPagosExists(tbPagos.Pagos_Id))
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
            ViewData["Clien_Id"] = new SelectList(_context.tbClientes, "Clien_id", "Clien_Direccion", tbPagos.Clien_Id);
            ViewData["MetPa_Id"] = new SelectList(_context.tbMetodosDePago, "MetPa_Id", "MetPa_Descripcion", tbPagos.MetPa_Id);
            ViewData["Plane_Id"] = new SelectList(_context.tbPlanes, "Plane_Id", "Plane_Descripcion", tbPagos.Plane_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPagos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbPagos.Usuar_Modificacion);
            return View(tbPagos);
        }

        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _context.Database.ExecuteSqlRaw("Gnral.SP_Pagos_Eliminar @id",
                                new SqlParameter("@id", id));
            return RedirectToAction(nameof(Index));
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbPagos = await _context.tbPagos.FindAsync(id);
            if (tbPagos != null)
            {
                _context.tbPagos.Remove(tbPagos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbPagosExists(int id)
        {
            return _context.tbPagos.Any(e => e.Pagos_Id == id);
        }
    }
}
