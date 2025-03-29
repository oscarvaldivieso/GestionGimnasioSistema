using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using GestionGimnasio.Filters;

namespace GestionGimnasio.Controllers
{
    [AuthFilter]
    public class RolesController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public RolesController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Roles";
            ViewData["ActiveParent"] = "Acceso";

            ViewBag.PageTitle = "Roles";

            var gestionGimnasioContext = _context.tbRoles.Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Roles";
            ViewData["ActiveParent"] = "Acceso";

            ViewBag.PageTitle = "Roles > Detalles";


            if (id == null)
            {
                return NotFound();
            }

            var tbRoles = await _context.tbRoles
            .Include(t => t.Usuar_CreacionNavigation)
            .Include(t => t.Usuar_ModificacionNavigation)
            .Include(t => t.tbRolesPorPantalla)
            .ThenInclude(rp => rp.Panta)
            .FirstOrDefaultAsync(m => m.Roles_Id == id);
            if (tbRoles == null)
            {
                return NotFound();
            }

            List<string> nombresPantalla = tbRoles.tbRolesPorPantalla.Select(rp => rp.Panta.Panta_Nombre).ToList();

            ViewBag.NombresPantalla = nombresPantalla;

            return View(tbRoles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Roles";
            ViewData["ActiveParent"] = "Acceso";

            ViewBag.PageTitle = "Roles > Crear";

            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            var pantallas = _context.tbPantallas.ToList();
            ViewBag.Pantallas = pantallas;
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Roles_Descripcion, string pantallasSeleccionadas)
        {
            try
            {
                _context.Database.ExecuteSqlRaw("Acces.SP_Rol_Insertar @role, @usua, @fecha",
                new SqlParameter("@role", Roles_Descripcion),
                new SqlParameter("@usua", 1),
                new SqlParameter("@fecha", DateTime.Now)
                );
                //Obtener el rol creado para asignarle las pantallas
                var nuevoRol = _context.tbRoles.OrderByDescending(x => x.Roles_Id).FirstOrDefault();

                if (!string.IsNullOrEmpty(pantallasSeleccionadas))
                {
                    var pantallasIds = JsonConvert.DeserializeObject<List<int>>(pantallasSeleccionadas);
                    foreach (var pantallaId in pantallasIds)
                    {
                        _context.Database.ExecuteSqlRaw("Acces.SP_RolesPorPantalla_Insertar @rol, @pantalla, @usua, @fecha",
                            new SqlParameter("@rol", nuevoRol.Roles_Id),
                            new SqlParameter("@pantalla", pantallaId),
                            new SqlParameter("@usua", 1),
                            new SqlParameter("@fecha", DateTime.Now)
                        );
                    }
                }

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
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Roles";
            ViewData["ActiveParent"] = "Acceso";

            ViewBag.PageTitle = "Roles > Editar";

            var rol = await _context.tbRoles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            var pantallas = _context.tbPantallas.ToList();
            var pantallasAsignadas = _context.tbRolesPorPantalla
                .Where(rp => rp.Roles_Id == id)
                .Select(rp => rp.Panta_Id)
                .ToList();

            ViewBag.Pantallas = pantallas;
            ViewBag.PantallasAsignadas = pantallasAsignadas;
            return View(rol);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string pantallasSeleccionadas, tbRoles tbRoles)
        {
            if(ModelState.IsValid)
            {
                _context.Database.ExecuteSqlRaw("Acces.SP_Rol_Actualizar @role, @descripcion, @usua, @fecha",
                new SqlParameter("@role", id),
                new SqlParameter("@descripcion", tbRoles.Roles_Descripcion),
                new SqlParameter("@usua", 1),
                new SqlParameter("@fecha", DateTime.Now)
                );

                var pantallasIds = JsonConvert.DeserializeObject<List<int>>(pantallasSeleccionadas);
                _context.Database.ExecuteSqlRaw("Acces.SP_RolesPorPantalla_Eliminar @role",
                    new SqlParameter("@role", id)
                );

                // Insertar las nuevas pantallas asignadas
                foreach (var pantallaId in pantallasIds)
                {
                    _context.Database.ExecuteSqlRaw("Acces.SP_RolesPorPantalla_Insertar @rol, @pantalla, @usua, @fecha",
                            new SqlParameter("@rol", id),
                            new SqlParameter("@pantalla", pantallaId),
                            new SqlParameter("@usua", 1),
                            new SqlParameter("@fecha", DateTime.Now)
                    );
                }
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                _context.Database.ExecuteSqlRaw("Acces.SP_RolesPorPantalla_Eliminar @role",
                        new SqlParameter("@role", id)
                    );
                _context.Database.ExecuteSqlRaw("Acces.SP_Rol_Eliminar @role",
                    new SqlParameter("@role", id)
                );
            }
            catch (DbUpdateException)
            {
                TempData["MensajeError"] = "Error al guardar los datos. Verifique la información.";
            }
            catch (SqlException ex) when (ex.Number == 547) // Error de restricción de clave foránea
            {
                TempData["MensajeError"] = "No se puede eliminar el rol porque está siendo utilizado en otras partes del sistema.";
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

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbRoles = await _context.tbRoles.FindAsync(id);
            if (tbRoles != null)
            {
                _context.tbRoles.Remove(tbRoles);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbRolesExists(int id)
        {
            return _context.tbRoles.Any(e => e.Roles_Id == id);
        }
    }
}
