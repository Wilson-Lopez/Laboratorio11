using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _repositorio;

    public AsistentesController(AsistenteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Asistentes
    public async Task<IActionResult> Index()
    {
        return View(await _repositorio.ListarActivosAsync());
    }

    // GET: Asistentes/Create
    public IActionResult Create()
    {
        return View(new Asistente());
    }

    // POST: Asistentes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Dni,Correo,Telefono")] Asistente asistente)
    {
        Normalizar(asistente);

        if (!string.IsNullOrEmpty(asistente.Dni) && await _repositorio.ExisteDniAsync(asistente.Dni))
            ModelState.AddModelError(nameof(Asistente.Dni), $"Ya existe un asistente con el DNI {asistente.Dni}.");

        if (!string.IsNullOrEmpty(asistente.Correo) && await _repositorio.ExisteCorreoAsync(asistente.Correo))
            ModelState.AddModelError(nameof(Asistente.Correo), $"Ya existe un asistente con el correo {asistente.Correo}.");

        if (!ModelState.IsValid)
            return View(asistente);

        await _repositorio.CrearAsync(asistente);
        TempData["Mensaje"] = $"Asistente {asistente.Nombres} registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static void Normalizar(Asistente asistente)
    {
        asistente.Nombres = asistente.Nombres?.Trim()!;
        asistente.Dni = asistente.Dni?.Trim()!;
        asistente.Correo = asistente.Correo?.Trim().ToLowerInvariant()!;
        asistente.Telefono = string.IsNullOrWhiteSpace(asistente.Telefono) ? null : asistente.Telefono.Trim();
    }
}
