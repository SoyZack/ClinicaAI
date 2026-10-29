using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class PreguntasFrecuentesFaq
{
    public int IdFaq { get; set; }

    public string? Categoria { get; set; }

    public string Pregunta { get; set; } = null!;

    public string Respuesta { get; set; } = null!;

    public string? PalabrasClave { get; set; }
}
