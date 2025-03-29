using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using System.Dynamic;

namespace GestionGimnasio.Controllers
{
    public class EjerciciosPorRutinasController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public EjerciciosPorRutinasController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: EjerciciosPorRutinas
        public async Task<IActionResult> Index()
        {
            var rutinasConEjercicios = await _context.tbEjerciciosPorRutinas
                .Include(e => e.Rutin)
                .Include(e => e.Ejerc)
                .ToListAsync();

            var rutinasAgrupadas = rutinasConEjercicios
                .GroupBy(r => r.Rutin_Id)
                .Select(g =>
                {
                    dynamic expando = new ExpandoObject();
                    expando.Rutina = g.First().Rutin;
                    expando.Ejercicios = g.Select(e => e.Ejerc).ToList();
                    return expando;
                })
                .ToList();

            return View(rutinasAgrupadas);
        }

        // GET: EjerciciosPorRutinas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbEjerciciosPorRutinas = await _context.tbEjerciciosPorRutinas
                .Include(t => t.Ejerc)
                .Include(t => t.Rutin)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.EjRut_Id == id);
            if (tbEjerciciosPorRutinas == null)
            {
                return NotFound();
            }

            return View(tbEjerciciosPorRutinas);
        }

        // GET: EjerciciosPorRutinas/Create
        public IActionResult Create()
        {
            ViewData["Ejerc_Id"] = new SelectList(_context.tbEjercicios, "Ejerc_Id", "Ejerc_Descripcion");
            ViewData["Rutin_Id"] = new SelectList(_context.tbRutinas, "Rutin_Id", "Rutin_Nombre");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: EjerciciosPorRutinas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EjRut_Id,Rutin_Id,Ejerc_Id,EjRut_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEjerciciosPorRutinas tbEjerciciosPorRutinas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbEjerciciosPorRutinas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ejerc_Id"] = new SelectList(_context.tbEjercicios, "Ejerc_Id", "Ejerc_Descripcion", tbEjerciciosPorRutinas.Ejerc_Id);
            ViewData["Rutin_Id"] = new SelectList(_context.tbRutinas, "Rutin_Id", "Rutin_Nombre", tbEjerciciosPorRutinas.Rutin_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjerciciosPorRutinas.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjerciciosPorRutinas.Usuar_Modificacion);
            return View(tbEjerciciosPorRutinas);
        }

        // GET: EjerciciosPorRutinas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbEjerciciosPorRutinas = await _context.tbEjerciciosPorRutinas.FindAsync(id);
            if (tbEjerciciosPorRutinas == null)
            {
                return NotFound();
            }
            ViewData["Ejerc_Id"] = new SelectList(_context.tbEjercicios, "Ejerc_Id", "Ejerc_Descripcion", tbEjerciciosPorRutinas.Ejerc_Id);
            ViewData["Rutin_Id"] = new SelectList(_context.tbRutinas, "Rutin_Id", "Rutin_Nombre", tbEjerciciosPorRutinas.Rutin_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjerciciosPorRutinas.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjerciciosPorRutinas.Usuar_Modificacion);
            return View(tbEjerciciosPorRutinas);
        }

        // POST: EjerciciosPorRutinas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EjRut_Id,Rutin_Id,Ejerc_Id,EjRut_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEjerciciosPorRutinas tbEjerciciosPorRutinas)
        {
            if (id != tbEjerciciosPorRutinas.EjRut_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbEjerciciosPorRutinas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbEjerciciosPorRutinasExists(tbEjerciciosPorRutinas.EjRut_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ejerc_Id"] = new SelectList(_context.tbEjercicios, "Ejerc_Id", "Ejerc_Descripcion", tbEjerciciosPorRutinas.Ejerc_Id);
            ViewData["Rutin_Id"] = new SelectList(_context.tbRutinas, "Rutin_Id", "Rutin_Nombre", tbEjerciciosPorRutinas.Rutin_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjerciciosPorRutinas.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEjerciciosPorRutinas.Usuar_Modificacion);
            return View(tbEjerciciosPorRutinas);
        }

        // GET: EjerciciosPorRutinas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbEjerciciosPorRutinas = await _context.tbEjerciciosPorRutinas
                .Include(t => t.Ejerc)
                .Include(t => t.Rutin)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.EjRut_Id == id);
            if (tbEjerciciosPorRutinas == null)
            {
                return NotFound();
            }

            return View(tbEjerciciosPorRutinas);
        }

        // POST: EjerciciosPorRutinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbEjerciciosPorRutinas = await _context.tbEjerciciosPorRutinas.FindAsync(id);
            if (tbEjerciciosPorRutinas != null)
            {
                _context.tbEjerciciosPorRutinas.Remove(tbEjerciciosPorRutinas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbEjerciciosPorRutinasExists(int id)
        {
            return _context.tbEjerciciosPorRutinas.Any(e => e.EjRut_Id == id);
        }
    }
}
