﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionGimnasio.Models;

public partial class tbEmpleados
{
    public int Emple_Id { get; set; }

    [Display(Name = "DNI")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Emple_Identidad { get; set; }

    [Display(Name = "Primer Nombre")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Emple_PrimerNombre { get; set; }

    [Display(Name = "Segundo Nombre")]
    public string Emple_SegundoNombre { get; set; }

    [Display(Name = "Primer Apellido")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Emple_PrimerApellido { get; set; }

    [Display(Name = "Segundo Apellido")]
    public string Emple_SegundoApellido { get; set; }

    [Display(Name = "Sexo")]
    public string Emple_Sexo { get; set; }

    [Display(Name = "Estado Civil")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public int EsCiv_Id { get; set; }

    [Display(Name = "Fecha Nacimineto")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public DateTime Emple_FechaNacimiento { get; set; }

    [Display(Name = "Direccion")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Emple_Direccion { get; set; }

    [Display(Name = "Municipio")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Munic_Codigo { get; set; }

    [Display(Name = "Cargo")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public int Cargo_Id { get; set; }

    [NotMapped]
    [Display(Name = "Departamento")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public int Depar_Codigo { get; set; }

    public bool? Emple_Estado { get; set; }

    public int Usuar_Creacion { get; set; }

    public DateTime Fecha_Creacion { get; set; }

    public int? Usuar_Modificacion { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }

    public virtual tbCargos Cargo { get; set; }

    [Display(Name = "Estado Civil")]
    public virtual tbEstadosCiviles EsCiv { get; set; }

    [Display(Name = "Municipio")]
    public virtual tbMunicipios Munic_CodigoNavigation { get; set; }

    public virtual tbUsuarios Usuar_CreacionNavigation { get; set; }

    public virtual tbUsuarios Usuar_ModificacionNavigation { get; set; }

    public virtual ICollection<tbClases> tbClases { get; set; } = new List<tbClases>();

    public virtual ICollection<tbUsuarios> tbUsuarios { get; set; } = new List<tbUsuarios>();
}