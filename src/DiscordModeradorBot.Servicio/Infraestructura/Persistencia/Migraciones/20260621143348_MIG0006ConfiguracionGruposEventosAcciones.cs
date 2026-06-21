using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class MIG0006ConfiguracionGruposEventosAcciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnowflakeServidor = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Prioridad = table.Column<int>(type: "INTEGER", nullable: false),
                    Continuar = table.Column<bool>(type: "INTEGER", nullable: false),
                    Modo = table.Column<string>(type: "TEXT", nullable: false),
                    ModoCombinacionGrupos = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrupoDeReglas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnowflakeServidor = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    ModoCoincidencia = table.Column<string>(type: "TEXT", nullable: false),
                    MinimoCoincidencias = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoDeReglas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    OrdenEjecucion = table.Column<int>(type: "INTEGER", nullable: false),
                    VentanaBorradoDias = table.Column<int>(type: "INTEGER", nullable: true),
                    DuracionTimeoutMinutos = table.Column<int>(type: "INTEGER", nullable: true),
                    RolObjetivo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accion_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoGrupo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventoId = table.Column<int>(type: "INTEGER", nullable: false),
                    GrupoDeReglasId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoGrupo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoGrupo_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoRegla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GrupoDeReglasId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClaseRegla = table.Column<string>(type: "TEXT", nullable: false),
                    ReglaContenidoId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClaveReglaConducta = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoRegla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrupoRegla_GrupoDeReglas_GrupoDeReglasId",
                        column: x => x.GrupoDeReglasId,
                        principalTable: "GrupoDeReglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accion_EventoId_OrdenEjecucion",
                table: "Accion",
                columns: new[] { "EventoId", "OrdenEjecucion" });

            migrationBuilder.CreateIndex(
                name: "IX_Evento_SnowflakeServidor_Prioridad",
                table: "Evento",
                columns: new[] { "SnowflakeServidor", "Prioridad" });

            migrationBuilder.CreateIndex(
                name: "IX_EventoGrupo_EventoId",
                table: "EventoGrupo",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoDeReglas_SnowflakeServidor",
                table: "GrupoDeReglas",
                column: "SnowflakeServidor");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoRegla_GrupoDeReglasId",
                table: "GrupoRegla",
                column: "GrupoDeReglasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accion");

            migrationBuilder.DropTable(
                name: "EventoGrupo");

            migrationBuilder.DropTable(
                name: "GrupoRegla");

            migrationBuilder.DropTable(
                name: "Evento");

            migrationBuilder.DropTable(
                name: "GrupoDeReglas");
        }
    }
}
