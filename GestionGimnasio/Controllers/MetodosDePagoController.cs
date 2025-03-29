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
    public class MetodosDePagoController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public MetodosDePagoController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: MetodosDePago
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Metodos De Pago";
            ViewData["ActiveParent"] = "Pagos";

            ViewBag.PageTitle = "Metodos De Pago";
            var gestionGimnasioContext = _context.tbMetodosDePago.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X => X.MetPa_Estado == true).ToListAsync());
        }

        // GET: MetodosDePago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMetodosDePago = await _context.tbMetodosDePago
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.MetPa_Id == id);
            if (tbMetodosDePago == null)
            {
                return NotFound();
            }

            return View(tbMetodosDePago);
        }

        // GET: MetodosDePago/Create
        public IActionResult Create()
        {
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: MetodosDePago/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MetPa_Id,MetPa_Descripcion,MetPa_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbMetodosDePago tbMetodosDePago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_MetodosDePago_Insertar @method, @usercreatedAt, @datecreatedAt",
                                                     new SqlParameter("@method", tbMetodosDePago.MetPa_Descripcion),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Metodo de pago creado correctamente";
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
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMetodosDePago.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMetodosDePago.Usuar_Modificacion);
            return RedirectToAction(nameof(Index));
        }

        // GET: MetodosDePago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMetodosDePago = await _context.tbMetodosDePago.FindAsync(id);
            if (tbMetodosDePago == null)
            {
                return NotFound();
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMetodosDePago.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMetodosDePago.Usuar_Modificacion);
            return PartialView("_Edit", tbMetodosDePago);
        }

        // POST: MetodosDePago/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MetPa_Id,MetPa_Descripcion,MetPa_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbMetodosDePago tbMetodosDePago)
        {
            if (id != tbMetodosDePago.MetPa_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_MetodosDePago_Actualizar @code, @method, @userModified, @dateModifiedAt",
                         new SqlParameter("@code", tbMetodosDePago.MetPa_Id),
                         new SqlParameter("@method", tbMetodosDePago.MetPa_Descripcion),
                         new SqlParameter("@userModified", 1),
                         new SqlParameter("@dateModifiedAt", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbMetodosDePagoExists(tbMetodosDePago.MetPa_Id))
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMetodosDePago.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMetodosDePago.Usuar_Modificacion);
            return View(tbMetodosDePago);
        }

        // GET: MetodosDePago/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC Gnral.SP_MetodosDePago_Eliminar @code",
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

            return RedirectToAction(nameof(Index));
        }

        // POST: MetodosDePago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbMetodosDePago = await _context.tbMetodosDePago.FindAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool tbMetodosDePagoExists(int id)
        {
            return _context.tbMetodosDePago.Any(e => e.MetPa_Id == id);
        }
    }
}
