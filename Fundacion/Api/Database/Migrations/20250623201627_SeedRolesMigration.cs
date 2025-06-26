using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Roles",
            columns: new[] { "Name", "Description" },
            values: new object[,]
            {
                { "AdminSistema", "Administrador del sistema: gestiona la configuración y mantenimiento general." },
                { "AdminBecas", "Administrador de becas: gestiona procesos y asignaciones de becas." },
                { "AdminUsuarios", "Administrador de usuarios: administra cuentas y permisos de usuarios." },
                { "AdminFinanzas", "Administrador de finanzas: supervisa y gestiona operaciones financieras." },
                { "AdminDonaciones", "Administrador de Donacioes: supervisa y gestiona operaciones de donaciones." },
                { "AdminInventario", "Inventario: usuario que supervisa y gestiona los inventarios" },
                { "AdminPublicitario", "Publicitario: usuario que gestiona toda la publicidad" },
                { "Estudiante", "Estudiante: usuario que aplica o solicita servicios o beneficios." }, 
                { "Voluntario", "Voluntario: usuario que visualiza su trabajo en la fundacion." },
                { "Donador", "Donador: usuario que supervisa y gestiona sus donaciones" }
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
            table: "Roles",
            keyColumn: "Name",
            keyValues: new object[]
            {
                "AdminSistema",
                "AdminBecas",
                "AdminUsuarios",
                "AdminFinanzas",
                "AdminDonaciones",
                "AdminInventario",
                "AdminPublicitario",
                "Estudiante",   
                "Voluntario",
                "Donador"

            });
        }
    }
}
