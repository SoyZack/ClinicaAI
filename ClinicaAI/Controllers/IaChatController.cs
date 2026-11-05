using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicaAI.Models.DB;
using ClinicaAI.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAI.Controllers
{
    [Authorize(Roles = "Paciente")]
    [Route("api/[controller]")]
    [ApiController]
    public class IaChatController : ControllerBase
    {
        private readonly GeminiIaService _geminiIaService;
        private readonly ClinicaAiContext _context;
        private readonly ILogger<IaChatController> _logger;
        private readonly bool _enableDemoFallback;

        public IaChatController(
            GeminiIaService geminiIaService,
            ClinicaAiContext context,
            ILogger<IaChatController> logger,
            IConfiguration configuration)
        {
            _geminiIaService = geminiIaService;
            _context = context;
            _logger = logger;
            _enableDemoFallback = configuration.GetValue<bool>("GeminiSettings:EnableDemoFallback");
        }

        // POST: api/IaChat/Preguntar
        [HttpPost("Preguntar")]
        public async Task<IActionResult> PreguntarAIChat([FromBody] ChatRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Mensaje))
                {
                    return BadRequest(new { exito = false, mensaje = "El mensaje no puede estar vacio." });
                }

                var email = User.Identity?.Name;
                if (string.IsNullOrWhiteSpace(email))
                {
                    return Unauthorized(new { exito = false, mensaje = "No se pudo identificar al paciente autenticado." });
                }

                var paciente = await _context.Pacientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Email == email);

                if (paciente == null)
                {
                    return StatusCode(403, new
                    {
                        exito = false,
                        mensaje = "Tu usuario no está vinculado a un expediente de paciente. Contacta a recepción."
                    });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    userId = email;
                }

                var conversacion = request.IdConversacion.HasValue
                    ? await _context.Conversaciones
                        .Include(c => c.Mensajes)
                        .FirstOrDefaultAsync(c => c.IdConversacion == request.IdConversacion.Value
                            && (c.IdPaciente == paciente.IdPaciente
                                || (c.IdPaciente == null && c.IdentificadorUsuarioCanal == userId)))
                    : null;

                if (request.IdConversacion.HasValue && conversacion == null)
                {
                    _logger.LogInformation(
                        "Se ignoró la conversación {ConversationId} porque no pertenece al paciente {PatientId}.",
                        request.IdConversacion.Value, paciente.IdPaciente);
                }

                if (conversacion == null)
                {
                    var canal = await _context.CanalesComunicacions
                        .FirstOrDefaultAsync(c => c.Nombre == "Chatbot Web");

                    if (canal == null)
                    {
                        return StatusCode(500, new { exito = false, mensaje = "No existe un canal configurado para el chatbot." });
                    }

                    conversacion = new Conversacione
                    {
                        IdCanal = canal.IdCanal,
                        IdPaciente = paciente.IdPaciente,
                        IdentificadorUsuarioCanal = userId,
                        Estado = "bot",
                        FechaInicio = DateTime.Now
                    };

                    _context.Conversaciones.Add(conversacion);
                }
                else if (conversacion.IdPaciente == null)
                {
                    conversacion.IdPaciente = paciente.IdPaciente;
                }

                var historial = conversacion.Mensajes
                    .Where(EsMensajeParaHistorial)
                    .OrderBy(m => m.IdMensaje)
                    .TakeLast(10)
                    .Select(m => new MensajeHistorialChat(m.Emisor, m.ContenidoTexto))
                    .ToList();

                TriajeIaResultado resultado;
                var esDemostracion = false;

                try
                {
                    resultado = await _geminiIaService.ObtenerRespuestaTriajeAsync(request.Mensaje.Trim(), historial);
                }
                catch (GeminiIaException ex) when (_enableDemoFallback && ex.IsTransient)
                {
                    esDemostracion = true;
                    resultado = new TriajeIaResultado(
                        "[MODO DEMOSTRACIÓN] Gemini no está disponible temporalmente. Esta respuesta no es una valoración médica ni un triaje real. En una consulta real, un profesional sanitario revisaría tus síntomas y te orientaría.",
                        "Baja",
                        null);
                    _logger.LogWarning("Se usó la respuesta de demostración porque Gemini no está disponible temporalmente.");
                }

                conversacion.Mensajes.Add(new Mensaje
                {
                    Emisor = "paciente",
                    ContenidoTexto = request.Mensaje.Trim(),
                    TipoContenido = "texto",
                    FechaEnvio = DateTime.Now
                });

                conversacion.Mensajes.Add(new Mensaje
                {
                    Emisor = "bot",
                    ContenidoTexto = resultado.Respuesta,
                    TipoContenido = "texto",
                    FechaEnvio = DateTime.Now
                });

                if (!esDemostracion)
                {
                    conversacion.TriajesChatbots.Add(new TriajesChatbot
                    {
                        IdPaciente = paciente.IdPaciente,
                        SintomasReportados = request.Mensaje.Trim(),
                        NivelPrioridad = resultado.Prioridad,
                        RecomendacionBot = resultado.Respuesta,
                        FechaTriaje = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    exito = true,
                    idConversacion = conversacion.IdConversacion,
                    remitente = "IA",
                    respuesta = resultado.Respuesta,
                    nivelPrioridad = resultado.Prioridad,
                    especialidad = resultado.Especialidad,
                    esDemostracion
                });
            }
            catch (GeminiIaException ex)
            {
                _logger.LogWarning("No se pudo obtener respuesta de Gemini. HTTP {StatusCode}; estado {ProviderStatus}.",
                    ex.HttpStatusCode, ex.ProviderStatus);

                var statusCode = ex.HttpStatusCode switch
                {
                    429 => StatusCodes.Status429TooManyRequests,
                    0 or null or 500 or 502 or 503 or 504 => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status502BadGateway
                };

                return StatusCode(statusCode, new
                {
                    exito = false,
                    mensaje = ObtenerMensajeErrorGemini(ex),
                    reintentarEnSegundos = ex.RetryAfter.HasValue
                        ? (int?)Math.Ceiling(ex.RetryAfter.Value.TotalSeconds)
                        : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el mensaje del chatbot.");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Ocurrió un error al procesar la solicitud."
                });
            }
        }

        private static bool EsMensajeParaHistorial(Mensaje mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje.ContenidoTexto))
            {
                return false;
            }

            var inicio = mensaje.ContenidoTexto.TrimStart();
            return !inicio.StartsWith("[MODO DEMOSTRACIÓN]", StringComparison.OrdinalIgnoreCase)
                && !inicio.StartsWith("Lo siento, hubo un problema", StringComparison.OrdinalIgnoreCase)
                && !inicio.StartsWith("El servicio de IA no está configurado", StringComparison.OrdinalIgnoreCase);
        }

        private static string ObtenerMensajeErrorGemini(GeminiIaException error)
        {
            return error.HttpStatusCode switch
            {
                0 or null => string.Equals(error.ProviderStatus, "TIMEOUT", StringComparison.OrdinalIgnoreCase)
                    ? "Gemini tardó demasiado en responder. Vuelve a intentarlo en unos minutos."
                    : "No se pudo conectar con Gemini. Comprueba la configuración y la conexión a Internet.",
                400 => "Gemini rechazó la solicitud. Comprueba el modelo configurado y el formato de la petición.",
                401 or 403 => "Gemini rechazó la API key o no tiene permisos. Comprueba la clave y que Generative Language API esté habilitada.",
                404 => "El modelo de Gemini configurado no está disponible para esta API key.",
                429 => error.RetryAfter is { } retryAfter
                    ? $"Se agotó la cuota gratuita de Gemini. Podrás reintentar en aproximadamente {FormatearDuracion(retryAfter)}."
                    : "Se agotó la cuota gratuita de Gemini. Reintenta más tarde o revisa la facturación del proyecto de Google.",
                500 or 502 or 503 or 504 => "Gemini está temporalmente saturado o no disponible. Vuelve a intentarlo en unos minutos.",
                _ => $"Gemini devolvió un error HTTP {error.HttpStatusCode} ({error.ProviderStatus ?? "sin detalle"}). Revisa los logs de la aplicación."
            };
        }

        private static string FormatearDuracion(TimeSpan duracion)
        {
            if (duracion.TotalHours >= 1)
            {
                return $"{(int)duracion.TotalHours} h {duracion.Minutes} min";
            }

            if (duracion.TotalMinutes >= 1)
            {
                return $"{(int)duracion.TotalMinutes} min";
            }

            return $"{Math.Max(1, (int)Math.Ceiling(duracion.TotalSeconds))} s";
        }

        public class ChatRequestDto
        {
            public string Mensaje { get; set; } = null!;
            public int? IdConversacion { get; set; }
        }
    }
}
