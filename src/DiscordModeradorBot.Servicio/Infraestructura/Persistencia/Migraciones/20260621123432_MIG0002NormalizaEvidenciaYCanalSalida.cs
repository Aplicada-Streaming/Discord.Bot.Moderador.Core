using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class MIG0002NormalizaEvidenciaYCanalSalida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanalesAfectados",
                table: "Incidente");

            migrationBuilder.DropColumn(
                name: "CopiaMensajes",
                table: "Incidente");

            migrationBuilder.AddColumn<string>(
                name: "PropositoCanalSalida",
                table: "Servidor",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SnowflakeCanalSalida",
                table: "Servidor",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CanalAfectado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IncidenteId = table.Column<int>(type: "INTEGER", nullable: false),
                    SnowflakeCanal = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanalAfectado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanalAfectado_Incidente_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensajeAccionado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IncidenteId = table.Column<int>(type: "INTEGER", nullable: false),
                    SnowflakeMensaje = table.Column<string>(type: "TEXT", nullable: false),
                    SnowflakeCanal = table.Column<string>(type: "TEXT", nullable: false),
                    ContenidoCopiado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeAccionado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajeAccionado_Incidente_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanalAfectado_IncidenteId",
                table: "CanalAfectado",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajeAccionado_IncidenteId",
                table: "MensajeAccionado",
                column: "IncidenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanalAfectado");

            migrationBuilder.DropTable(
                name: "MensajeAccionado");

            migrationBuilder.DropColumn(
                name: "PropositoCanalSalida",
                table: "Servidor");

            migrationBuilder.DropColumn(
                name: "SnowflakeCanalSalida",
                table: "Servidor");

            migrationBuilder.AddColumn<string>(
                name: "CanalesAfectados",
                table: "Incidente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CopiaMensajes",
                table: "Incidente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
