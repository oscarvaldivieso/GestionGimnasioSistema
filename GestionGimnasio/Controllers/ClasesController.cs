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
    public class ClasesController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public ClasesController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Clases
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Clases";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Clases";

            var gestionGimnasioContext = _context.tbClases.Include(t => t.Emple).Include(t => t.Gimna).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X => X.Clase_Estado == true).ToListAsync());
        }

        // GET: Clases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Clases";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Clases";

            if (id == null)
            {
                return NotFound();
            }

            var tbClases = await _context.tbClases
                .Include(t => t.Emple)
                .Include(t => t.Gimna)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Clase_Id == id);
            if (tbClases == null)
            {
                return NotFound();
            }

            return View(tbClases);
        }

        // GET: Clases/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Clases";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Clases";

            var empleados = _context.tbEmpleados.Select(e => new
            {
                e.Emple_Id,
                NombreCompleto = e.Emple_PrimerNombre + " " + e.Emple_SegundoNombre + " " + e.Emple_PrimerApellido + " " + e.Emple_SegundoApellido
            }).ToList();

            

            ViewData["Emple_Id"] = new SelectList(empleados, "Emple_Id", "NombreCompleto");
            var gimnasio = _context.tbGimnasios.Where(c => c.Gimna_Estado == true).ToList();
            ViewData["Gimna_Id"] = new SelectList(gimnasio, "Gimna_Id", "Gimna_Nombre");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Clases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Clase_Id,Emple_Id,Gimna_Id,Clase_Nombre,Clase_Descripcion,Clase_Fecha,Clase_HoraInicio,Clase_HoraFin,Clase_Capacidad,Clase_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbClases tbClases)
        {
            ModelState.Remove("Clase_Estado");
            ModelState.Remove("Usuar_CreacionNavigation");
            ModelState.Remove("Usuar_Modificacion");
            ModelState.Remove("Usuar_ModificacionNavigation");


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Clases_Insertar @emp_id, @gim_id, @name, @description, @schedule, @starthour, @finish_hour,@capacity, @usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@emp_id", tbClases.Emple_Id),
                                                     new SqlParameter("@gim_id", tbClases.Gimna_Id),
                                                     new SqlParameter("@name", tbClases.Clase_Nombre),
                                                     new SqlParameter("@description", tbClases.Clase_Descripcion),
                                                     new SqlParameter("@schedule", tbClases.Clase_Fecha),
                                                     new SqlParameter("@starthour", tbClases.Clase_HoraInicio),
                                                     new SqlParameter("@finish_hour", tbClases.Clase_HoraFin),
                                                     new SqlParameter("@capacity", tbClases.Clase_Capacidad),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Clase creada correctamente";
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
            ViewData["Emple_Id"] = new SelectList(_context.tbEmpleados, "Emple_Id", "Emple_Direccion", tbClases.Emple_Id);
            ViewData["Gimna_Id"] = new SelectList(_context.tbGimnasios, "Gimna_Id", "Gimna_Direccion", tbClases.Gimna_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClases.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClases.Usuar_Modificacion);
            return View(tbClases);
        }

        // GET: Clases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Clases";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Clases";

            var empleados = _context.tbEmpleados.Select(e => new
            {
                e.Emple_Id,
                NombreCompleto = e.Emple_PrimerNombre + " " + e.Emple_SegundoNombre + " " + e.Emple_PrimerApellido + " " + e.Emple_SegundoApellido
            }).ToList();


            if (id == null)
            {
                return NotFound();
            }

            var tbClases = await _context.tbClases.FindAsync(id);
            if (tbClases == null)
            {
                return NotFound();
            }
            ViewData["Emple_Id"] = new SelectList(empleados, "Emple_Id", "NombreCompleto");
            var gimnasio = _context.tbGimnasios.Where(c => c.Gimna_Estado == true).ToList();
            ViewData["Gimna_Id"] = new SelectList(gimnasio, "Gimna_Id", "Gimna_Nombre");
            //ViewData["Gimna_Id"] = new SelectList(_context.tbGimnasios, "Gimna_Id", "Gimna_Nombre", tbClases.Gimna_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClases.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClases.Usuar_Modificacion);
            return View(tbClases);
        }

        // POST: Clases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Clase_Id,Emple_Id,Gimna_Id,Clase_Nombre,Clase_Descripcion,Clase_Fecha,Clase_HoraInicio,Clase_HoraFin,Clase_Capacidad,Clase_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbClases tbClases)
        {
            ModelState.Remove("Clase_Estado");
            ModelState.Remove("Usuar_CreacionNavigation");
            ModelState.Remove("Usuar_Creacion");
            ModelState.Remove("Usuar_CreacionNavigation");

            if (id != tbClases.Clase_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Clases_Actualizar @id, @emp_id, @gim_id, @name, @description, @schedule, @starthour, @finish_hour,@capacity, @usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@id", tbClases.Clase_Id),
                                                    new SqlParameter("@emp_id", tbClases.Emple_Id),
                                                     new SqlParameter("@gim_id", tbClases.Gimna_Id),
                                                     new SqlParameter("@name", tbClases.Clase_Nombre),
                                                     new SqlParameter("@description", tbClases.Clase_Descripcion),
                                                     new SqlParameter("@schedule", tbClases.Clase_Fecha),
                                                     new SqlParameter("@starthour", tbClases.Clase_HoraInicio),
                                                     new SqlParameter("@finish_hour", tbClases.Clase_HoraFin),
                                                     new SqlParameter("@capacity", tbClases.Clase_Capacidad),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbClasesExists(tbClases.Clase_Id))
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
                TempData["MensajeExito"] = "Clase editada correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Emple_Id"] = new SelectList(_context.tbEmpleados, "Emple_Id", "Emple_Direccion", tbClases.Emple_Id);
            var gimnasio = _context.tbGimnasios.Where(c => c.Gimna_Estado == true).ToList();
            ViewData["Gimna_Id"] = new SelectList(gimnasio, "Gimna_Id", "Gimna_Nombre", tbClases.Gimna_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClases.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClases.Usuar_Modificacion);
            return View(tbClases);
        }

        // GET: Clases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC Gnral.SP_Clases_Eliminar @code",
                             new SqlParameter("@code", id));
            }
            catch (DbUpdateException)
            {
                TempData["MensajeError"] = "Error al guardar los datos. Verifique la información.";
            }
            catch (SqlException ex) when (ex.Number == 547) // Error de restricción de clave foránea
            {
                TempData["MensajeError"] = "No se puede eliminar la clase porque está siendo utilizada en otras partes del sistema.";
            }
            catch (SqlException)
            {
                TempData["MensajeError"] = "No se puede eliminar el registro";
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Ocurrió un error inesperado.";
            }
            TempData["MensajeExito"] = "Clase eliminada correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Clases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbClases = await _context.tbClases.FindAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool tbClasesExists(int id)
        {
            return _context.tbClases.Any(e => e.Clase_Id == id);
        }
    }
}
