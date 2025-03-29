using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using Microsoft.Data.SqlClient;
using System.Net.WebSockets;
using GestionGimnasio.Filters;

namespace GestionGimnasio.Controllers
{
    [AuthFilter]
    public class UsuariosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public UsuariosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Usuarios";
            ViewData["ActiveParent"] = "Acceso";

            ViewBag.PageTitle = "Usuarios";
            ViewData["Emple_id"] = new SelectList(_context.tbEmpleados, "Emple_Id", "Emple_PrimerNombre");
            ViewData["Roles_id"] = new SelectList(_context.tbRoles, "Roles_Id", "Roles_Descripcion");
            var gestionGimnasioContext = _context.tbUsuarios.Include(t => t.Emple).Include(t => t.Roles).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation).Where(x=>x.Usuar_Estado==true);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbUsuarios = await _context.tbUsuarios
                .Include(t => t.Emple)
                .Include(t => t.Roles)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Usuar_id == id);
            if (tbUsuarios == null)
            {
                return NotFound();
            }

            return View(tbUsuarios);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["Emple_id"] = new SelectList(_context.tbEmpleados, "Emple_Id", "Emple_Direccion");
            ViewData["Roles_id"] = new SelectList(_context.tbRoles, "Roles_Id", "Roles_Descripcion");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Usuar_id,Usuar_Nombre,Usuar_Contrasena,Emple_id,Roles_id,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion,Usuar_EsAdmin,Usuar_Estado")] tbUsuarios tbUsuarios)
        {
            ModelState.Remove("Usuar_ContrasenaVieja");
            ModelState.Remove("Usuar_Contrasena1");
            ModelState.Remove("Usuar_Contrasena2");
            if (ModelState.IsValid)
            {
                _context.Database.ExecuteSqlRaw("Acces.SP_Usuario_Insertar @nombre, @contrasena, @emple, @role, @isAdmin ,@usercreatedAt, @datecreatedAt",
                    new SqlParameter("@nombre", tbUsuarios.Usuar_Nombre),
                    new SqlParameter("@contrasena", tbUsuarios.Usuar_Contrasena),
                    new SqlParameter("@emple", tbUsuarios.Emple_id),
                    new SqlParameter("@role", tbUsuarios.Roles_id),
                    new SqlParameter("@isAdmin", tbUsuarios.Usuar_EsAdmin),
                    new SqlParameter("@usercreatedAt", 1),
                    new SqlParameter("@datecreatedAt", DateTime.Now)
                );
                TempData["MensajeExito"] = "Usuario creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Emple_id"] = new SelectList(_context.tbEmpleados, "Emple_Id", "Emple_Direccion", tbUsuarios.Emple_id);
            ViewData["Roles_id"] = new SelectList(_context.tbRoles, "Roles_Id", "Roles_Descripcion", tbUsuarios.Roles_id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbUsuarios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbUsuarios.Usuar_Modificacion);
            return View(tbUsuarios);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbUsuarios = await _context.tbUsuarios.FindAsync(id);
            if (tbUsuarios == null)
            {
                return NotFound();
            }
            ViewData["Emple_id"] = new SelectList(_context.tbEmpleados, "Emple_Id", "Emple_PrimerNombre", tbUsuarios.Emple_id);
            ViewData["Roles_id"] = new SelectList(_context.tbRoles, "Roles_Id", "Roles_Descripcion", tbUsuarios.Roles_id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbUsuarios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbUsuarios.Usuar_Modificacion);
            return PartialView("_Edit", tbUsuarios);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Usuar_id,Usuar_Nombre,Emple_id,Roles_id,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion,Usuar_EsAdmin,Usuar_Estado")] tbUsuarios tbUsuarios)
        {
            ModelState.Remove("Usuar_ContrasenaVieja");
            ModelState.Remove("Usuar_Contrasena1");
            ModelState.Remove("Usuar_Contrasena2");
            ModelState.Remove("Usuar_Contrasena");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Acces.SP_Usuario_Actualizar @id, @nombre, @emple, @role, @isAdmin ,@userUpdateAt, @dateUpdateAt",
                        new SqlParameter("@id", tbUsuarios.Usuar_id),
                        new SqlParameter("@nombre", tbUsuarios.Usuar_Nombre),
                        new SqlParameter("@emple", tbUsuarios.Emple_id),
                        new SqlParameter("@role", tbUsuarios.Roles_id),
                        new SqlParameter("@isAdmin", tbUsuarios.Usuar_EsAdmin),
                        new SqlParameter("@userUpdateAt", 1),
                        new SqlParameter("@dateUpdateAt", DateTime.Now)
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbUsuariosExists(tbUsuarios.Usuar_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["MensajeExito"] = "Usuario editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Emple_id"] = new SelectList(_context.tbEmpleados, "Emple_Id", "Emple_Direccion", tbUsuarios.Emple_id);
            ViewData["Roles_id"] = new SelectList(_context.tbRoles, "Roles_Id", "Roles_Descripcion", tbUsuarios.Roles_id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbUsuarios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbUsuarios.Usuar_Modificacion);
            return View(tbUsuarios);
        }
        public async Task<IActionResult> RestablecerClave(int? id)
        {
            Variable.Variable.idUsuario = id.Value;
            if (id == null)
            {
                return NotFound();
            }
            var tbUsuarios = await _context.tbUsuarios.FindAsync(id);
            if (tbUsuarios == null)
            {
                return NotFound();
            }
            
            return PartialView("_RestablecerClave", tbUsuarios);
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(int id, tbUsuarios tbUsuarios)
        {
            int idUsuario = Variable.Variable.idUsuario;
            ModelState.Remove("Usuar_Nombre");
            ModelState.Remove("Usuar_Contrasena");
            ModelState.Remove("Usuar_ContrasenaVieja");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Acces.SP_Usuario_CambiarContrasena @id, @contrasena",
                        new SqlParameter("@id", idUsuario),
                        new SqlParameter("@contrasena", tbUsuarios.Usuar_Contrasena1)
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbUsuariosExists(tbUsuarios.Usuar_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["MensajeExito"] = "Clave cambiada correctamente";
                return RedirectToAction(nameof(Index));
            }
            return View(tbUsuarios);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _context.Database.ExecuteSqlRaw("Acces.SP_Usuario_Eliminar @id",
                new SqlParameter("@id", id)
            );
            TempData["MensajeExito"] = "Usuario eliminado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbUsuarios = await _context.tbUsuarios.FindAsync(id);
            if (tbUsuarios != null)
            {
                _context.tbUsuarios.Remove(tbUsuarios);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbUsuariosExists(int id)
        {
            return _context.tbUsuarios.Any(e => e.Usuar_id == id);
        }
    }
}
