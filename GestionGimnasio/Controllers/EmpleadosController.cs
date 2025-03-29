using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using Microsoft.Data.SqlClient;

namespace GestionGimnasio.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public EmpleadosController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Empleados";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Empleados";
            var gestionGimnasioContext = _context.tbEmpleados.Include(t => t.Cargo).Include(t => t.EsCiv).Include(t => t.Munic_CodigoNavigation).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation).Where(x=>x.Emple_Estado==true);
            return View(await gestionGimnasioContext.ToListAsync());
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Empleados";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Empleados";

            if (id == null)
            {
                return NotFound();
            }

            var tbEmpleados = await _context.tbEmpleados
                .Include(t => t.Cargo)
                .Include(t => t.EsCiv)
                .Include(t => t.Munic_CodigoNavigation)
                .Include(t => t.Usuar_CreacionNavigation)
                .Include(t => t.Usuar_ModificacionNavigation)
                .FirstOrDefaultAsync(m => m.Emple_Id == id);
            if (tbEmpleados == null)
            {
                return NotFound();
            }

            return View(tbEmpleados);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Empleados";
            ViewData["ActiveParent"] = "Gimnasio";

            

            ViewBag.PageTitle = "Empleados > Crear";

            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion");
            var cargo = _context.tbCargos.Where(c => c.Cargo_Estado == true).ToList();
            ViewData["Cargo_Id"] = new SelectList(cargo, "Cargo_Id", "Cargo_Nombre");
            var estado = _context.tbEstadosCiviles.Where(e => e.EsCiv_Estado == true).ToList();
            ViewData["EsCiv_Id"] = new SelectList(estado, "EsCiv_Id", "EsCiv_Descripcion");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Emple_Id,Emple_Identidad,Emple_PrimerNombre,Emple_SegundoNombre,Emple_PrimerApellido,Emple_SegundoApellido,Emple_Sexo,EsCiv_Id,Emple_FechaNacimiento,Emple_Direccion,Munic_Codigo,Cargo_Id,Emple_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEmpleados tbEmpleados)
        {
            if(tbEmpleados.Emple_SegundoNombre==null)
            {
                tbEmpleados.Emple_SegundoNombre = "";
            }
            if (tbEmpleados.Emple_SegundoApellido == null)
            {
                tbEmpleados.Emple_SegundoApellido = "";
            }
            
            if (ModelState.IsValid)
            {
                _context.Database.ExecuteSqlRaw("Gimna.SP_Empleado_Insertar @dni, @primerN, @segundoN, @primerA, @segundoA, @sexo, @esci, @fecha, @dire, @cargo, @munic, @usua, @fechacreacion",
                    new SqlParameter ("@dni", tbEmpleados.Emple_Identidad),
                    new SqlParameter("@primerN", tbEmpleados.Emple_PrimerNombre),
                    new SqlParameter("@segundoN", tbEmpleados.Emple_SegundoNombre),
                    new SqlParameter("@primerA", tbEmpleados.Emple_PrimerApellido),
                    new SqlParameter("@segundoA", tbEmpleados.Emple_SegundoApellido),
                    new SqlParameter("@sexo", tbEmpleados.Emple_Sexo),
                    new SqlParameter("@esci", tbEmpleados.EsCiv_Id),
                    new SqlParameter("@fecha", tbEmpleados.Emple_FechaNacimiento),
                    new SqlParameter("@dire", tbEmpleados.Emple_Direccion),
                    new SqlParameter("@cargo", tbEmpleados.Cargo_Id),
                    new SqlParameter("@munic", tbEmpleados.Munic_Codigo),
                    new SqlParameter("@usua", 1),
                    new SqlParameter("@fechacreacion", DateTime.Now));
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cargo_Id"] = new SelectList(_context.tbCargos, "Cargo_Id", "Cargo_Nombre", tbEmpleados.Cargo_Id);
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbEmpleados.EsCiv_Id);
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbEmpleados.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEmpleados.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEmpleados.Usuar_Modificacion);
            return View(tbEmpleados);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Empleados";
            ViewData["ActiveParent"] = "Gimnasio";

            ViewBag.PageTitle = "Empleados";

            if (id == null)
            {
                return NotFound();
            }

            var tbEmpleados = await _context.tbEmpleados.FindAsync(id);
            if (tbEmpleados == null)
            {
                return NotFound();
            }
            var municipioEmpleado = await _context.tbMunicipios.FindAsync(tbEmpleados.Munic_Codigo);
            var departamentoEmpleado = await _context.tbDepartamentos.FindAsync(municipioEmpleado.Depar_Codigo);
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion", departamentoEmpleado.Depar_Codigo);
            var municipiosFiltrados = _context.tbMunicipios.Where(m => m.Depar_Codigo == departamentoEmpleado.Depar_Codigo).ToList();
            ViewData["Munic_Codigo"] = new SelectList(municipiosFiltrados, "Munic_Codigo", "Munic_Nombre", tbEmpleados.Munic_Codigo);

            ViewData["Cargo_Id"] = new SelectList(_context.tbCargos, "Cargo_Id", "Cargo_Nombre", tbEmpleados.Cargo_Id);
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbEmpleados.EsCiv_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEmpleados.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEmpleados.Usuar_Modificacion);
            return View(tbEmpleados);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Emple_Id,Emple_Identidad,Emple_PrimerNombre,Emple_SegundoNombre,Emple_PrimerApellido,Emple_SegundoApellido,Emple_Sexo,EsCiv_Id,Emple_FechaNacimiento,Emple_Direccion,Munic_Codigo,Cargo_Id,Emple_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbEmpleados tbEmpleados)
        {
            if (id != tbEmpleados.Emple_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (tbEmpleados.Emple_SegundoNombre == null)
                {
                    tbEmpleados.Emple_SegundoNombre = "";
                }
                if (tbEmpleados.Emple_SegundoApellido == null)
                {
                    tbEmpleados.Emple_SegundoApellido = "";
                }
                
                try
                {
                    _context.Database.ExecuteSqlRaw("Gimna.SP_Empleado_Actualizar @id, @dni, @primerN, @segundoN, @primerA, @segundoA, @sexo, @esci, @fecha, @dire, @cargo, @munic, @usua, @fechamodificacion",
                        new SqlParameter("@id", tbEmpleados.Emple_Id),
                        new SqlParameter("@dni", tbEmpleados.Emple_Identidad),
                        new SqlParameter("@primerN", tbEmpleados.Emple_PrimerNombre),
                        new SqlParameter("@segundoN", tbEmpleados.Emple_SegundoNombre),
                        new SqlParameter("@primerA", tbEmpleados.Emple_PrimerApellido),
                        new SqlParameter("@segundoA", tbEmpleados.Emple_SegundoApellido),
                        new SqlParameter("@sexo", tbEmpleados.Emple_Sexo),
                        new SqlParameter("@esci", tbEmpleados.EsCiv_Id),
                        new SqlParameter("@fecha", tbEmpleados.Emple_FechaNacimiento),
                        new SqlParameter("@dire", tbEmpleados.Emple_Direccion),
                        new SqlParameter("@cargo", tbEmpleados.Cargo_Id),
                        new SqlParameter("@munic", tbEmpleados.Munic_Codigo),
                        new SqlParameter("@usua", 1),
                        new SqlParameter("@fechamodificacion", DateTime.Now));
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbEmpleadosExists(tbEmpleados.Emple_Id))
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
            ViewData["Cargo_Id"] = new SelectList(_context.tbCargos, "Cargo_Id", "Cargo_Nombre", tbEmpleados.Cargo_Id);
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbEmpleados.EsCiv_Id);
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbEmpleados.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEmpleados.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbEmpleados.Usuar_Modificacion);
            return View(tbEmpleados);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _context.Database.ExecuteSqlRaw("Gimna.SP_Empleado_Eliminar @id",
                new SqlParameter("@id", id));
            return RedirectToAction(nameof(Index));
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbEmpleados = await _context.tbEmpleados.FindAsync(id);
            if (tbEmpleados != null)
            {
                _context.tbEmpleados.Remove(tbEmpleados);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CargarMunicipio(string? cod)
        {
            var ddl = _context.tbMunicipios.FromSqlRaw("Gnral.SP_MunicipioPorDepartamento @depar_codigo",
                                new SqlParameter ("@depar_codigo", cod)).ToList();
            return Ok(ddl);
        }

        private bool tbEmpleadosExists(int id)
        {
            return _context.tbEmpleados.Any(e => e.Emple_Id == id);
        }
    }
}
