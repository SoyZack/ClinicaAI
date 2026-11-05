using ClinicaAI.Models;
using ClinicaAI.Models.DB;
using ClinicaAI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAI.Controllers;

public class HomeController(ClinicaAiContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Paciente"))
        {
            var email = User.Identity?.Name;
            var paciente = string.IsNullOrWhiteSpace(email)
                ? null
                : await db.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.Email == email);

            List<Cita> citas = [];
            if (paciente is not null)
            {
                citas = await db.Citas
                    .AsNoTracking()
                    .Include(c => c.IdMedicoNavigation)
                        .ThenInclude(m => m.IdEspecialidadNavigation)
                    .Where(c => c.IdPaciente == paciente.IdPaciente
                        && c.FechaHora >= DateTime.Now
                        && (c.Estado == "programada" || c.Estado == "confirmada"))
                    .OrderBy(c => c.FechaHora)
                    .ToListAsync();
            }

            return View("Paciente", new HomePacienteViewModel
            {
                Paciente = paciente,
                ProximasCitas = citas
            });
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier
        });
    }
}
