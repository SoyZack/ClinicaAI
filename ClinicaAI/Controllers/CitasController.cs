using ClinicaAI.Models.DB;
using ClinicaAI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAI.Controllers;

[Authorize(Roles = "Administrador,Médico")]
public class CitasController(ClinicaAiContext db, ILogger<CitasController> logger) : Controller
{
    public async Task<IActionResult> Index(DateTime? fecha)
    {
        ViewData["Fecha"] = fecha?.ToString("yyyy-MM-dd");

        var query = db.Citas
            .AsNoTracking()
            .Include(c => c.IdPacienteNavigation)
            .Include(c => c.IdMedicoNavigation)
            .AsQueryable();

        if (fecha.HasValue)
        {
            var inicio = fecha.Value.Date;
            var fin = inicio.AddDays(1);
            query = query.Where(c => c.FechaHora >= inicio && c.FechaHora < fin);
        }

        var citas = await query
            .OrderBy(c => c.FechaHora)
            .ToListAsync();

        return View(citas);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new CitaForm();
        await CargarOpciones(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CitaForm model)
    {
        if (model.FechaHora.HasValue && await db.Citas.AnyAsync(c =>
                c.IdMedico == model.IdMedico && c.FechaHora == model.FechaHora.Value))
        {
            ModelState.AddModelError(nameof(model.FechaHora), "El médico ya tiene una cita en ese horario.");
        }

        if (!await db.Pacientes.AnyAsync(p => p.IdPaciente == model.IdPaciente))
        {
            ModelState.AddModelError(nameof(model.IdPaciente), "Seleccione un paciente existente.");
        }

        if (!await db.Medicos.AnyAsync(m => m.IdMedico == model.IdMedico && m.Estado == "activo"))
        {
            ModelState.AddModelError(nameof(model.IdMedico), "Seleccione un médico activo existente.");
        }

        if (!ModelState.IsValid)
        {
            await CargarOpciones(model);
            return View(model);
        }

        var cita = new Cita
        {
            IdPaciente = model.IdPaciente,
            IdMedico = model.IdMedico,
            FechaHora = model.FechaHora!.Value,
            MotivoConsulta = model.MotivoConsulta.Trim(),
            FechaCreacion = DateTime.Now,
            Estado = "programada",
            OrigenAgendamiento = "recepcion_web"
        };

        db.Citas.Add(cita);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Error al guardar la cita.");
            ModelState.AddModelError(string.Empty, "No se pudo guardar la cita. Compruebe los datos e inténtelo de nuevo.");
            await CargarOpciones(model);
            return View(model);
        }

        TempData["Exito"] = "Cita registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarOpciones(CitaForm model)
    {
        model.Pacientes = await db.Pacientes
            .AsNoTracking()
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .Select(p => new SelectListItem
            {
                Text = p.Nombre + " " + p.Apellido,
                Value = p.IdPaciente.ToString()
            })
            .ToListAsync();

        model.Medicos = await db.Medicos
            .AsNoTracking()
            .Where(m => m.Estado == "activo")
            .OrderBy(m => m.Apellido)
            .ThenBy(m => m.Nombre)
            .Select(m => new SelectListItem
            {
                Text = m.Nombre + " " + m.Apellido,
                Value = m.IdMedico.ToString()
            })
            .ToListAsync();
    }
}
