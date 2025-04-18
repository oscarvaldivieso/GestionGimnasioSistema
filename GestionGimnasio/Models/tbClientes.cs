﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionGimnasio.Models;

public partial class tbClientes
{
    public int Clien_id { get; set; }

    [Display(Name = "Identidad")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Clien_Identidad { get; set; }

    [Display(Name = "Primer Nombre")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Clien_PrimerNombre { get; set; }

    [Display(Name = "Segundo Nombre")]

    public string Clien_SegundoNombre { get; set; }

    [Display(Name = "Primer Apellido")]
    [Required(ErrorMessage = "El campo {0} es requerido")]

    public string Clien_PrimerApellido { get; set; }


    [Display(Name = "Segundo Apellido")]
    public string Clien_SegundoApellido { get; set; }


    [Display(Name = "Sexo")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Clien_Sexo { get; set; }


    [Display(Name = "Estado Civil")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public int EsCiv_Id { get; set; }

    [Display(Name = "Fecha de Nacimiento")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public DateTime Clien_FechaNacimiento { get; set; }

    [Display(Name = "Direccion")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Clien_Direccion { get; set; }

    [Display(Name = "Municipio")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Munic_Codigo { get; set; }

    [Display(Name = "¿Es miembro activo?")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public bool Clien_esMiembroActivo { get; set; }

    public bool? Clien_Estado { get; set; }

    public int Usuar_Creacion { get; set; }

    public DateTime Fecha_Creacion { get; set; }

    public int? Usuar_Modificacion { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }

    public virtual tbEstadosCiviles EsCiv { get; set; }

    [Display(Name = "Municipio")]
    public virtual tbMunicipios Munic_CodigoNavigation { get; set; }

    public virtual tbUsuarios Usuar_CreacionNavigation { get; set; }

    public virtual tbUsuarios Usuar_ModificacionNavigation { get; set; }

    public virtual ICollection<tbEjerciciosRutinasPorCliente> tbEjerciciosRutinasPorCliente { get; set; } = new List<tbEjerciciosRutinasPorCliente>();

    public virtual ICollection<tbPagos> tbPagos { get; set; } = new List<tbPagos>();
}