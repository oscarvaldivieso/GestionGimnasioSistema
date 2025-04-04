﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionGimnasio.Models;

public partial class tbRoles
{
    public int Roles_Id { get; set; }

    [Display(Name = "Rol")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Roles_Descripcion { get; set; }

    public bool? Roles_Estado { get; set; }

    public int Usuar_Creacion { get; set; }

    public DateTime Fecha_Creacion { get; set; }

    public int? Usuar_Modificacion { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }

    public virtual tbUsuarios Usuar_CreacionNavigation { get; set; }

    public virtual tbUsuarios Usuar_ModificacionNavigation { get; set; }

    public virtual ICollection<tbRolesPorPantalla> tbRolesPorPantalla { get; set; } = new List<tbRolesPorPantalla>();

    public virtual ICollection<tbUsuarios> tbUsuarios { get; set; } = new List<tbUsuarios>();
}