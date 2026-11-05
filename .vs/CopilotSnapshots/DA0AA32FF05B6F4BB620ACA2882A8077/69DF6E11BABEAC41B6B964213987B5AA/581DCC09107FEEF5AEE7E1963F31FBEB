using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentityInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "canales_comunicacion",
                columns: table => new
                {
                    id_canal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__canales___D4C01CC8B9F955C1", x => x.id_canal);
                });

            migrationBuilder.CreateTable(
                name: "especialidades",
                columns: table => new
                {
                    id_especialidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__especial__C1D137639CD6F569", x => x.id_especialidad);
                });

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    id_paciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    documento_identidad = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    genero = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__paciente__2C2C72BB57BF678F", x => x.id_paciente);
                });

            migrationBuilder.CreateTable(
                name: "preguntas_frecuentes_faq",
                columns: table => new
                {
                    id_faq = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoria = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    pregunta = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    respuesta = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    palabras_clave = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__pregunta__D6560C1EE111E3E7", x => x.id_faq);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "medicos",
                columns: table => new
                {
                    id_medico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    id_especialidad = table.Column<int>(type: "int", nullable: false),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__medicos__E038EB430641E0BE", x => x.id_medico);
                    table.ForeignKey(
                        name: "FK_medicos_especialidad",
                        column: x => x.id_especialidad,
                        principalTable: "especialidades",
                        principalColumn: "id_especialidad");
                });

            migrationBuilder.CreateTable(
                name: "conversaciones",
                columns: table => new
                {
                    id_conversacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: true),
                    id_canal = table.Column<int>(type: "int", nullable: false),
                    identificador_usuario_canal = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "bot"),
                    fecha_inicio = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    fecha_fin = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__conversa__DA6816872FF38CBF", x => x.id_conversacion);
                    table.ForeignKey(
                        name: "FK_conversaciones_canal",
                        column: x => x.id_canal,
                        principalTable: "canales_comunicacion",
                        principalColumn: "id_canal");
                    table.ForeignKey(
                        name: "FK_conversaciones_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente");
                });

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    id_cita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_medico = table.Column<int>(type: "int", nullable: false),
                    id_conversacion = table.Column<int>(type: "int", nullable: true),
                    fecha_hora = table.Column<DateTime>(type: "datetime", nullable: false),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "programda"),
                    origen_agendamiento = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "chatbot"),
                    motivo_consulta = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__citas__6AEC3C0904418519", x => x.id_cita);
                    table.ForeignKey(
                        name: "FK_citas_conversacion",
                        column: x => x.id_conversacion,
                        principalTable: "conversaciones",
                        principalColumn: "id_conversacion");
                    table.ForeignKey(
                        name: "FK_citas_medico",
                        column: x => x.id_medico,
                        principalTable: "medicos",
                        principalColumn: "id_medico");
                    table.ForeignKey(
                        name: "FK_citas_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente");
                });

            migrationBuilder.CreateTable(
                name: "mensajes",
                columns: table => new
                {
                    id_mensaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_conversacion = table.Column<int>(type: "int", nullable: false),
                    EMISOR = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    contenido_texto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    tipo_contenido = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "texto"),
                    fecha_envio = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__mensajes__5B37C7F6C9325B98", x => x.id_mensaje);
                    table.ForeignKey(
                        name: "FK_mensajes_conversaciones",
                        column: x => x.id_conversacion,
                        principalTable: "conversaciones",
                        principalColumn: "id_conversacion");
                });

            migrationBuilder.CreateTable(
                name: "triajes_chatbot",
                columns: table => new
                {
                    id_triaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_conversacion = table.Column<int>(type: "int", nullable: false),
                    id_paciente = table.Column<int>(type: "int", nullable: true),
                    sintomas_reportados = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    nivel_prioridad = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "Baja"),
                    recomendacion_bot = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    fecha_triaje = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__triajes___F2A884C80331E19E", x => x.id_triaje);
                    table.ForeignKey(
                        name: "FK_triajes_conversaciones",
                        column: x => x.id_conversacion,
                        principalTable: "conversaciones",
                        principalColumn: "id_conversacion");
                    table.ForeignKey(
                        name: "FK_triajes_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_conversacion",
                table: "citas",
                column: "id_conversacion");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_medico",
                table: "citas",
                column: "id_medico");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_paciente",
                table: "citas",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_conversaciones_id_canal",
                table: "conversaciones",
                column: "id_canal");

            migrationBuilder.CreateIndex(
                name: "IX_conversaciones_id_paciente",
                table: "conversaciones",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "UQ__especial__72AFBCC68223EACC",
                table: "especialidades",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_medicos_id_especialidad",
                table: "medicos",
                column: "id_especialidad");

            migrationBuilder.CreateIndex(
                name: "UQ__medicos__AB6E6164A4DB9474",
                table: "medicos",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_mensajes_id_conversacion",
                table: "mensajes",
                column: "id_conversacion");

            migrationBuilder.CreateIndex(
                name: "UQ__paciente__1A03B13F9D5C0077",
                table: "pacientes",
                column: "documento_identidad",
                unique: true,
                filter: "[documento_identidad] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__paciente__2A16D9456563CD0B",
                table: "pacientes",
                column: "telefono",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_triajes_chatbot_id_conversacion",
                table: "triajes_chatbot",
                column: "id_conversacion");

            migrationBuilder.CreateIndex(
                name: "IX_triajes_chatbot_id_paciente",
                table: "triajes_chatbot",
                column: "id_paciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "mensajes");

            migrationBuilder.DropTable(
                name: "preguntas_frecuentes_faq");

            migrationBuilder.DropTable(
                name: "triajes_chatbot");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "medicos");

            migrationBuilder.DropTable(
                name: "conversaciones");

            migrationBuilder.DropTable(
                name: "especialidades");

            migrationBuilder.DropTable(
                name: "canales_comunicacion");

            migrationBuilder.DropTable(
                name: "pacientes");
        }
    }
}
