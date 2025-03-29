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
    public class RutinasController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public RutinasController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Rutinas
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Rutinas";
            ViewData["ActiveParent"] = "Gimnasio"; 

            ViewBag.PageTitle = "Rutinas";
            var gestionGimnasioContext = _context.tbRutinas.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X => X.Rutin_Estado == true).ToListAsync());
        }

        // GET: Rutinas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Rutinas";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Rutinas > Detalles";

            if (id == null)
            {
                return NotFound();
            }

            var tbRutinas = await _context.tbRutinas
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Rutin_Id == id);
            if (tbRutinas == null)
            {
                return NotFound();
            }

            return View(tbRutinas);
        }

        // GET: Rutinas/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Rutinas";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Rutinas > Crear";

            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Rutinas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rutin_Id,Rutin_Nombre,Rutin_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbRutinas tbRutinas)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gimna.SP_Rutinas_Insertar @name, @usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@name", tbRutinas.Rutin_Nombre),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Rutina creada correctamente";
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
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbRutinas.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbRutinas.Usuar_Modificacion);
            return RedirectToAction(nameof(Index));
        }

        // GET: Rutinas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Rutinas";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Rutinas > Editar";

            if (id == null)
            {
                return NotFound();
            }

            var tbRutinas = await _context.tbRutinas.FindAsync(id);
            if (tbRutinas == null)
            {
                return NotFound();
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbRutinas.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbRutinas.Usuar_Modificacion);
            return PartialView("_Edit",tbRutinas);
        }

        // POST: Rutinas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rutin_Id,Rutin_Nombre,Rutin_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbRutinas tbRutinas)
        {
            if (id != tbRutinas.Rutin_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gimna.SP_Rutinas_Actualizar @id, @name, @usercreatedAt, @datecreatedAt",
                                               new SqlParameter("@id", tbRutinas.Rutin_Id),
                                               new SqlParameter("@name", tbRutinas.Rutin_Nombre),
                                                new SqlParameter("@usercreatedAt", 1),
                                                new SqlParameter("@datecreatedAt", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbRutinasExists(tbRutinas.Rutin_Id))
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
                TempData["MensajeExito"] = "Rutina editada correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbRutinas.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbRutinas.Usuar_Modificacion);
            return View(tbRutinas);
        }

        // GET: Rutinas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC Gimna.SP_Rutinas_Eliminar @code",
                             new SqlParameter("@code", id));
            }
            catch (DbUpdateException)
            {
                TempData["MensajeError"] = "Error al guardar los datos. Verifique la información.";
            }
            catch (SqlException ex) when (ex.Number == 547) // Error de restricción de clave foránea
            {
                TempData["MensajeError"] = "No se puede eliminar el departamento porque está siendo utilizado en otras partes del sistema.";
            }
            catch (SqlException)
            {
                TempData["MensajeError"] = "No se puede eliminar el registro";
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Ocurrió un error inesperado.";
            }
            TempData["MensajeExito"] = "Rutina eliminada correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Rutinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbRutinas = await _context.tbRutinas.FindAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool tbRutinasExists(int id)
        {
            return _context.tbRutinas.Any(e => e.Rutin_Id == id);
        }
    }
}
