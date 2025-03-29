using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionGimnasio.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using GestionGimnasio.Filters;

namespace GestionGimnasio.Controllers
{
    [AuthFilter]
    public class ClientesController : Controller
    {
        private readonly GestionGimnasioContext _context;

        public ClientesController(GestionGimnasioContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Clientes";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Clientes";

            var gestionGimnasioContext = _context.tbClientes.Include(t => t.EsCiv).Include(t => t.Munic_CodigoNavigation).Include(t => t.Usuar_CreacionNavigation).Include(t => t.Usuar_ModificacionNavigation);
            return View(await gestionGimnasioContext.Where(X => X.Clien_Estado == true).ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActivePage"] = "Clientes";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Clientes > Detalles";

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

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Clientes";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Clientes > Crear";

            var estadoCiviles = _context.tbEstadosCiviles.Where(X => X.EsCiv_Estado == true);
            ViewData["EsCiv_Id"] = new SelectList(estadoCiviles, "EsCiv_Id", "EsCiv_Descripcion");
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion");
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena");
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Clien_id,Clien_Identidad,Clien_PrimerNombre,Clien_SegundoNombre,Clien_PrimerApellido,Clien_SegundoApellido,Clien_Sexo,EsCiv_Id,Clien_FechaNacimiento,Clien_Direccion,Munic_Codigo,Clien_esMiembroActivo,Clien_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbClientes tbClientes)
        {
            ModelState.Remove("Clien_Estado");
            ModelState.Remove("Usuar_CreacionNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Usuar_Modificacion");
            ModelState.Remove("Usuar_ModificacionNavigation");

            if (tbClientes.Clien_SegundoNombre == null)
            {
                tbClientes.Clien_SegundoNombre = "";
            }
            if (tbClientes.Clien_SegundoNombre == null)
            {
                tbClientes.Clien_SegundoNombre = "";
            }
            //var dni = _context.tbClientes.Where(x => x.Clien_Identidad == tbClientes.Clien_Identidad).ToList();
            //if (dni != null)
            //{
            //    TempData["MensajeError"] = "DNI duplicado.";
            //    return RedirectToAction(nameof(Create));
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Cliente_Insertar @dni, @firstname, @secondname, @firstlastname, @secondlastname, @gender, @civilstate,@birthday,@address,@isActiveMember,@munic_codigo,@usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@dni", tbClientes.Clien_Identidad),
                                                     new SqlParameter("@firstname", tbClientes.Clien_PrimerNombre),
                                                     new SqlParameter("@secondname", tbClientes.Clien_SegundoNombre ?? (object)DBNull.Value),
                                                     new SqlParameter("@firstlastname", tbClientes.Clien_PrimerApellido),
                                                     new SqlParameter("@secondlastname", tbClientes.Clien_SegundoApellido ?? (object)DBNull.Value),
                                                     new SqlParameter("@gender", tbClientes.Clien_Sexo),
                                                     new SqlParameter("@civilstate", tbClientes.EsCiv_Id),
                                                     new SqlParameter("@birthday", tbClientes.Clien_FechaNacimiento),
                                                     new SqlParameter("@isActiveMember", tbClientes.Clien_esMiembroActivo),
                                                     new SqlParameter("@address", tbClientes.Clien_Direccion),
                                                     new SqlParameter("@munic_codigo", tbClientes.Munic_Codigo),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
                    TempData["MensajeExito"] = "Cliente creado correctamente";
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
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbClientes.EsCiv_Id);
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbClientes.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Modificacion);
            return View(tbClientes);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActivePage"] = "Clientes";
            ViewData["ActiveParent"] = "General";

            ViewBag.PageTitle = "Clientes > Editar";

            if (id == null)
            {
                return NotFound();
            }

            var tbClientes = await _context.tbClientes.FindAsync(id);
            if (tbClientes == null)
            {
                return NotFound();
            }
            var municipioCliente = await _context.tbMunicipios.FindAsync(tbClientes.Munic_Codigo);
            var departamentoCliente = await _context.tbDepartamentos.FindAsync(municipioCliente.Depar_Codigo);
            ViewData["Depar_Codigo"] = new SelectList(_context.tbDepartamentos, "Depar_Codigo", "Depar_Descripcion", departamentoCliente.Depar_Codigo);
            var municipiosFiltrados = _context.tbMunicipios.Where(m => m.Depar_Codigo == departamentoCliente.Depar_Codigo).ToList();
            ViewData["Munic_Codigo"] = new SelectList(municipiosFiltrados, "Munic_Codigo", "Munic_Nombre", tbClientes.Munic_Codigo);
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbClientes.EsCiv_Id);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Modificacion);
            return View(tbClientes);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Clien_id,Clien_Identidad,Clien_PrimerNombre,Clien_SegundoNombre,Clien_PrimerApellido,Clien_SegundoApellido,Clien_Sexo,EsCiv_Id,Clien_FechaNacimiento,Clien_Direccion,Munic_Codigo,Clien_esMiembroActivo,Clien_Estado,Usuar_Creacion,Fecha_Creacion,Usuar_Modificacion,Fecha_Modificacion")] tbClientes tbClientes)
        {
            ModelState.Remove("Clien_Estado");
            ModelState.Remove("Usuar_CreacionNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Munic_CodigoNavigation");
            ModelState.Remove("Usuar_Creacion");
            ModelState.Remove("Usuar_CreacionNavigation");

            if (id != tbClientes.Clien_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (tbClientes.Clien_SegundoNombre == null)
                {
                    tbClientes.Clien_SegundoNombre = "";
                }
                if (tbClientes.Clien_SegundoApellido == null)
                {
                    tbClientes.Clien_SegundoApellido = "";
                }
                try
                {
                    _context.Database.ExecuteSqlRaw("Gnral.SP_Cliente_Actualizar @id, @dni, @firstname, @secondname, @firstlastname, @secondlastname, @gender, @civilstate,@birthday,@address,@isActiveMember,@munic_codigo,@usercreatedAt, @datecreatedAt",
                                                    new SqlParameter("@id", tbClientes.Clien_id),
                                                    new SqlParameter("@dni", tbClientes.Clien_Identidad),
                                                     new SqlParameter("@firstname", tbClientes.Clien_PrimerNombre),
                                                     new SqlParameter("@secondname", tbClientes.Clien_SegundoNombre ?? (object)DBNull.Value),
                                                     new SqlParameter("@firstlastname", tbClientes.Clien_PrimerApellido),
                                                     new SqlParameter("@secondlastname", tbClientes.Clien_SegundoApellido ?? (object)DBNull.Value),
                                                     new SqlParameter("@gender", tbClientes.Clien_Sexo),
                                                     new SqlParameter("@civilstate", tbClientes.EsCiv_Id),
                                                     new SqlParameter("@birthday", tbClientes.Clien_FechaNacimiento),
                                                     new SqlParameter("@isActiveMember", tbClientes.Clien_esMiembroActivo),
                                                     new SqlParameter("@address", tbClientes.Clien_Direccion),
                                                     new SqlParameter("@munic_codigo", tbClientes.Munic_Codigo),
                                                     new SqlParameter("@usercreatedAt", 1),
                                                     new SqlParameter("@datecreatedAt", DateTime.Now));
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
                TempData["MensajeExito"] = "Cliente editado correctamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["EsCiv_Id"] = new SelectList(_context.tbEstadosCiviles, "EsCiv_Id", "EsCiv_Descripcion", tbClientes.EsCiv_Id);
            ViewData["Munic_Codigo"] = new SelectList(_context.tbMunicipios, "Munic_Codigo", "Munic_Codigo", tbClientes.Munic_Codigo);
            ViewData["Usuar_Creacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Creacion);
            ViewData["Usuar_Modificacion"] = new SelectList(_context.tbUsuarios, "Usuar_id", "Usuar_Contrasena", tbClientes.Usuar_Modificacion);
            return View(tbClientes);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC Gnral.SP_Cliente_Eliminar @code",
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
            TempData["MensajeExito"] = "Cliente eliminado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbClientes = await _context.tbClientes.FindAsync(id);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult CargarMunicipio(string? cod)
        {
            var ddl = _context.tbMunicipios.FromSqlRaw("Gnral.SP_MunicipioPorDepartamento @depar_codigo",
                                new SqlParameter("@depar_codigo", cod)).ToList();
            return Ok(ddl);
        }
        private bool tbClientesExists(int id)
        {
            return _context.tbClientes.Any(e => e.Clien_id == id);
        }
    }
}
