using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis.Scripting;
using GestionGimnasio.Filters;

namespace GestionGimnasio.Controllers
{
    [AuthFilter]
    public class MunicipiosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public MunicipiosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Municipios
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Municipios";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Municipios";
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion");
            var gestionGimnasioContext = _context.tbMunicipios.Include(t => t.Depar_CodigoNavigation).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: Municipios/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMunicipios = await _context.tbMunicipios
                .Include(t => t.Depar_CodigoNavigation)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Munic_Codigo == id);
            if (tbMunicipios == null)
            {
                return NotFound();
            }

            return View(tbMunicipios);
        }

        // GET: Municipios/Create
        public IActionResult Create()
        {
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Codigo");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Municipios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Munic_Codigo,Munic_Nombre,Depar_Codigo,Munic_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbMunicipios tbMunicipios)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Municipio_Insertar @code, @name, @depa, @usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@code", tbMunicipios.Munic_Codigo),
                                                     new SqlParameter("@name", tbMunicipios.Munic_Nombre),
                                                     new SqlParameter("@depa", tbMunicipios.Depar_Codigo),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Municipio creado correctamente";
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
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Codigo", tbMunicipios.Depar_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMunicipios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMunicipios.Usuar_Modificacion);
            return RedirectToAction(nameof(Index));
        }

        // GET: Municipios/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMunicipios = await _context.tbMunicipios.FindAsync(id);
            if (tbMunicipios == null)
            {
                return NotFound();
            }
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion", tbMunicipios.Depar_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMunicipios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMunicipios.Usuar_Modificacion);
            return PartialView("_Edit", tbMunicipios);
            return View(tbMunicipios);
        }

        // POST: Municipios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Munic_Codigo,Munic_Nombre,Depar_Codigo,Munic_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbMunicipios tbMunicipios)
        {
            if (id != tbMunicipios.Munic_Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Municipio_Actualizar @code, @name, @depa, @userModifAt, @dateModifAt",
                                                new SqlParameter("@code", tbMunicipios.Munic_Codigo),
                                                 new SqlParameter("@name", tbMunicipios.Munic_Nombre),
                                                 new SqlParameter("@depa", tbMunicipios.Depar_Codigo),
                                                 new SqlParameter("@userModifAt", 1),
                                                 new SqlParameter("@dateModifAt", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbMunicipiosExists(tbMunicipios.Munic_Codigo))
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
                TempData["MensajeExito"] = "Municipio editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Codigo", tbMunicipios.Depar_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMunicipios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbMunicipios.Usuar_Modificacion);
            return View(tbMunicipios);
        }

        // GET: Municipios/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _context.Database.ExecuteSqlRaw("Gnral.SP_Municipio_Eliminar @code",
                                                    new SqlParameter("@code", id));
                TempData["MensajeExito"] = "Municipio eliminado correctamente";
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

        // POST: Municipios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            return RedirectToAction(nameof(Index));
        }

        private bool tbMunicipiosExists(string id)
        {
            return _context.tbMunicipios.Any(e => e.Munic_Codigo == id);
        }
    }
}
