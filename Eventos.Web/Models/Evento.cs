using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Evento
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Título")]
    [Required(ErrorMessage = "Ingrese el título del evento.")]
    [StringLength(100, ErrorMessage = "El título no puede tener más de {1} caracteres.")]
    public string Titulo { get; set; } = null!;

    [Display(Name = "Fecha y hora")]
    [Required(ErrorMessage = "Ingrese la fecha del evento.")]
    [DataType(DataType.DateTime)]
    public DateTime Fecha { get; set; }

    [Display(Name = "Lugar")]
    [Required(ErrorMessage = "Ingrese el lugar del evento.")]
    [StringLength(100, ErrorMessage = "El lugar no puede tener más de {1} caracteres.")]
    public string Lugar { get; set; } = null!;

    [Display(Name = "Aforo")]
    [Required(ErrorMessage = "Ingrese el aforo.")]
    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre {1} y {2}.")]
    public int Aforo { get; set; }

    [Display(Name = "Precio (S/)")]
    [Required(ErrorMessage = "Ingrese el precio.")]
    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0", "99999999.99", ParseLimitsInInvariantCulture = true,
        ErrorMessage = "El precio no puede ser negativo (máximo 99999999.99).")]
    public decimal Precio { get; set; }

    // Migración 2 (AgregarModalidadEvento): valores Presencial o Virtual
    [Display(Name = "Modalidad")]
    [Required(ErrorMessage = "Seleccione la modalidad.")]
    [StringLength(20, ErrorMessage = "La modalidad no puede tener más de {1} caracteres.")]
    [RegularExpression("^(Presencial|Virtual)$", ErrorMessage = "La modalidad debe ser Presencial o Virtual.")]
    public string Modalidad { get; set; } = "Presencial";

    [Display(Name = "Categoría")]
    [Required(ErrorMessage = "Seleccione una categoría.")]
    public int CategoriaId { get; set; }

    public bool Activo { get; set; }

    // Navegación: categoría del evento (FK CategoriaId)
    [ForeignKey(nameof(CategoriaId))]
    public Categoria? Categoria { get; set; }

    // Navegación: inscripciones al evento
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
