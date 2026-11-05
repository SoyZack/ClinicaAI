using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaAI.Models.DB; 
using Microsoft.AspNetCore.Authorization;

namespace ClinicaAI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Médico")]
    public class ConsultasApiController : ControllerBase
    {
        private readonly ClinicaAiContext _context;
        private readonly ILogger<ConsultasApiController> _logger;

        public ConsultasApiController(ClinicaAiContext context, ILogger<ConsultasApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. POST: api/ConsultasApi/Registrar
        // Registra el resultado o la atencion clinica de un paciente

        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarConsulta([FromBody] TriajesChatbot nuevaConsulta)
        {
            try
            {
                if (nuevaConsulta == null)
                {
                    return BadRequest(new { exito = false, mensaje = "Los datos de la consulta no son validos." });
                }

                // Asignar fecha actual en el servidor si no viene establecida
                if(nuevaConsulta.FechaTriaje == null)
                {
                    nuevaConsulta.FechaTriaje = DateTime.Now;
                }

                _context.TriajesChatbots.Add(nuevaConsulta);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    exito = true,
                    mensaje = "Consulta registrada exitosamente.",
                    idTriaje = nuevaConsulta.IdTriaje
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la consulta clínica.");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al guardar la consulta clínica en la base de datos."
                });
            }
        }

        // 2. GET: api/ConsultasApi/Listar
        // Devuelve el historial de todas las consultas realizadas
        [HttpGet("Listar")]
        public async Task<IActionResult> ListarConsultas()
        {
            try
            {
                var consultas = await _context.TriajesChatbots
                    .Include(t => t.IdPacienteNavigation)
                    .Include(t => t.IdConversacionNavigation)
                    .Select(t => new
                    {
                        t.IdTriaje,
                        t.SintomasReportados,
                        t.NivelPrioridad,
                        t.RecomendacionBot,
                        t.FechaTriaje,
                        Paciente = t.IdPacienteNavigation != null
                            ? t.IdPacienteNavigation.Nombre + " " + t.IdPacienteNavigation.Apellido
                            : "Paciente no registrado",
                        Telefono = t.IdPacienteNavigation != null ? t.IdPacienteNavigation.Telefono : "N/A"
                    })
                    .OrderByDescending(t => t.FechaTriaje)
                    .ToListAsync();

                return Ok(consultas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar el historial de consultas.");
                return StatusCode(500, new { exito = false, mensaje = "Error al consultar el historial." });
            }
        }

        // 3. GET: api/ConsultaApi/Paciente/5
        // Historial medico de un paciente especifico
        [HttpGet("Paciente/{idPaciente}")]
        public async Task<IActionResult> ConsultasPorPaciente(int idPaciente)
        {
            try
            {
                var historial = await _context.TriajesChatbots
                    .Where(t => t.IdPaciente == idPaciente)
                    .Select(t => new
                    {
                        t.IdTriaje,
                        t.SintomasReportados,
                        t.NivelPrioridad,
                        t.RecomendacionBot,
                        t.FechaTriaje
                    })
                    .OrderByDescending(t => t.FechaTriaje)
                    .ToListAsync();
                return Ok(historial);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial del paciente {IdPaciente}.", idPaciente);
                return StatusCode(500, new { exito = false, mensaje = "Error al obtener historial del paciente." });
            }
        }
    }
}
