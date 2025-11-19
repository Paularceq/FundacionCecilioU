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
                { "AdminSistema",      "Administrador del sistema: gestiona la configuración técnica y el mantenimiento general de la plataforma." },
                { "AdminBecas",        "Administrador de becas: administra los procesos de aplicación, evaluación y asignación de becas." },
                { "AdminUsuarios",     "Administrador de usuarios: gestiona las cuentas, roles y permisos de los distintos usuarios del sistema." },
                { "AdminFinanzas",     "Administrador de finanzas: supervisa y administra las transacciones y registros financieros." },
                { "AdminDonaciones",   "Administrador de donaciones: gestiona las donaciones recibidas y su distribución en la plataforma." },
                { "AdminInventario",   "Administrador de inventario: supervisa y controla el ingreso, retiro y stock de productos." },
                { "AdminPublicitario", "Administrador publicitario: gestiona el contenido visual y promocional de la organización." },
                { "Estudiante",        "Estudiante: usuario que aplica o participa en programas de formación o beneficios." },
                { "Voluntario",        "Voluntario: usuario que colabora activamente en las actividades de la fundación." },
                { "Donador",           "Donador: usuario que realiza y da seguimiento a sus donaciones." }
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
