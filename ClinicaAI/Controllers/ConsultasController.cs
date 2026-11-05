using ClinicaAI.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAI.Controllers;

[Authorize(Roles = "Administrador,Médico")]
public class ConsultasController(ClinicaAiContext db) : Controller
{
    public async Task<IActionResult> Index(int? idPaciente, string? buscar)
    {
        ViewData["IdPaciente"] = idPaciente;
        ViewData["Buscar"] = buscar;

        var query = db.TriajesChatbots
            .AsNoTracking()
            .Include(t => t.IdPacienteNavigation)
            .AsQueryable();

        if (idPaciente.HasValue)
        {
            query = query.Where(t => t.IdPaciente == idPaciente.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(t =>
                t.SintomasReportados.Contains(buscar) ||
                (t.RecomendacionBot != null && t.RecomendacionBot.Contains(buscar)) ||
                (t.IdPacienteNavigation != null &&
                 (t.IdPacienteNavigation.Nombre + " " + t.IdPacienteNavigation.Apellido).Contains(buscar)));
        }

        var consultas = await query
            .OrderByDescending(t => t.FechaTriaje)
            .ToListAsync();

        return View(consultas);
    }

    [HttpGet]
    public async Task<IActionResult> Detalles(int id)
    {
        var consulta = await db.TriajesChatbots
            .AsNoTracking()
            .Include(t => t.IdPacienteNavigation)
            .FirstOrDefaultAsync(t => t.IdTriaje == id);

        return consulta == null ? NotFound() : View(consulta);
    }
}
