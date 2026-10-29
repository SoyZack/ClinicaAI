using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class Cita
{
    public int IdCita { get; set; }

    public int IdPaciente { get; set; }

    public int IdMedico { get; set; }

    public int? IdConversacion { get; set; }

    public DateTime FechaHora { get; set; }

    public string? Estado { get; set; }

    public string? OrigenAgendamiento { get; set; }

    public string? MotivoConsulta { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Conversacione? IdConversacionNavigation { get; set; }

    public virtual Medico IdMedicoNavigation { get; set; } = null!;

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
