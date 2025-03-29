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
    public class CargosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public CargosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Cargos
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Cargos";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Cargos";
            var gestionGimnasioContext = _context.tbCargos.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation).Where(x=>x.Cargo_Estado==true);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: Cargos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCargos = await _context.tbCargos
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Cargo_Id == id);
            if (tbCargos == null)
            {
                return NotFound();
            }

            return View(tbCargos);
        }

        // GET: Cargos/Create
        public IActionResult Create()
        {
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Cargos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cargo_Id,Cargo_Nombre,Cargo_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbCargos tbCargos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Cargo_Insertar @name, @usercreatedAt, @datecreatedAt",
                        new SqlParameter("@name", tbCargos.Cargo_Nombre),
                        new SqlParameter("@usercreatedAt", 1),
                        new SqlParameter("@datecreatedAt", DateTime.Now)
                    );
                    TempData["MensajeExito"] = "Cargo creado correctamente";
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
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbCargos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbCargos.Usuar_Modificacion);
            return RedirectToAction(nameof(Index));
        }

        // GET: Cargos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCargos = await _context.tbCargos.FindAsync(id);
            if (tbCargos == null)
            {
                return NotFound();
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbCargos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbCargos.Usuar_Modificacion);
            return PartialView("_Edit", tbCargos);
            return View(tbCargos);
        }

        // POST: Cargos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Cargo_Id,Cargo_Nombre,Cargo_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbCargos tbCargos)
        {
            if (id != tbCargos.Cargo_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Cargo_Actualizar @id, @name, @userModifiedAt, @dateModifiedAt",
                        new SqlParameter("@id", tbCargos.Cargo_Id),
                        new SqlParameter("@name", tbCargos.Cargo_Nombre),
                        new SqlParameter("@userModifiedAt", 1),
                        new SqlParameter("@dateModifiedAt", DateTime.Now)
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbCargosExists(tbCargos.Cargo_Id))
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
                TempData["MensajeExito"] = "Cargo editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbCargos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbCargos.Usuar_Modificacion);
            return View(tbCargos);
        }

        // GET: Cargos/Delete/5
        public async Task<IActionResult> Delete(int? id, tbEmpleados tbEmpleados)
        {
            try
            {
                var cargoexiste = _context.tbEmpleados.Where(x => x.Cargo_Id == id).ToList().FirstOrDefault();
                if (cargoexiste!=null)
                {
                    TempData["MensajeError"] = "No se puede eliminar el ejercicio porque está siendo utilizado en otras partes del sistema.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Database.ExecuteSqlRaw("Gnral.SP_Cargo_Eliminar @id",
                    new SqlParameter("@id", id)
                );
                TempData["MensajeExito"] = "Cargo eliminado correctamente";
            }
            catch (DbUpdateException)
            {
                TempData["MensajeError"] = "Error al guardar los datos. Verifique la información.";
            }
            catch (SqlException ex) when (ex.Number == 547) // Error de restricción de clave foránea
            {
                TempData["MensajeError"] = "No se puede eliminar el ejercicio porque está siendo utilizado en otras partes del sistema.";
            }
            catch (SqlException)
            {
                TempData["MensajeError"] = "No se puede eliminar el registro";
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Ocurrió un error inesperado.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Cargos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbCargos = await _context.tbCargos.FindAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool tbCargosExists(int id)
        {
            return _context.tbCargos.Any(e => e.Cargo_Id == id);
        }
    }
}
