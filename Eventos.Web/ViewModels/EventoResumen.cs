namespace Eventos.Web.ViewModels;

// Fila del listado de eventos: datos del evento, su categoría, inscritos confirmados y cupos
public class EventoResumen
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = null!;
    public string Modalidad { get; set; } = null!;
    public int Aforo { get; set; }
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = null!;
    public int Inscritos { get; set; }

    public int CuposDisponibles => Aforo - Inscritos;
}
