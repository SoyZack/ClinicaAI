using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaAI.Models.DB;

namespace ClinicaAI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CitasApiController : ControllerBase
    {
        private readonly ClinicaAiContext _context;
        private readonly ILogger<CitasApiController> _logger;
        
        public CitasApiController(ClinicaAiContext context, ILogger<CitasApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/CitasApi/Crear
        [HttpPost("Crear")]
        public async Task<IActionResult> CrearCita([FromBody] Cita nuevaCita)
        {
            try
            {
                if (nuevaCita == null)
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "Los datos de la cita son obligatorios."
                    });
                }

                // 1. REGLA DE NEGOCIO: Validar si el medico ya tiene una cita en la misma fecha y hora

                bool medicoOcupado = await _context.Citas.AnyAsync(C => C.IdMedico == nuevaCita.IdMedico && C.FechaHora == nuevaCita.FechaHora);
                if (medicoOcupado)
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "El medico ya tiene una cita programada en este horario exacto."
                    });
                }

                // 2. Establecer valores por defecto
                nuevaCita.FechaCreacion = DateTime.Now;
                nuevaCita.Estado = "programada";

                // 3. Guardar en la base de datos
                _context.Citas.Add(nuevaCita);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    exito = true,
                    mensaje = "Cita registrada exitosamente.",
                    idCita = nuevaCita.IdCita
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la cita.");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Ocurrió un error interno en el servidor."
                });
            }
        }

        // GET: api/CitasApi/Listar
        [HttpGet("Listar")]
        public async Task<IActionResult> ListarCitas()
        {
            try
            {
                var citas = _context.Citas
                    .Include(c => c.IdPacienteNavigation) // Ajusta el nombre de la navegacion segun Scanffold
                    .Include(c => c.IdMedicoNavigation)
                    .Select(c => new
                    {
                        c.IdCita,
                        c.FechaHora,
                        c.Estado,
                        c.MotivoConsulta,
                        Paciente = c.IdPacienteNavigation != null ? c.IdPacienteNavigation.Nombre : "Desconocido",
                        Medico = c.IdMedicoNavigation != null ? c.IdMedicoNavigation.Nombre : "Desconocido"
                    })
                    .ToListAsync();
                return Ok(citas);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las citas.");
                return StatusCode(500, new { exito = false, mensaje = "Error al obtener las citas." });
            }
        }
    }
}

