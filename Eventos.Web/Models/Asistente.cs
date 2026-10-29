using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Nombres")]
    [Required(ErrorMessage = "Ingrese los nombres.")]
    [StringLength(100, ErrorMessage = "Los nombres no pueden tener más de {1} caracteres.")]
    public string Nombres { get; set; } = null!;

    [Display(Name = "DNI")]
    [Required(ErrorMessage = "Ingrese el DNI.")]
    [StringLength(8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos.")]
    public string Dni { get; set; } = null!;

    // Longitud máxima necesaria: SQL Server no permite un índice único sobre nvarchar(max)
    [Display(Name = "Correo")]
    [Required(ErrorMessage = "Ingrese el correo.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo con formato válido.")]
    [StringLength(100, ErrorMessage = "El correo no puede tener más de {1} caracteres.")]
    public string Correo { get; set; } = null!;

    // Migración 3 (AgregarTelefonoAsistente): opcional
    [Display(Name = "Teléfono")]
    [StringLength(9, ErrorMessage = "El teléfono no puede tener más de {1} caracteres.")]
    public string? Telefono { get; set; }

    public bool Activo { get; set; }

    // Navegación: inscripciones del asistente
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
