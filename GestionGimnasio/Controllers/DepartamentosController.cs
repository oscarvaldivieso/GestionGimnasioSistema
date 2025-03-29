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
    public class DepartamentosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public DepartamentosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Departamentos
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Departamentos";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Departamentos";

            var gestionGimnasioContext = _context.tbDepartamentos.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X=>X.Depar_Estado == true).ToListAsync());
        }

        // GET: Departamentos/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbDepartamentos = await _context.tbDepartamentos
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Depar_Codigo == id);
            if (tbDepartamentos == null)
            {
                return NotFound();
            }

            return View(tbDepartamentos);
        }

        // GET: Departamentos/Create
        public IActionResult Create()
        {
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Departamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Depar_Codigo,Depar_Descripcion,Depar_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbDepartamentos tbDepartamentos)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Departamento_Insertar @code, @name, @usercreatedAt, @datecreatedAt",
                                                new SqlParameter("@code", tbDepartamentos.Depar_Codigo),
                                                 new SqlParameter("@name", tbDepartamentos.Depar_Descripcion),
                                                 new SqlParameter("@usercreatedAt", 1),
                                                 new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Departamento creado correctamente";
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
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbDepartamentos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbDepartamentos.Usuar_Modificacion);
            return RedirectToAction(nameof(Index));
        }

        // GET: Departamentos/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbDepartamentos = await _context.tbDepartamentos.FindAsync(id);
            if (tbDepartamentos == null)
            {
                return NotFound();
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbDepartamentos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbDepartamentos.Usuar_Modificacion);
            return PartialView("_Edit", tbDepartamentos);
        }

        // POST: Departamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Depar_Codigo,Depar_Descripcion,Depar_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbDepartamentos tbDepartamentos)
        {
            if (id != tbDepartamentos.Depar_Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Departamento_Actualizar @code, @department, @userModified, @dateModifiedAt",
                         new SqlParameter("@code", tbDepartamentos.Depar_Codigo),
                         new SqlParameter("@department", tbDepartamentos.Depar_Descripcion),
                         new SqlParameter("@userModified", 1),
                         new SqlParameter("@dateModifiedAt", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbDepartamentosExists(tbDepartamentos.Depar_Codigo))
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
                TempData["MensajeExito"] = "Departamento editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbDepartamentos.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbDepartamentos.Usuar_Modificacion);
            return View(tbDepartamentos);
        }

        // GET: Departamentos/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC Gnral.SP_Departamento_Eliminar @code",
                             new SqlParameter("@code", id));
                TempData["MensajeExito"] = "Departamento eliminado correctamente";
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

            
            return RedirectToAction(nameof(Index));
        }

        // POST: Departamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tbDepartamentos = await _context.tbDepartamentos.FindAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private bool tbDepartamentosExists(string id)
        {
            return _context.tbDepartamentos.Any(e => e.Depar_Codigo == id);
        }
    }
}
