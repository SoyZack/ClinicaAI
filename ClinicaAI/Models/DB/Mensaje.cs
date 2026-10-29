using System;
using System.Collections.Generic;

namespace ClinicaAI.Models.DB;

public partial class Mensaje
{
    public int IdMensaje { get; set; }

    public int IdConversacion { get; set; }

    public string Emisor { get; set; } = null!;

    public string ContenidoTexto { get; set; } = null!;

    public string? TipoContenido { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public virtual Conversacione IdConversacionNavigation { get; set; } = null!;
}
