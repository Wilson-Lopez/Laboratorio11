using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private static readonly string[] Modalidades = ["Presencial", "Virtual"];

    private readonly EventoRepositorio _repositorio;

    public EventosController(EventoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Eventos?categoriaId=1
    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = new SelectList(await _repositorio.ListarCategoriasActivasAsync(),
            nameof(Categoria.Id), nameof(Categoria.Nombre), categoriaId);
        return View(await _repositorio.ListarActivosAsync(categoriaId));
    }

    // GET: Eventos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var evento = await _repositorio.ObtenerActivoAsync(id);
        if (evento == null)
            return NotFound();

        return View(evento);
    }

    // GET: Eventos/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync(null);
        return View();
    }

    // POST: Eventos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Fecha,Lugar,Aforo,Precio,Modalidad,CategoriaId")] Evento evento)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(evento);
            return View(evento);
        }

        await _repositorio.CrearAsync(evento);
        TempData["Mensaje"] = $"Evento \"{evento.Titulo}\" registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Eventos/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _repositorio.ObtenerActivoAsync(id);
        if (evento == null)
            return NotFound();

        await CargarListasAsync(evento);
        return View(evento);
    }

    // POST: Eventos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,Modalidad,CategoriaId")] Evento evento)
    {
        if (id != evento.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(evento);
            return View(evento);
        }

        if (!await _repositorio.ActualizarAsync(evento))
            return NotFound();

        TempData["Mensaje"] = $"Evento \"{evento.Titulo}\" actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Eventos/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _repositorio.ObtenerActivoAsync(id);
        if (evento == null)
            return NotFound();

        return View(evento);
    }

    // POST: Eventos/Delete/5 (eliminación lógica)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!ModelState.IsValid)
        {
            var evento = await _repositorio.ObtenerActivoAsync(id);
            return evento == null ? NotFound() : View(evento);
        }

        if (!await _repositorio.EliminarAsync(id))
            return NotFound();

        TempData["Mensaje"] = "Evento eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(Evento? evento)
    {
        ViewBag.Categorias = new SelectList(await _repositorio.ListarCategoriasActivasAsync(),
            nameof(Categoria.Id), nameof(Categoria.Nombre), evento?.CategoriaId);
        ViewBag.Modalidades = new SelectList(Modalidades, evento?.Modalidad ?? "Presencial");
    }
}
