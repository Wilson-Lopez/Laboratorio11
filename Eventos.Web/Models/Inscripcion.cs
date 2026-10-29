using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Inscripcion
{
    // Valores permitidos para Estado
    public const string Confirmada = "Confirmada";
    public const string Cancelada = "Cancelada";

    [Key]
    public int Id { get; set; }

    [Display(Name = "Evento")]
    [Required(ErrorMessage = "Seleccione un evento.")]
    public int EventoId { get; set; }

    [Display(Name = "Asistente")]
    [Required(ErrorMessage = "Seleccione un asistente.")]
    public int AsistenteId { get; set; }

    [Display(Name = "Fecha de inscripción")]
    public DateTime FechaInscripcion { get; set; }

    [Display(Name = "Estado")]
    [Required]
    [StringLength(20)]
    [RegularExpression("^(Confirmada|Cancelada)$", ErrorMessage = "El estado debe ser Confirmada o Cancelada.")]
    public string Estado { get; set; } = Confirmada;

    // Navegación: evento inscrito (FK EventoId)
    [ForeignKey(nameof(EventoId))]
    public Evento? Evento { get; set; }

    // Navegación: asistente inscrito (FK AsistenteId)
    [ForeignKey(nameof(AsistenteId))]
    public Asistente? Asistente { get; set; }
}
