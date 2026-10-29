using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class Especialidade
{
    public int IdEspecialidad { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();
}
