using ClinicaAI.Models.DB;
using ClinicaAI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAI.Controllers;

[Authorize(Roles = "Administrador,Médico")]
public class EspecialidadesController(ClinicaAiContext db, ILogger<EspecialidadesController> logger) : Controller
{
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;

        var query = db.Especialidades
            .AsNoTracking()
            .Include(e => e.Medicos)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(e =>
                e.Nombre.Contains(buscar) ||
                (e.Descripcion != null && e.Descripcion.Contains(buscar)));
        }

        return View(await query.OrderBy(e => e.Nombre).ToListAsync());
    }

    public IActionResult Crear() => View("Formulario", new EspecialidadForm());

    public async Task<IActionResult> Editar(int id)
    {
        var especialidad = await db.Especialidades.FindAsync(id);
        if (especialidad == null)
        {
            return NotFound();
        }

        return View("Formulario", new EspecialidadForm
        {
            IdEspecialidad = especialidad.IdEspecialidad,
            Nombre = especialidad.Nombre,
            Descripcion = especialidad.Descripcion
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(EspecialidadForm model)
    {
        model.Nombre = model.Nombre?.Trim() ?? "";
        model.Descripcion = string.IsNullOrWhiteSpace(model.Descripcion) ? null : model.Descripcion.Trim();

        if (await db.Especialidades.AnyAsync(e => e.IdEspecialidad != model.IdEspecialidad && e.Nombre == model.Nombre))
        {
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe una especialidad con ese nombre.");
        }

        if (!ModelState.IsValid)
        {
            return View("Formulario", model);
        }

        var especialidad = model.IdEspecialidad == 0
            ? new Especialidade()
            : await db.Especialidades.FindAsync(model.IdEspecialidad);

        if (especialidad == null)
        {
            return NotFound();
        }

        especialidad.Nombre = model.Nombre;
        especialidad.Descripcion = model.Descripcion;

        if (model.IdEspecialidad == 0)
        {
            db.Especialidades.Add(especialidad);
        }

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Error al guardar la especialidad.");
            ModelState.AddModelError(string.Empty, "No se pudo guardar. Verifique que el nombre no esté duplicado.");
            return View("Formulario", model);
        }

        TempData["Exito"] = "Especialidad guardada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var especialidad = await db.Especialidades.FindAsync(id);
        if (especialidad == null)
        {
            return NotFound();
        }

        if (await db.Medicos.AnyAsync(m => m.IdEspecialidad == id))
        {
            TempData["Error"] = "No se puede eliminar: hay médicos asignados a esta especialidad.";
            return RedirectToAction(nameof(Index));
        }

        db.Especialidades.Remove(especialidad);

        try
        {
            await db.SaveChangesAsync();
            TempData["Exito"] = "Especialidad eliminada correctamente.";
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Error al eliminar la especialidad.");
            TempData["Error"] = "No se pudo eliminar la especialidad.";
        }

        return RedirectToAction(nameof(Index));
    }
}
