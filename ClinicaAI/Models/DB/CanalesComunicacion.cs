using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class CanalesComunicacion
{
    public int IdCanal { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Conversacione> Conversaciones { get; set; } = new List<Conversacione>();
}
