using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class MIG0007ParametrosServidor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParametrosServidor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnowflakeServidor = table.Column<string>(type: "TEXT", nullable: false),
                    UmbralCanalesDistintos = table.Column<int>(type: "INTEGER", nullable: true),
                    VentanaDeteccionSegundos = table.Column<double>(type: "REAL", nullable: true),
                    VentanaAntirreboteSegundos = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosServidor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosServidor_SnowflakeServidor",
                table: "ParametrosServidor",
                column: "SnowflakeServidor",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParametrosServidor");
        }
    }
}
