using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class Paciente
{
    public int IdPaciente { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? DocumentoIdentidad { get; set; }

    public string Telefono { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Genero { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Conversacione> Conversaciones { get; set; } = new List<Conversacione>();

    public virtual ICollection<TriajesChatbot> TriajesChatbots { get; set; } = new List<TriajesChatbot>();
}
