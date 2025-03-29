using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Data.SqlClient;

namespace GestionGimnasio.Controllers
{
    public class ReporteGimnasiosController : Controller
    {
        private readonly IConverter _converter;

        private readonly GestionGimnasioContext _context;

        public ReporteGimnasiosController(IConverter converter, GestionGimnasioContext context)
        {
            _converter = converter;
            _context = context;
        }

        // GET: ReporteGimnasios
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Gimnasios Reporte";
            ViewData["ActiveParent"] = "Reportes";

            ViewBag.PageTitle = "Gimnasios";
            return View();
        }


        //Vista de pdf
        public async Task<IActionResult> VistaParaPDF(string munic_Codigo)
        {
            var index = _context.tbGimnasios.Include(t => t.Munic_CodigoNavigation).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation).Where(t=>t.Gimna_Estado==true).OrderBy(t=>t.Munic_Codigo);
            return View("VistaParaPDF", await index.ToListAsync());
        }

        public IActionResult MostrarPDFenPagina()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/ReporteGimnasios/VistaParaPDF";


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
            url_pagina = $"{url_pagina}/ReporteGimnasios/VistaParaPDF";


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
            string nombrePDF = "Gimnasios_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";

            return File(archivoPDF, "application/pdf", nombrePDF);
        }



        // GET: ReporteGimnasios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: ReporteGimnasios/Create
        public IActionResult Create()
        {
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: ReporteGimnasios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Gimna_Id,Gimna_Nombre,Gimna_SemanaHoraApertura,Gimna_SemanaHoraCierre,Gimna_FinDeHoraApertura,Gimna_FinDeHoraCierre,Gimna_Direccion,Munic_Codigo,Gimna_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbGimnasios tbGimnasios)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbGimnasios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbGimnasios.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Modificacion);
            return View(tbGimnasios);
        }

        // GET: ReporteGimnasios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbGimnasios = await _context.tbGimnasios.FindAsync(id);
            if (tbGimnasios == null)
            {
                return NotFound();
            }
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbGimnasios.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Modificacion);
            return View(tbGimnasios);
        }

        // POST: ReporteGimnasios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Gimna_Id,Gimna_Nombre,Gimna_SemanaHoraApertura,Gimna_SemanaHoraCierre,Gimna_FinDeHoraApertura,Gimna_FinDeHoraCierre,Gimna_Direccion,Munic_Codigo,Gimna_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbGimnasios tbGimnasios)
        {
            if (id != tbGimnasios.Gimna_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbGimnasios);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbGimnasios.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbGimnasios.Usuar_Modificacion);
            return View(tbGimnasios);
        }

        // GET: ReporteGimnasios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

        // POST: ReporteGimnasios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbGimnasios = await _context.tbGimnasios.FindAsync(id);
            if (tbGimnasios != null)
            {
                _context.tbGimnasios.Remove(tbGimnasios);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbGimnasiosExists(int id)
        {
            return _context.tbGimnasios.Any(e => e.Gimna_Id == id);
        }
    }
}
