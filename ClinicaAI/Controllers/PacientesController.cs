using ClinicaAI.Models.DB;
using ClinicaAI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ClinicaAI.Controllers;

[Authorize]
public class PacientesController(ClinicaAiContext db) : Controller
{
    [Authorize(Roles = "Administrador,Médico")]
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        var query = db.Pacientes.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var telefonoBuscado = buscar.Replace("-", "");
            query = query.Where(p => (p.Nombre + " " + p.Apellido).Contains(buscar) || p.Telefono.Replace("-", "").Contains(telefonoBuscado) || (p.DocumentoIdentidad != null && p.DocumentoIdentidad.Contains(buscar)));
        }
        return View(await query.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToListAsync());
    }
    [Authorize(Roles = "Administrador,Médico")]
    public IActionResult Crear() => View("Formulario", new PacienteForm());

    [Authorize(Roles = "Administrador,Médico")]
    public async Task<IActionResult> Editar(int id)
    {
        var p = await db.Pacientes.FindAsync(id);
        if (p == null) return NotFound();
        return View("Formulario", new PacienteForm { IdPaciente = p.IdPaciente, Nombre = p.Nombre, Apellido = p.Apellido, Telefono = p.Telefono, Email = p.Email, DocumentoIdentidad = p.DocumentoIdentidad, FechaNacimiento = p.FechaNacimiento, Genero = p.Genero });
    }
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador,Médico")]
    public async Task<IActionResult> Guardar(PacienteForm model)
    {
        if (await db.Pacientes.AnyAsync(p => p.IdPaciente != model.IdPaciente && p.Telefono == model.Telefono))
            ModelState.AddModelError(nameof(model.Telefono), "El teléfono ya está registrado.");
        // SQL Server también aplica unicidad a NULL en estos índices.
        if (await db.Pacientes.AnyAsync(p => p.IdPaciente != model.IdPaciente && p.DocumentoIdentidad == model.DocumentoIdentidad))
            ModelState.AddModelError(nameof(model.DocumentoIdentidad), "El documento ya está registrado. Si está vacío, indique un documento distinto.");
        var email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        if (email != null && await db.Pacientes.AnyAsync(p => p.IdPaciente != model.IdPaciente && p.Email == email))
            ModelState.AddModelError(nameof(model.Email), "El correo ya está registrado en otro paciente.");
        if (!ModelState.IsValid) return View("Formulario", model);
        var p = model.IdPaciente == 0 ? new Paciente { FechaRegistro = DateTime.Now } : await db.Pacientes.FindAsync(model.IdPaciente);
        if (p == null) return NotFound();
        p.Nombre = model.Nombre.Trim(); p.Apellido = model.Apellido.Trim(); p.Telefono = model.Telefono; p.Email = email;
        p.DocumentoIdentidad = model.DocumentoIdentidad; p.FechaNacimiento = model.FechaNacimiento; p.Genero = model.Genero;
        if (model.IdPaciente == 0) db.Pacientes.Add(p);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { ModelState.AddModelError("", "No se pudo guardar. Compruebe que el teléfono, documento y correo no estén repetidos."); return View("Formulario", model); }
        TempData["Exito"] = "Paciente guardado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Paciente")]
    public IActionResult ChatPaciente()
    {
        return View();
    }
}