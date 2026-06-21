using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class MIG0001EsquemaInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incidente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServidorId = table.Column<string>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    NombrePolitica = table.Column<string>(type: "TEXT", nullable: false),
                    Modo = table.Column<string>(type: "TEXT", nullable: false),
                    Accion = table.Column<string>(type: "TEXT", nullable: false),
                    Resultado = table.Column<string>(type: "TEXT", nullable: false),
                    CanalesAfectados = table.Column<string>(type: "TEXT", nullable: false),
                    CopiaMensajes = table.Column<string>(type: "TEXT", nullable: false),
                    Instante = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servidor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnowflakeServidor = table.Column<string>(type: "TEXT", nullable: false),
                    TokenCifrado = table.Column<string>(type: "TEXT", nullable: false),
                    EstadoConexion = table.Column<string>(type: "TEXT", nullable: false),
                    EstadoActivacion = table.Column<string>(type: "TEXT", nullable: false),
                    NombreDescriptivo = table.Column<string>(type: "TEXT", nullable: true),
                    CreadoEn = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servidor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidente_ServidorId_Instante",
                table: "Incidente",
                columns: new[] { "ServidorId", "Instante" });

            migrationBuilder.CreateIndex(
                name: "IX_Servidor_SnowflakeServidor",
                table: "Servidor",
                column: "SnowflakeServidor",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incidente");

            migrationBuilder.DropTable(
                name: "Servidor");
        }
    }
}
