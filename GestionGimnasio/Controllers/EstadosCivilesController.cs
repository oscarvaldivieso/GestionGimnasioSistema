using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using Microsoft.Data.SqlClient;
using GestionGimnasio.Filters;

namespace GestionGimnasio.Controllers
{
    [AuthFilter]
    public class EstadosCivilesController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public EstadosCivilesController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: EstadosCiviles
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Estados Civiles";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Estados Civiles";
            var gestionGimnasioContext = _context.tbEstadosCiviles.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation).Where(x => x.EsCiv_Estado == true);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: EstadosCiviles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbEstadosCiviles = await _context.tbEstadosCiviles
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.EsCiv_Id == id);
            if (tbEstadosCiviles == null)
            {
                return NotFound();
            }

            return View(tbEstadosCiviles);
        }

        // GET: EstadosCiviles/Create
        public IActionResult Create()
        {
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: EstadosCiviles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EsCiv_Id,EsCiv_Descripcion,EsCiv_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEstadosCiviles tbEstadosCiviles)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_EstadosCiviles_Insertar @descripcion, @usercreatedAt, @datecreatedAt",
                        new SqlParameter("@descripcion", tbEstadosCiviles.EsCiv_Descripcion),
                        new SqlParameter("@usercreatedAt", 1),
                        new SqlParameter("@datecreatedAt", DateTime.Now)
                    );
                    TempData["MensajeExito"] = "Estado Civil creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    TempData["MensajeError"] = "Error al guardar los datos. Verifique la información.";
                }
                catch (SqlException)
                {
                    TempData["MensajeError"] = "No se puede insertar el registro.";
                }
                catch (Exception)
                {
                    TempData["MensajeError"] = "Ocurrió un error inesperado.";
                }
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEstadosCiviles.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEstadosCiviles.Usuar_Modificacion);
            return RedirectToAction(nameof(Index));
        }

        // GET: EstadosCiviles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbEstadosCiviles = await _context.tbEstadosCiviles.FindAsync(id);
            if (tbEstadosCiviles == null)
            {
                return NotFound();
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEstadosCiviles.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEstadosCiviles.Usuar_Modificacion);
            return PartialView("_Edit", tbEstadosCiviles);
            return View(tbEstadosCiviles);
        }

        // POST: EstadosCiviles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EsCiv_Id,EsCiv_Descripcion,EsCiv_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEstadosCiviles tbEstadosCiviles)
        {
            if (id != tbEstadosCiviles.EsCiv_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_EstadosCiviles_Actualizar @id, @descripcion, @userupdateAt, @dateupdateAt",
                        new SqlParameter("@id", tbEstadosCiviles.EsCiv_Id),
                        new SqlParameter("@descripcion", tbEstadosCiviles.EsCiv_Descripcion),
                        new SqlParameter("@userupdateAt", 1),
                        new SqlParameter("@dateupdateAt", DateTime.Now)
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbEstadosCivilesExists(tbEstadosCiviles.EsCiv_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    TempData["MensajeError"] = "Error al guardar los datos. Verifique la información.";
                }
                catch (SqlException)
                {
                    TempData["MensajeError"] = "No se puede editar el registro";
                }
                catch (Exception)
                {
                    TempData["MensajeError"] = "Ocurrió un error inesperado.";
                }
                TempData["MensajeExito"] = "Estado Civil editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEstadosCiviles.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEstadosCiviles.Usuar_Modificacion);
            return View(tbEstadosCiviles);
        }

        // GET: EstadosCiviles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                _context.Database.ExecuteSqlRaw("Gnral.SP_EstadosCiviles_Eliminar @id",
                    new SqlParameter("@id", id)
                );

            }
            catch (DbUpdateException)
            {
                TempData["MensajeError"] = "Error al guardar los datos. Verifique la información.";
            }
            catch (SqlException ex) when (ex.Number == 547) // Error de restricción de clave foránea
            {
                TempData["MensajeError"] = "No se puede eliminar el Estado Civil porque está siendo utilizado en otras partes del sistema.";
            }
            catch (SqlException)
            {
                TempData["MensajeError"] = "No se puede eliminar el registro";
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Ocurrió un error inesperado.";
            }
            TempData["MensajeExito"] = "Estado Civil eliminado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: EstadosCiviles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbEstadosCiviles = await _context.tbEstadosCiviles.FindAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool tbEstadosCivilesExists(int id)
        {
            return _context.tbEstadosCiviles.Any(e => e.EsCiv_Id == id);
        }
    }
}
