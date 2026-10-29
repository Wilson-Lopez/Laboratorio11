using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

[Index(nameof(Nombre), IsUnique = true)]
public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Categoría")]
    [Required(ErrorMessage = "Ingrese el nombre de la categoría.")]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de {1} caracteres.")]
    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    // Navegación: eventos de la categoría
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
