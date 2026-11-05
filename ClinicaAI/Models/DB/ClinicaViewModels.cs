using System.ComponentModel.DataAnnotations;
using ClinicaAI.Models.DB;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ClinicaAI.Models.ViewModels;

public class PacienteForm : IValidatableObject
{
    public int IdPaciente { get; set; }
    [Required, StringLength(100), RegularExpression("^[A-Za-z\\u00C0-\\u024F\\u1E00-\\u1EFF '’-]+$", ErrorMessage = "No se pueden colocar números en el nombre.")]
    public string Nombre { get; set; } = "";
    [Required, StringLength(100), RegularExpression("^[A-Za-z\\u00C0-\\u024F\\u1E00-\\u1EFF '’-]+$", ErrorMessage = "No se pueden colocar números en el apellido.")]
    public string Apellido { get; set; } = "";
    [Required(ErrorMessage = "El documento de identidad es obligatorio."), StringLength(10), RegularExpression(@"^\d{8}-\d$", ErrorMessage = "El documento debe tener el formato 09678123-5.")]
    [Display(Name = "Documento de identidad")]
    public string? DocumentoIdentidad { get; set; }
    [Required(ErrorMessage = "El teléfono es obligatorio."), StringLength(9), RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "El teléfono debe tener 8 dígitos con el formato 7458-9865.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = "";
    [EmailAddress, StringLength(256), Display(Name = "Correo electrónico")]
    public string? Email { get; set; }
    [Display(Name = "Fecha de nacimiento"), DataType(DataType.Date)]
    public DateOnly? FechaNacimiento { get; set; }
    [RegularExpression("^(M|F|O)$"), Display(Name = "Género")]
    public string? Genero { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
            yield return new ValidationResult("La fecha de nacimiento no puede ser futura.", new[] { nameof(FechaNacimiento) });
    }
}
public class MedicoForm
{
    public int IdMedico { get; set; }
    [Required, StringLength(100), RegularExpression("^[A-Za-z\\u00C0-\\u024F\\u1E00-\\u1EFF '’-]+$", ErrorMessage = "No se pueden colocar números en el nombre.")]
    public string Nombre { get; set; } = "";
    [Required, StringLength(100), RegularExpression("^[A-Za-z\\u00C0-\\u024F\\u1E00-\\u1EFF '’-]+$", ErrorMessage = "No se pueden colocar números en el apellido.")]
    public string Apellido { get; set; } = "";
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una especialidad."), Display(Name = "Especialidad")]
    public int IdEspecialidad { get; set; }
    [StringLength(9), RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "El teléfono debe tener 8 dígitos con el formato 7458-9865."), Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
    [EmailAddress, StringLength(100), Display(Name = "Correo electrónico")]
    public string? Email { get; set; }
    [Required, RegularExpression("^(activo|inactivo)$")]
    public string Estado { get; set; } = "activo";
    public List<SelectListItem> Especialidades { get; set; } = [];
}
public class EspecialidadForm
{
    public int IdEspecialidad { get; set; }
    [Required, StringLength(100), RegularExpression("^[A-Za-z\\u00C0-\\u024F\\u1E00-\\u1EFF '’-]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = "";
    [StringLength(255), Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
}
public class CitaForm : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un paciente."), Display(Name = "Paciente")]
    public int IdPaciente { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un médico."), Display(Name = "Médico")]
    public int IdMedico { get; set; }
    [Required, Display(Name = "Fecha y hora")]
    public DateTime? FechaHora { get; set; }
    [Required, StringLength(255), Display(Name = "Motivo de consulta")]
    public string MotivoConsulta { get; set; } = "";
    public List<SelectListItem> Pacientes { get; set; } = [];
    public List<SelectListItem> Medicos { get; set; } = [];
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (FechaHora.HasValue && FechaHora.Value <= DateTime.Now)
            yield return new ValidationResult("Seleccione una fecha y hora futuras.", new[] { nameof(FechaHora) });
    }
}

public class HomePacienteViewModel
{
    public Paciente? Paciente { get; set; }
    public IReadOnlyList<Cita> ProximasCitas { get; set; } = [];
}