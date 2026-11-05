using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ClinicaAI.Services;

public class GeminiIaService
{
    private const string PromptSistema = """
        Eres el asistente virtual de triaje médico de CliniAI.
        Evalúa los síntomas del paciente, clasifica la prioridad y sugiere una especialidad.
        Devuelve exclusivamente un objeto JSON válido con esta forma:
        {"prioridad":"Baja|Alta|Urgencia","especialidad":"...","respuesta":"..."}
        Reglas:
        - "respuesta" debe ser un texto breve, claro y empático para el paciente, en español.
        - No des diagnósticos definitivos ni recetes medicamentos.
        - Si el paciente pregunta algo no relacionado con salud o medicina, indícalo amablemente en "respuesta", usa prioridad "Baja" y especialidad "No aplica".
        - Si detectas signos de alarma, usa prioridad "Urgencia" y recomienda atención inmediata.
        - Usa el historial de la conversación para mantener el contexto.
        """;

    private readonly string _apiKey;
    private readonly IReadOnlyList<string> _modelos;
    private readonly TimeSpan _timeout;
    private readonly ILogger<GeminiIaService> _logger;
    private readonly object _lock = new();
    private string? _ultimoModeloExitoso;

    public GeminiIaService(IConfiguration configuration, ILogger<GeminiIaService> logger)
    {
        _apiKey = configuration["GeminiSettings:ApiKey"] ?? "";
        _modelos = ObtenerModelos(configuration);
        _timeout = TimeSpan.FromSeconds(configuration.GetValue<int?>("GeminiSettings:TimeoutSeconds") ?? 15);
        _logger = logger;
        _logger.LogInformation("Modelos Gemini configurados (primario y respaldos): {Models}", string.Join(", ", _modelos));
    }

    public async Task<TriajeIaResultado> ObtenerRespuestaTriajeAsync(
        string mensajePaciente,
        IReadOnlyList<MensajeHistorialChat>? historial = null)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new GeminiIaException(null, "No se ha cargado GeminiSettings:ApiKey.");
        }

        GeminiIaException? ultimoError = null;
        var modelos = ObtenerOrdenModelos();

        for (var i = 0; i < modelos.Count; i++)
        {
            var modelo = modelos[i];
            try
            {
                var texto = await SolicitarRespuestaAsync(modelo, mensajePaciente, historial);
                MarcarModeloExitoso(modelo);

                if (i > 0)
                {
                    _logger.LogInformation("Gemini respondió usando el modelo de respaldo {Model}.", modelo);
                }

                return InterpretarRespuesta(texto);
            }
            catch (GeminiIaException ex) when (EsFalloDelModelo(ex))
            {
                ultimoError = ex;
                _logger.LogWarning(
                    "El modelo Gemini {Model} no está disponible (HTTP {StatusCode}; estado {ProviderStatus}). Se probará el siguiente modelo.",
                    modelo, ex.HttpStatusCode, ex.ProviderStatus);

                if (ex.HttpStatusCode == 404 && i == 0)
                {
                    await RegistrarModelosDisponiblesAsync();
                }
            }
        }

        throw ultimoError ?? new GeminiIaException(null, "No hay modelos Gemini configurados.");
    }

    private async Task<string> SolicitarRespuestaAsync(
        string modelo,
        string mensajePaciente,
        IReadOnlyList<MensajeHistorialChat>? historial)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(modelo)}:generateContent";

        try
        {
            using var timeoutCts = new CancellationTokenSource(_timeout);
            var options = new RestClientOptions
            {
                Timeout = _timeout,
                ThrowOnAnyError = false
            };
            using var client = new RestClient(options);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("x-goog-api-key", _apiKey);
            request.AddJsonBody(CrearPayload(mensajePaciente, historial));

            var response = await client.ExecuteAsync(request, timeoutCts.Token);

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                var texto = ExtraerTexto(response.Content);
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return texto;
                }

                throw new GeminiIaException((int)response.StatusCode, "RESPUESTA_VACIA");
            }

            if (response.StatusCode == 0)
            {
                _logger.LogWarning("Error de transporte con Gemini ({Error}).",
                    response.ErrorException?.Message ?? response.ErrorMessage ?? "sin detalle");
                throw new GeminiIaException(null, "TRANSPORT_ERROR");
            }

            var error = LeerErrorProveedor(response.Content);
            _logger.LogWarning("Gemini devolvió HTTP {StatusCode}; estado {ProviderStatus}; mensaje {ProviderMessage}",
                (int)response.StatusCode, error.Status, error.Message);
            throw new GeminiIaException(
                (int)response.StatusCode,
                error.Status ?? response.StatusDescription,
                error.RetryAfter);
        }
        catch (GeminiIaException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Tiempo de espera agotado al llamar al modelo Gemini {Model}.", modelo);
            throw new GeminiIaException(null, "TIMEOUT");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error de conexión con Gemini ({ExceptionType}): {Message}", ex.GetType().Name, ex.Message);
            throw new GeminiIaException(null, null);
        }
    }

    private static Dictionary<string, object> CrearPayload(
        string mensajePaciente,
        IReadOnlyList<MensajeHistorialChat>? historial)
    {
        var contents = new List<object>();

        foreach (var mensaje in historial ?? [])
        {
            if (string.IsNullOrWhiteSpace(mensaje.Texto))
            {
                continue;
            }

            contents.Add(new Dictionary<string, object>
            {
                ["role"] = string.Equals(mensaje.Emisor, "bot", StringComparison.OrdinalIgnoreCase) ? "model" : "user",
                ["parts"] = new[]
                {
                    new Dictionary<string, object> { ["text"] = mensaje.Texto.Trim() }
                }
            });
        }

        contents.Add(new Dictionary<string, object>
        {
            ["role"] = "user",
            ["parts"] = new[]
            {
                new Dictionary<string, object> { ["text"] = mensajePaciente }
            }
        });

        return new Dictionary<string, object>
        {
            ["systemInstruction"] = new Dictionary<string, object>
            {
                ["parts"] = new[]
                {
                    new Dictionary<string, object> { ["text"] = PromptSistema }
                }
            },
            ["contents"] = contents,
            ["generationConfig"] = new Dictionary<string, object>
            {
                ["temperature"] = 0.2,
                ["responseMimeType"] = "application/json",
                ["responseSchema"] = new Dictionary<string, object>
                {
                    ["type"] = "OBJECT",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["prioridad"] = new Dictionary<string, object>
                        {
                            ["type"] = "STRING",
                            ["enum"] = new[] { "Baja", "Alta", "Urgencia" }
                        },
                        ["especialidad"] = new Dictionary<string, object>
                        {
                            ["type"] = "STRING"
                        },
                        ["respuesta"] = new Dictionary<string, object>
                        {
                            ["type"] = "STRING"
                        }
                    },
                    ["required"] = new[] { "prioridad", "especialidad", "respuesta" }
                }
            }
        };
    }

    private static string? ExtraerTexto(string content)
    {
        var json = JObject.Parse(content);
        var candidatos = json["candidates"] as JArray;
        return candidatos?[0]?["content"]?["parts"]?[0]?["text"]?.Value<string>();
    }

    private static TriajeIaResultado InterpretarRespuesta(string texto)
    {
        var limpio = texto.Trim();

        if (limpio.StartsWith("```", StringComparison.Ordinal))
        {
            var salto = limpio.IndexOf('\n');
            if (salto >= 0)
            {
                limpio = limpio[(salto + 1)..];
            }

            if (limpio.EndsWith("```", StringComparison.Ordinal))
            {
                limpio = limpio[..^3];
            }

            limpio = limpio.Trim();
        }

        try
        {
            var json = JObject.Parse(limpio);
            var respuesta = ((string?)json["respuesta"])?.Trim();
            if (!string.IsNullOrWhiteSpace(respuesta))
            {
                var especialidad = ((string?)json["especialidad"])?.Trim();
                return new TriajeIaResultado(
                    respuesta,
                    NormalizarPrioridad((string?)json["prioridad"]),
                    string.IsNullOrWhiteSpace(especialidad) ? null : especialidad);
            }
        }
        catch (JsonException)
        {
        }

        return new TriajeIaResultado(texto.Trim(), ObtenerPrioridad(texto), null);
    }

    private static string NormalizarPrioridad(string? prioridad)
    {
        return prioridad?.Trim().ToLowerInvariant() switch
        {
            "urgencia" => "Urgencia",
            "alta" => "Alta",
            _ => "Baja"
        };
    }

    private static string ObtenerPrioridad(string respuesta)
    {
        if (Regex.IsMatch(respuesta, @"\b(urgencia|urgente|emergencia)\b", RegexOptions.IgnoreCase))
        {
            return "Urgencia";
        }

        if (Regex.IsMatch(respuesta, @"\b(alta|alto)\b", RegexOptions.IgnoreCase))
        {
            return "Alta";
        }

        return "Baja";
    }

    private IReadOnlyList<string> ObtenerOrdenModelos()
    {
        string? ultimo;
        lock (_lock)
        {
            ultimo = _ultimoModeloExitoso;
        }

        if (string.IsNullOrWhiteSpace(ultimo))
        {
            return _modelos;
        }

        var modelos = new List<string>(_modelos.Count) { ultimo };
        modelos.AddRange(_modelos.Where(m => !string.Equals(m, ultimo, StringComparison.OrdinalIgnoreCase)));
        return modelos;
    }

    private void MarcarModeloExitoso(string modelo)
    {
        lock (_lock)
        {
            _ultimoModeloExitoso = modelo;
        }
    }

    private static bool EsFalloDelModelo(GeminiIaException ex)
    {
        return ex.HttpStatusCode is null or 0 or 404 or 429 or 500 or 502 or 503 or 504
            || string.Equals(ex.ProviderStatus, "RESPUESTA_VACIA", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<string> ObtenerModelos(IConfiguration configuration)
    {
        var modelos = new List<string>();

        void Agregar(string? modelo)
        {
            var valor = modelo?.Trim();
            if (!string.IsNullOrWhiteSpace(valor) && !modelos.Contains(valor, StringComparer.OrdinalIgnoreCase))
            {
                modelos.Add(valor);
            }
        }

        Agregar(configuration["GeminiSettings:Model"]);

        foreach (var respaldo in configuration.GetSection("GeminiSettings:FallbackModels").Get<string[]>() ?? [])
        {
            Agregar(respaldo);
        }

        if (modelos.Count == 0)
        {
            modelos.Add("gemini-3.6-flash");
        }

        return modelos;
    }

    private async Task RegistrarModelosDisponiblesAsync()
    {
        try
        {
            var request = new RestRequest("https://generativelanguage.googleapis.com/v1beta/models", Method.Get);
            request.AddHeader("x-goog-api-key", _apiKey);

            using var client = new RestClient(new RestClientOptions { Timeout = TimeSpan.FromSeconds(10) });
            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                _logger.LogWarning("No se pudo consultar la lista de modelos Gemini. HTTP {StatusCode}.", (int)response.StatusCode);
                return;
            }

            var models = JObject.Parse(response.Content)["models"] as JArray;
            var available = models?
                .Where(model => model["supportedGenerationMethods"] is JArray methods &&
                                methods.Any(method => string.Equals((string?)method, "generateContent", StringComparison.OrdinalIgnoreCase)))
                .Select(model => (string?)model["name"])
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray() ?? [];

            _logger.LogWarning("Modelos Gemini que admiten generateContent para esta clave: {Models}",
                available.Length > 0 ? string.Join(", ", available) : "ninguno devuelto");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("No se pudo obtener la lista de modelos Gemini ({ExceptionType}).", ex.GetType().Name);
        }
    }

    private static GeminiErrorInfo LeerErrorProveedor(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new GeminiErrorInfo(null, null, null);
        }

        try
        {
            var error = JObject.Parse(content)["error"];
            var status = (string?)error?["status"];
            var message = (string?)error?["message"];
            var retryAfter = ObtenerRetryAfter(error);
            return new GeminiErrorInfo(status, message, retryAfter);
        }
        catch (JsonException)
        {
            return new GeminiErrorInfo(null, null, null);
        }
    }

    private static TimeSpan? ObtenerRetryAfter(JToken? error)
    {
        var details = error?["details"] as JArray;
        var retryInfo = details?.FirstOrDefault(detail =>
            ((string?)detail["@type"])?.EndsWith("RetryInfo", StringComparison.OrdinalIgnoreCase) == true);

        var retryDelay = (string?)retryInfo?["retryDelay"];
        if (!string.IsNullOrWhiteSpace(retryDelay) && TryParseGoogleDuration(retryDelay, out var retryAfter))
        {
            return retryAfter;
        }

        var message = (string?)error?["message"];
        if (!string.IsNullOrWhiteSpace(message))
        {
            var match = Regex.Match(message, @"Please retry in ([0-9hms.]+)", RegexOptions.IgnoreCase);
            if (match.Success && TryParseGoogleDuration(match.Groups[1].Value, out var retryFromMessage))
            {
                return retryFromMessage;
            }
        }

        return null;
    }

    private static bool TryParseGoogleDuration(string value, out TimeSpan result)
    {
        result = default;
        var valorLimpio = value.Trim().TrimEnd('.');
        var match = Regex.Match(valorLimpio, @"^(?:(\d+)h)?(?:(\d+)m)?(?:([\d.]+)s)?$", RegexOptions.IgnoreCase);
        if (!match.Success || string.IsNullOrWhiteSpace(match.Value))
        {
            return false;
        }

        var horas = match.Groups[1].Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : 0;
        var minutos = match.Groups[2].Success ? int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture) : 0;
        var segundos = match.Groups[3].Success ? double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture) : 0;
        var total = TimeSpan.FromSeconds(horas * 3600 + minutos * 60 + segundos);

        if (total <= TimeSpan.Zero)
        {
            return false;
        }

        result = total;
        return true;
    }

    private sealed record GeminiErrorInfo(string? Status, string? Message, TimeSpan? RetryAfter);
}

public sealed record MensajeHistorialChat(string Emisor, string Texto);

public sealed record TriajeIaResultado(string Respuesta, string Prioridad, string? Especialidad);

public sealed class GeminiIaException : Exception
{
    public int? HttpStatusCode { get; }
    public string? ProviderStatus { get; }
    public TimeSpan? RetryAfter { get; }
    public bool IsTransient => HttpStatusCode is null or 0 or 429 or 500 or 502 or 503 or 504;

    public GeminiIaException(int? httpStatusCode, string? providerStatus, TimeSpan? retryAfter = null)
    {
        HttpStatusCode = httpStatusCode;
        ProviderStatus = providerStatus;
        RetryAfter = retryAfter;
    }
}
