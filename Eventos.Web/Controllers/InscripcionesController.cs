using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _repositorio;

    public InscripcionesController(InscripcionRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Inscripciones
    public async Task<IActionResult> Index()
    {
        return View(await _repositorio.ListarAsync());
    }

    // GET: Inscripciones/Create
    public async Task<IActionResult> Create()
    {
        var inscripcion = new Inscripcion();
        await CargarListasAsync(inscripcion);
        return View(inscripcion);
    }

    // POST: Inscripciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            var evento = await _repositorio.ObtenerEventoActivoAsync(inscripcion.EventoId);
            if (evento == null)
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento seleccionado no existe o fue eliminado.");
            else if (evento.Fecha < DateTime.Now)
                ModelState.AddModelError(nameof(Inscripcion.EventoId),
                    $"El evento \"{evento.Titulo}\" ya pasó ({evento.Fecha:dd/MM/yyyy HH:mm}).");
            else if (await _repositorio.ContarConfirmadasAsync(evento.Id) >= evento.Aforo)
                ModelState.AddModelError(nameof(Inscripcion.EventoId),
                    $"El aforo del evento \"{evento.Titulo}\" está completo ({evento.Aforo} inscritos).");

            if (!await _repositorio.ExisteAsistenteActivoAsync(inscripcion.AsistenteId))
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente seleccionado no existe o fue eliminado.");
            else if (await _repositorio.EstaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito (Confirmada) en este evento.");
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(inscripcion);
            return View(inscripcion);
        }

        await _repositorio.CrearAsync(inscripcion);
        TempData["Mensaje"] = "Inscripción registrada correctamente con estado Confirmada.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Inscripciones/Cancelar/5 (Estado = Cancelada)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (!await _repositorio.CancelarAsync(id))
        {
            TempData["Error"] = "La inscripción no existe o ya está cancelada.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Mensaje"] = "Inscripción cancelada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(Inscripcion inscripcion)
    {
        ViewBag.Eventos = new SelectList(await _repositorio.ListarEventosActivosAsync(),
            nameof(Evento.Id), nameof(Evento.Titulo), inscripcion.EventoId);
        ViewBag.Asistentes = new SelectList(await _repositorio.ListarAsistentesActivosAsync(),
            nameof(Asistente.Id), nameof(Asistente.Nombres), inscripcion.AsistenteId);
    }
}
