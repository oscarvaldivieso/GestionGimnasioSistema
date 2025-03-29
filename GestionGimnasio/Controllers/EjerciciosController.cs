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
    public class EjerciciosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public EjerciciosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Ejercicios
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Ejercicios";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Ejercicios";
            var gestionGimnasioContext = _context.tbEjercicios.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X => X.Ejerc_Estado == true).ToListAsync());
        }

        // GET: Ejercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbEjercicios = await _context.tbEjercicios
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Ejerc_Id == id);
            if (tbEjercicios == null)
            {
                return NotFound();
            }

            return View(tbEjercicios);
        }

        // GET: Ejercicios/Create
        public IActionResult Create()
        {
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Ejercicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ejerc_Id,Ejerc_Nombre,Ejerc_Descripcion,Ejerc_MusculoPrincipal,Ejerc_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEjercicios tbEjercicios)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gimna.SP_Ejercicios_Insertar @name, @description, @mainMuscle, @usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@name", tbEjercicios.Ejerc_Nombre),
                                                     new SqlParameter("@description", tbEjercicios.Ejerc_Descripcion),
                                                     new SqlParameter("@mainMuscle", tbEjercicios.Ejerc_MusculoPrincipal),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Ejercicio creado correctamente";
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
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjercicios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjercicios.Usuar_Modificacion);
            return RedirectToAction(nameof(Index));
        }

        // GET: Ejercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbEjercicios = await _context.tbEjercicios.FindAsync(id);
            if (tbEjercicios == null)
            {
                return NotFound();
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjercicios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjercicios.Usuar_Modificacion);
            return PartialView("_Edit",tbEjercicios);
        }

        // POST: Ejercicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ejerc_Id,Ejerc_Nombre,Ejerc_Descripcion,Ejerc_MusculoPrincipal,Ejerc_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEjercicios tbEjercicios)
        {
            if (id != tbEjercicios.Ejerc_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gimna.SP_Ejercicios_Actualizar @code, @name, @description, @mainMuscle, @usercreatedAt, @datecreatedAt",
                                                new SqlParameter("@code", tbEjercicios.Ejerc_Id),
                                                new SqlParameter("@name", tbEjercicios.Ejerc_Nombre),
                                                 new SqlParameter("@description", tbEjercicios.Ejerc_Descripcion),
                                                 new SqlParameter("@mainMuscle", tbEjercicios.Ejerc_MusculoPrincipal),
                                                 new SqlParameter("@usercreatedAt", 1),
                                                 new SqlParameter("@datecreatedAt", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbEjerciciosExists(tbEjercicios.Ejerc_Id))
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
                TempData["MensajeExito"] = "Ejercicio editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjercicios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjercicios.Usuar_Modificacion);
            return View(tbEjercicios);
        }

        // GET: Ejercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC Gimna.SP_Ejercicios_Eliminar @code",
                             new SqlParameter("@code", id));
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
            TempData["MensajeExito"] = "Ejercicio eliminado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Ejercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbEjercicios = await _context.tbEjercicios.FindAsync(id);
            
            return RedirectToAction(nameof(Index));
        }

        private bool tbEjerciciosExists(int id)
        {
            return _context.tbEjercicios.Any(e => e.Ejerc_Id == id);
        }
    }
}
