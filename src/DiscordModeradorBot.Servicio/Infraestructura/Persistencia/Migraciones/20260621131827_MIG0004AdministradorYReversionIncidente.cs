using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Migraciones
{
    /// <summary>
    /// MIG-0004 (R4): agrega la tabla Administrador para el administrador único (modelo-datos-logico
    /// §2.1, RC-06, RN-13) — identificador de cuenta único (índice ux_administrador_cuenta) y
    /// resguardo de contraseña como hash PHC, nunca en claro (ADR-03) — y los campos de reversión
    /// del baneo en Incidente (modelo-datos-logico §2.11, CU-07): ReversionAutorId (FK lógica a
    /// Administrador) y ReversionFecha (ticks UTC, nullables; solo se completan al desbanear). El
    /// desbaneo NO restaura mensajes (RN-11). Coherente con el esquema de R1/R2/R3.
    /// </summary>
    /// <inheritdoc />
    public partial class MIG0004AdministradorYReversionIncidente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReversionAutorId",
                table: "Incidente",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReversionFecha",
                table: "Incidente",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdentificadorCuenta = table.Column<string>(type: "TEXT", nullable: false),
                    ResguardoPassword = table.Column<string>(type: "TEXT", nullable: false),
                    CreadoEn = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrador", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrador_IdentificadorCuenta",
                table: "Administrador",
                column: "IdentificadorCuenta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropColumn(
                name: "ReversionAutorId",
                table: "Incidente");

            migrationBuilder.DropColumn(
                name: "ReversionFecha",
                table: "Incidente");
        }
    }
}
