using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    // Eventos activos con su categoría, filtro opcional por categoría y orden por fecha.
    // Inscritos = inscripciones confirmadas; los cupos disponibles se calculan con el aforo.
    public async Task<List<EventoResumen>> ListarActivosAsync(int? categoriaId)
    {
        var consulta = _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo);

        if (categoriaId.HasValue)
            consulta = consulta.Where(e => e.CategoriaId == categoriaId.Value);

        return await consulta
            .OrderBy(e => e.Fecha)
            .Select(e => new EventoResumen
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Fecha = e.Fecha,
                Lugar = e.Lugar,
                Modalidad = e.Modalidad,
                Aforo = e.Aforo,
                Precio = e.Precio,
                Categoria = e.Categoria!.Nombre,
                Inscritos = e.Inscripciones.Count(i => i.Estado == Inscripcion.Confirmada)
            })
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerActivoAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    // Datos de la lista desplegable de categorías
    public async Task<List<Categoria>> ListarCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task CrearAsync(Evento evento)
    {
        evento.Activo = true;
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Evento evento)
    {
        var actual = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == evento.Id && e.Activo);
        if (actual == null)
            return false;

        actual.Titulo = evento.Titulo;
        actual.Fecha = evento.Fecha;
        actual.Lugar = evento.Lugar;
        actual.Aforo = evento.Aforo;
        actual.Precio = evento.Precio;
        actual.Modalidad = evento.Modalidad;
        actual.CategoriaId = evento.CategoriaId;
        await _context.SaveChangesAsync();
        return true;
    }

    // Eliminación lógica: Activo = false
    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento == null)
            return false;

        evento.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
