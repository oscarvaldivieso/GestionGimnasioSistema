﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionGimnasio.Models;

public partial class tbGimnasios
{
    public int Gimna_Id { get; set; }

    [Display(Name = "Gimnasio")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Gimna_Nombre { get; set; }


    [Display(Name = "Horario semanal apertura")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public TimeOnly Gimna_SemanaHoraApertura { get; set; }

    [Display(Name = "Horario semanal cierre")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public TimeOnly Gimna_SemanaHoraCierre { get; set; }
    [Display(Name = "Horario fin de semana apertura")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public TimeOnly Gimna_FinDeHoraApertura { get; set; }

    [Display(Name = "Horario fin de semana cierre")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public TimeOnly Gimna_FinDeHoraCierre { get; set; }

    [Display(Name = "Direccion")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Gimna_Direccion { get; set; }

    [Display(Name = "Municipio")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Munic_Codigo { get; set; }

    public bool? Gimna_Estado { get; set; }

    public int Usuar_Creacion { get; set; }

    public DateTime Fecha_Creacion { get; set; }

    public int? Usuar_Modificacion { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }


    [Display(Name = "Municipio")]
    public virtual tbMunicipios Munic_CodigoNavigation { get; set; }

    public virtual tbUsuarios Usuar_CreacionNavigation { get; set; }

    public virtual tbUsuarios Usuar_ModificacionNavigation { get; set; }

    public virtual ICollection<tbClases> tbClases { get; set; } = new List<tbClases>();
}