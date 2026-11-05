using ClinicaAI.Models.DB;
using ClinicaAI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace ClinicaAI.Controllers;

[Authorize(Roles = "Administrador,Médico")]
public class MedicosController(ClinicaAiContext db) : Controller
{
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        var query = db.Medicos.AsNoTracking().Include(m => m.IdEspecialidadNavigation).AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(m => (m.Nombre + " " + m.Apellido).Contains(buscar) || m.IdEspecialidadNavigation.Nombre.Contains(buscar));
        return View(await query.OrderBy(m => m.Apellido).ToListAsync());
    }
    public async Task<IActionResult> Crear()
    {
        var model = new MedicoForm(); await Opciones(model); return View("Formulario", model);
    }
    public async Task<IActionResult> Editar(int id)
    {
        var m = await db.Medicos.FindAsync(id);
        if (m == null) return NotFound();
        var model = new MedicoForm { IdMedico = m.IdMedico, Nombre = m.Nombre, Apellido = m.Apellido, IdEspecialidad = m.IdEspecialidad, Telefono = m.Telefono, Email = m.Email, Estado = m.Estado ?? "activo" };
        await Opciones(model); return View("Formulario", model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(MedicoForm model)
    {
        if (!await db.Especialidades.AnyAsync(e => e.IdEspecialidad == model.IdEspecialidad))
            ModelState.AddModelError(nameof(model.IdEspecialidad), "Seleccione una especialidad existente.");
        if (await db.Medicos.AnyAsync(m => m.IdMedico != model.IdMedico && m.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "El correo ya está registrado. Si está vacío, indique un correo distinto.");
        if (!ModelState.IsValid) { await Opciones(model); return View("Formulario", model); }
        var m = model.IdMedico == 0 ? new Medico() : await db.Medicos.FindAsync(model.IdMedico);
        if (m == null) return NotFound();
        m.Nombre = model.Nombre.Trim(); m.Apellido = model.Apellido.Trim(); m.IdEspecialidad = model.IdEspecialidad;
        m.Telefono = model.Telefono; m.Email = model.Email; m.Estado = model.Estado;
        if (model.IdMedico == 0) db.Medicos.Add(m);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { ModelState.AddModelError("", "No se pudo guardar. Revise el correo y la especialidad."); await Opciones(model); return View("Formulario", model); }
        TempData["Exito"] = "Médico guardado correctamente.";
        return RedirectToAction(nameof(Index));
    }
    private async Task Opciones(MedicoForm model) => model.Especialidades = await db.Especialidades.AsNoTracking().OrderBy(e => e.Nombre).Select(e => new SelectListItem(e.Nombre, e.IdEspecialidad.ToString())).ToListAsync();

    public IActionResult NotificacionesMedico()
    {
        return View();
    }
}
