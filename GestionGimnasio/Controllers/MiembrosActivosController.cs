using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using DinkToPdf;
using Microsoft.AspNetCore.Http.Extensions;
using DinkToPdf.Contracts;

namespace GestionGimnasio.Controllers
{
    public class MiembrosActivosController : Controller
    {
        private readonly IConverter _converter;

        private readonly GestionGimnasioContext _context;

        public MiembrosActivosController(IConverter converter,GestionGimnasioContext context)
        {
            _converter = converter;
            _context = context;
        }

        // GET: MiembrosActivos
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Reporte Miembros Activos";
            ViewData["ActiveParent"] = "Reportes";

            ViewBag.PageTitle = "Miembros Activos";
            return View();
        }

        //Vista de pdf
        public async Task<IActionResult> VistaParaPDF(string munic_Codigo)
        {
            var index = _context.tbClientes.Include(t => t.EsCiv).Include(t => t.Munic_CodigoNavigation).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View("VistaParaPDF", await index.ToListAsync());
        }

        public IActionResult MostrarPDFenPagina()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/MiembrosActivos/VistaParaPDF";


            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings(){
                        Page = url_pagina
                    }
                }

            };

            var archivoPDF = _converter.Convert(pdf);
            return File(archivoPDF, "application/pdf");
        }

        public IActionResult DescargarPDF()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/MiembrosActivos/VistaParaPDF";


            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings(){
                        Page = url_pagina
                    }
                }

            };

            var archivoPDF = _converter.Convert(pdf);
            string nombrePDF = "MiembroS_Activos_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";

            return File(archivoPDF, "application/pdf", nombrePDF);
        }

        // GET: MiembrosActivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbClientes = await _context.tbClientes
                .Include(t => t.EsCiv)
                .Include(t => t.Munic_CodigoNavigation)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Clien_id == id);
            if (tbClientes == null)
            {
                return NotFound();
            }

            return View(tbClientes);
        }

        // GET: MiembrosActivos/Create
        public IActionResult Create()
        {
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion");
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: MiembrosActivos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Clien_id,Clien_Identidad,Clien_PrimerNombre,Clien_SegundoNombre,Clien_PrimerApellido,Clien_SegundoApellido,Clien_Sexo,EsCiv_Id,Clien_FechaNacimiento,Clien_Direccion,Munic_Codigo,Clien_esMiembroActivo,Clien_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbClientes tbClientes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbClientes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbClientes.EsCiv_Id);
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbClientes.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Modificacion);
            return View(tbClientes);
        }

        // GET: MiembrosActivos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbClientes = await _context.tbClientes.FindAsync(id);
            if (tbClientes == null)
            {
                return NotFound();
            }
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbClientes.EsCiv_Id);
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbClientes.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Modificacion);
            return View(tbClientes);
        }

        // POST: MiembrosActivos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Clien_id,Clien_Identidad,Clien_PrimerNombre,Clien_SegundoNombre,Clien_PrimerApellido,Clien_SegundoApellido,Clien_Sexo,EsCiv_Id,Clien_FechaNacimiento,Clien_Direccion,Munic_Codigo,Clien_esMiembroActivo,Clien_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbClientes tbClientes)
        {
            if (id != tbClientes.Clien_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbClientes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbClientesExists(tbClientes.Clien_id))
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
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbClientes.EsCiv_Id);
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbClientes.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Modificacion);
            return View(tbClientes);
        }

        // GET: MiembrosActivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbClientes = await _context.tbClientes
                .Include(t => t.EsCiv)
                .Include(t => t.Munic_CodigoNavigation)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Clien_id == id);
            if (tbClientes == null)
            {
                return NotFound();
            }

            return View(tbClientes);
        }

        // POST: MiembrosActivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbClientes = await _context.tbClientes.FindAsync(id);
            if (tbClientes != null)
            {
                _context.tbClientes.Remove(tbClientes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbClientesExists(int id)
        {
            return _context.tbClientes.Any(e => e.Clien_id == id);
        }
    }
}
