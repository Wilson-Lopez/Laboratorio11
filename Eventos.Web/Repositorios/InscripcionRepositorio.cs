using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio
{
    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    // Inscripciones con su evento y asistente (más recientes primero)
    public async Task<List<Inscripcion>> ListarAsync()
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    // Datos de la lista desplegable de eventos
    public async Task<List<Evento>> ListarEventosActivosAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    // Datos de la lista desplegable de asistentes
    public async Task<List<Asistente>> ListarAsistentesActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerEventoActivoAsync(int eventoId)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);
    }

    public async Task<bool> ExisteAsistenteActivoAsync(int asistenteId)
    {
        return await _context.Asistentes
            .AsNoTracking()
            .AnyAsync(a => a.Id == asistenteId && a.Activo);
    }

    // ¿El asistente ya tiene una inscripción Confirmada en el evento?
    public async Task<bool> EstaInscritoAsync(int eventoId, int asistenteId)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .AnyAsync(i => i.EventoId == eventoId &&
                           i.AsistenteId == asistenteId &&
                           i.Estado == Inscripcion.Confirmada);
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .CountAsync(i => i.EventoId == eventoId && i.Estado == Inscripcion.Confirmada);
    }

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        inscripcion.FechaInscripcion = DateTime.Now;
        inscripcion.Estado = Inscripcion.Confirmada;
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    // Cancelar: Estado = Cancelada (solo inscripciones Confirmada)
    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.Id == id && i.Estado == Inscripcion.Confirmada);
        if (inscripcion == null)
            return false;

        inscripcion.Estado = Inscripcion.Cancelada;
        await _context.SaveChangesAsync();
        return true;
    }
}
