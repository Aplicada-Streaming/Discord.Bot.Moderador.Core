using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Migraciones
{
    /// <summary>
    /// MIG-0003 (R3): agrega la tabla Regla para las reglas de contenido (modelo-datos-logico §2.5,
    /// CU-04). Guarda el criterio (expresión regular validada, RN-03), su clase, la sensibilidad a
    /// mayúsculas y la asociación al servidor (snowflake como TEXTO, RN-08) y a la política por su
    /// nombre lógico. Coherente con el esquema de R1/R2.
    /// </summary>
    /// <inheritdoc />
    public partial class MIG0003ReglaContenido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Regla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnowflakeServidor = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    TipoCriterio = table.Column<string>(type: "TEXT", nullable: false),
                    Criterio = table.Column<string>(type: "TEXT", nullable: false),
                    SensibleAMayusculas = table.Column<bool>(type: "INTEGER", nullable: false),
                    NombrePolitica = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regla", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regla_SnowflakeServidor",
                table: "Regla",
                column: "SnowflakeServidor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Regla");
        }
    }
}
