using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Migraciones
{
    /// <summary>
    /// MIG-0005 (R5): agrega la tabla Exencion (modelo-datos-logico §2.4, CU-15, RN-07) — sujeto
    /// de confianza (rol, usuario o canal) excluido de la moderación, asociado a un servidor por
    /// su snowflake (RN-08, RC-01). Crea el índice ix_exencion_servidor para el descarte de
    /// exentos por servidor (RN-07) y el índice único ux_exencion_sujeto sobre
    /// (servidor, tipo, snowflake) para evitar duplicados (CU-15 EXENCION_DUPLICADA). Coherente
    /// con el esquema de R1-R4.
    /// </summary>
    /// <inheritdoc />
    public partial class MIG0005Exencion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exencion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnowflakeServidor = table.Column<string>(type: "TEXT", nullable: false),
                    TipoSujeto = table.Column<string>(type: "TEXT", nullable: false),
                    SnowflakeSujeto = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exencion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exencion_SnowflakeServidor",
                table: "Exencion",
                column: "SnowflakeServidor");

            migrationBuilder.CreateIndex(
                name: "IX_Exencion_SnowflakeServidor_TipoSujeto_SnowflakeSujeto",
                table: "Exencion",
                columns: new[] { "SnowflakeServidor", "TipoSujeto", "SnowflakeSujeto" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exencion");
        }
    }
}
