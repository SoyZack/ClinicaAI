using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class Medico
{
    public int IdMedico { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public int IdEspecialidad { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual Especialidade IdEspecialidadNavigation { get; set; } = null!;
}
