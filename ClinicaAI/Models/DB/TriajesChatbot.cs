using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class TriajesChatbot
{
    public int IdTriaje { get; set; }

    public int IdConversacion { get; set; }

    public int? IdPaciente { get; set; }

    public string SintomasReportados { get; set; } = null!;

    public string? NivelPrioridad { get; set; }

    public string? RecomendacionBot { get; set; }

    public DateTime? FechaTriaje { get; set; }

    public virtual Conversacione IdConversacionNavigation { get; set; } = null!;

    public virtual Paciente? IdPacienteNavigation { get; set; }
}
