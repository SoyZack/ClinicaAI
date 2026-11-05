using ClinicaAI.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAI.Data;

public static class ClinicaAIInitializer
{
    private static readonly (string Nombre, string Descripcion)[] EspecialidadesBase =
    [
        ("Medicina General", "Atención médica integral y consulta primaria."),
        ("Pediatría", "Atención de la salud de niños y adolescentes."),
        ("Cardiología", "Diagnóstico y tratamiento de enfermedades del corazón."),
        ("Psiquiatría", "Evaluación y tratamiento de la salud mental."),
        ("Medicina Interna", "Diagnóstico y tratamiento de enfermedades del adulto."),
        ("Traumatología", "Lesiones de huesos, músculos y articulaciones."),
        ("Ginecología y Obstetricia", "Salud femenina, embarazo y parto."),
        ("Dermatología", "Enfermedades de la piel, cabello y uñas.")
    ];

    public static async Task SeedEspecialidadesAsync(ClinicaAiContext db, ILogger logger)
    {
        if (await db.Especialidades.AnyAsync())
        {
            return;
        }

        try
        {
            db.Especialidades.AddRange(EspecialidadesBase.Select(e => new Especialidade
            {
                Nombre = e.Nombre,
                Descripcion = e.Descripcion
            }));

            await db.SaveChangesAsync();
            logger.LogInformation("Se cargaron {Cantidad} especialidades base.", EspecialidadesBase.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudieron cargar las especialidades base.");
        }
    }
}
