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
    public class GimnasiosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public GimnasiosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Gimnasios
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Gimnasios";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Gimnasios";

            var gestionGimnasioContext = _context.tbGimnasios.Include(t => t.Munic_CodigoNavigation).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X => X.Gimna_Estado == true).ToListAsync());
        }

        // GET: Gimnasios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Gimnasios";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Gimnasios > Detalles";

            if (id == null)
            {
                return NotFound();
            }

            var tbGimnasios = await _context.tbGimnasios
                .Include(t => t.Munic_CodigoNavigation)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Gimna_Id == id);
            if (tbGimnasios == null)
            {
                return NotFound();
            }

            return View(tbGimnasios);
        }

        // GET: Gimnasios/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Gimnasios";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Gimnasios > Crear";

            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Gimnasios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Gimna_Id,Gimna_Nombre,Gimna_SemanaHoraApertura,Gimna_SemanaHoraCierre,Gimna_FinDeHoraApertura,Gimna_FinDeHoraCierre,Gimna_Direccion,Munic_Codigo,Gimna_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbGimnasios tbGimnasios)
        {
            ModelState.Remove("Gimna_Estado");
            ModelState.Remove("Usuar_CreacionNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Usuar_Modificacion");
            ModelState.Remove("Usuar_ModificacionNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gimna.SP_Gimnasio_Insertar @name, @semanahoraapertura, @semanahoracierre, @findehoraapertura, @findehoracierre, @direccion, @munic_codigo,@usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@name", tbGimnasios.Gimna_Nombre),
                                                     new SqlParameter("@semanahoraapertura", tbGimnasios.Gimna_SemanaHoraApertura),
                                                     new SqlParameter("@semanahoracierre", tbGimnasios.Gimna_SemanaHoraCierre),
                                                     new SqlParameter("@findehoraapertura", tbGimnasios.Gimna_SemanaHoraApertura),
                                                     new SqlParameter("@findehoracierre", tbGimnasios.Gimna_SemanaHoraCierre),
                                                     new SqlParameter("@direccion", tbGimnasios.Gimna_Direccion),
                                                     new SqlParameter("@munic_codigo", tbGimnasios.Munic_Codigo),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Gimnasio creado correctamente";
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
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbGimnasios.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Modificacion);
            return View(tbGimnasios);
        }


        

        // GET: Gimnasios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Gimnasios";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Gimnasios > Editar";

            if (id == null)
            {
                return NotFound();
            }

            var tbGimnasios = await _context.tbGimnasios.FindAsync(id);
            if (tbGimnasios == null)
            {
                return NotFound();
            }
            var municipioEmpleado = await _context.tbMunicipios.FindAsync(tbGimnasios.Munic_Codigo);
            var departamentoEmpleado = await _context.tbDepartamentos.FindAsync(municipioEmpleado.Depar_Codigo);
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion", departamentoEmpleado.Depar_Codigo);
            var municipiosFiltrados = _context.tbMunicipios.Where(m => m.Depar_Codigo == departamentoEmpleado.Depar_Codigo).ToList();
            ViewData["Munic_Codigo"] = new SelectList(municipiosFiltrados, "Munic_Codigo", "Munic_Nombre", tbGimnasios.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Modificacion);
            return View(tbGimnasios);
        }

        // POST: Gimnasios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Gimna_Id,Gimna_Nombre,Gimna_SemanaHoraApertura,Gimna_SemanaHoraCierre,Gimna_FinDeHoraApertura,Gimna_FinDeHoraCierre,Gimna_Direccion,Munic_Codigo,Gimna_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbGimnasios tbGimnasios)
        {
            ModelState.Remove("Gimna_Estado");
            ModelState.Remove("Usuar_CreacionNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Usuar_Creacion");
            ModelState.Remove("Usuar_CreacionNavigation");

            if (id != tbGimnasios.Gimna_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                try
                {
                    _context.Database.ExecuteSqlRaw("Gimna.SP_Gimnasio_Actualizar @code, @name, @semanahoraapertura, @semanahoracierre, @findehoraapertura,@findehoracierre,@direccion,@munic_codigo,@usermodified, @datemodifiedAt",
                                                new SqlParameter("@code", tbGimnasios.Gimna_Id),
                                                new SqlParameter("@name", tbGimnasios.Gimna_Nombre),
                                                     new SqlParameter("@semanahoraapertura", tbGimnasios.Gimna_SemanaHoraApertura),
                                                     new SqlParameter("@semanahoracierre", tbGimnasios.Gimna_SemanaHoraCierre),
                                                     new SqlParameter("@findehoraapertura", tbGimnasios.Gimna_FinDeHoraApertura),
                                                     new SqlParameter("@findehoracierre", tbGimnasios.Gimna_FinDeHoraCierre),
                                                     new SqlParameter("@direccion", tbGimnasios.Gimna_Direccion),
                                                     new SqlParameter("@munic_codigo", tbGimnasios.Munic_Codigo),
                                                     new SqlParameter("@usermodified", 1),
                                                     new SqlParameter("@datemodifiedAt", DateTime.Now));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbGimnasiosExists(tbGimnasios.Gimna_Id))
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
                TempData["MensajeExito"] = "Gimnasio editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbGimnasios.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Modificacion);
            return View(tbGimnasios);
        }

        // GET: Gimnasios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC Gimna.SP_Gimnasio_Eliminar @code",
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

        // POST: Gimnasios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbGimnasios = await _context.tbGimnasios.FindAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool tbGimnasiosExists(int id)
        {
            return _context.tbGimnasios.Any(e => e.Gimna_Id == id);
        }

        [HttpPost]
        public IActionResult CargarMunicipio(string? cod)
        {
            var ddl = _context.tbMunicipios.FromSqlRaw("Gnral.SP_MunicipioPorDepartamento @depar_codigo",
                                new SqlParameter("@depar_codigo", cod)).ToList();
            return Ok(ddl);
        }
    }
}
