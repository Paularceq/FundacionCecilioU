using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaSolicitudBeca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesBeca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CedulaEstudiante = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreEstudiante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorreoContacto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TelefonoContacto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Colegio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NivelEducativo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CartaConsentimientoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CartaNotasUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ComentarioAdministrador = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EsFormularioManual = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesBeca", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesBeca");
        }
    }
}
