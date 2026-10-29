using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class Conversacione
{
    public int IdConversacion { get; set; }

    public int? IdPaciente { get; set; }

    public int IdCanal { get; set; }

    public string IdentificadorUsuarioCanal { get; set; } = null!;

    public string? Estado { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual CanalesComunicacion IdCanalNavigation { get; set; } = null!;

    public virtual Paciente? IdPacienteNavigation { get; set; }

    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    public virtual ICollection<TriajesChatbot> TriajesChatbots { get; set; } = new List<TriajesChatbot>();
}
