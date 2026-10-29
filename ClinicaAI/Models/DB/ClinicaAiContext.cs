using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ClinicaAI.Models.DB;

public partial class ClinicaAiContext : IdentityDbContext<IdentityUser>
{
    public ClinicaAiContext()
    {
    }

    public ClinicaAiContext(DbContextOptions<ClinicaAiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CanalesComunicacion> CanalesComunicacions { get; set; }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Conversacione> Conversaciones { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<Medico> Medicos { get; set; }

    public virtual DbSet<Mensaje> Mensajes { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<PreguntasFrecuentesFaq> PreguntasFrecuentesFaqs { get; set; }

    public virtual DbSet<TriajesChatbot> TriajesChatbots { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=bermudez; Database=ClinicaAI; TrustServerCertificate=True; Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CanalesComunicacion>(entity =>
        {
            entity.HasKey(e => e.IdCanal).HasName("PK__canales___D4C01CC8B9F955C1");

            entity.ToTable("canales_comunicacion");

            entity.Property(e => e.IdCanal).HasColumnName("id_canal");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.IdCita).HasName("PK__citas__6AEC3C0904418519");

            entity.ToTable("citas");

            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("programda")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaHora)
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.IdConversacion).HasColumnName("id_conversacion");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.MotivoConsulta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("motivo_consulta");
            entity.Property(e => e.OrigenAgendamiento)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("chatbot")
                .HasColumnName("origen_agendamiento");

            entity.HasOne(d => d.IdConversacionNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.IdConversacion)
                .HasConstraintName("FK_citas_conversacion");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citas_medico");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citas_paciente");
        });

        modelBuilder.Entity<Conversacione>(entity =>
        {
            entity.HasKey(e => e.IdConversacion).HasName("PK__conversa__DA6816872FF38CBF");

            entity.ToTable("conversaciones");

            entity.Property(e => e.IdConversacion).HasColumnName("id_conversacion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("bot")
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.IdCanal).HasColumnName("id_canal");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdentificadorUsuarioCanal)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("identificador_usuario_canal");

            entity.HasOne(d => d.IdCanalNavigation).WithMany(p => p.Conversaciones)
                .HasForeignKey(d => d.IdCanal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_conversaciones_canal");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Conversaciones)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("FK_conversaciones_paciente");
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.IdEspecialidad).HasName("PK__especial__C1D137639CD6F569");

            entity.ToTable("especialidades");

            entity.HasIndex(e => e.Nombre, "UQ__especial__72AFBCC68223EACC").IsUnique();

            entity.Property(e => e.IdEspecialidad).HasColumnName("id_especialidad");
            entity.Property(e => e.Descripcion)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.IdMedico).HasName("PK__medicos__E038EB430641E0BE");

            entity.ToTable("medicos");

            entity.HasIndex(e => e.Email, "UQ__medicos__AB6E6164A4DB9474").IsUnique();

            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("activo")
                .HasColumnName("estado");
            entity.Property(e => e.IdEspecialidad).HasColumnName("id_especialidad");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdEspecialidadNavigation).WithMany(p => p.Medicos)
                .HasForeignKey(d => d.IdEspecialidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_medicos_especialidad");
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.IdMensaje).HasName("PK__mensajes__5B37C7F6C9325B98");

            entity.ToTable("mensajes");

            entity.Property(e => e.IdMensaje).HasColumnName("id_mensaje");
            entity.Property(e => e.ContenidoTexto)
                .IsUnicode(false)
                .HasColumnName("contenido_texto");
            entity.Property(e => e.Emisor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EMISOR");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_envio");
            entity.Property(e => e.IdConversacion).HasColumnName("id_conversacion");
            entity.Property(e => e.TipoContenido)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("texto")
                .HasColumnName("tipo_contenido");

            entity.HasOne(d => d.IdConversacionNavigation).WithMany(p => p.Mensajes)
                .HasForeignKey(d => d.IdConversacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_mensajes_conversaciones");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.IdPaciente).HasName("PK__paciente__2C2C72BB57BF678F");

            entity.ToTable("pacientes");

            entity.HasIndex(e => e.DocumentoIdentidad, "UQ__paciente__1A03B13F9D5C0077").IsUnique();

            entity.HasIndex(e => e.Telefono, "UQ__paciente__2A16D9456563CD0B").IsUnique();

            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.DocumentoIdentidad)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("documento_identidad");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("genero");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<PreguntasFrecuentesFaq>(entity =>
        {
            entity.HasKey(e => e.IdFaq).HasName("PK__pregunta__D6560C1EE111E3E7");

            entity.ToTable("preguntas_frecuentes_faq");

            entity.Property(e => e.IdFaq).HasColumnName("id_faq");
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("categoria");
            entity.Property(e => e.PalabrasClave)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("palabras_clave");
            entity.Property(e => e.Pregunta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("pregunta");
            entity.Property(e => e.Respuesta)
                .IsUnicode(false)
                .HasColumnName("respuesta");
        });

        modelBuilder.Entity<TriajesChatbot>(entity =>
        {
            entity.HasKey(e => e.IdTriaje).HasName("PK__triajes___F2A884C80331E19E");

            entity.ToTable("triajes_chatbot");

            entity.Property(e => e.IdTriaje).HasColumnName("id_triaje");
            entity.Property(e => e.FechaTriaje)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_triaje");
            entity.Property(e => e.IdConversacion).HasColumnName("id_conversacion");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.NivelPrioridad)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Baja")
                .HasColumnName("nivel_prioridad");
            entity.Property(e => e.RecomendacionBot)
                .IsUnicode(false)
                .HasColumnName("recomendacion_bot");
            entity.Property(e => e.SintomasReportados)
                .IsUnicode(false)
                .HasColumnName("sintomas_reportados");

            entity.HasOne(d => d.IdConversacionNavigation).WithMany(p => p.TriajesChatbots)
                .HasForeignKey(d => d.IdConversacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_triajes_conversaciones");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.TriajesChatbots)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("FK_triajes_paciente");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
