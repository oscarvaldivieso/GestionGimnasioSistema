
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionGimnasio.Models;

public partial class tbCargos
{
    public int Cargo_Id { get; set; }

    [Display(Name = "Cargo")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Cargo_Nombre { get; set; }

    public bool? Cargo_Estado { get; set; }

    public int Usuar_Creacion { get; set; }

    public DateTime Fecha_Creacion { get; set; }

    public int? Usuar_Modificacion { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }

    public virtual tbUsuarios Usuar_CreacionNavigation { get; set; }

    public virtual tbUsuarios Usuar_ModificacionNavigation { get; set; }

    public virtual ICollection<tbEmpleados> tbEmpleados { get; set; } = new List<tbEmpleados>();
}