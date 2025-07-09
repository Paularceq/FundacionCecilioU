using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdminMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert SuperAdmin user if not exists
            migrationBuilder.Sql(@"
                    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Email] = 'super@admin.com')
                    BEGIN
                        INSERT INTO [Users] (
                            [Nombre], [Apellidos], [Email], [Nacionalidad], [Identificacion], [PasswordHash], [RequiereCambioDePassword]
                        ) VALUES (
                            'Super', 'Admin', 'super@admin.com', 'N/A', '000000000', 
                            -- Password: 'superAdmin123!' hashed with BCrypt
                            '$2a$11$UNHFtiOBSPAMGkiO8Wh.uujDP4sQUIIOT5gsGxs9OhfT/gJ4qhSWK', 
                            1
                        )
                    END
                ");

            // Assign all existing roles to SuperAdmin user
            migrationBuilder.Sql(@"
                    DECLARE @SuperAdminId INT = (SELECT TOP 1 [Id] FROM [Users] WHERE [Email] = 'super@admin.com');
                    IF @SuperAdminId IS NOT NULL
                    BEGIN
                        INSERT INTO [RoleUser] ([RolesId], [UsersId])
                        SELECT [Id], @SuperAdminId FROM [Roles]
                        WHERE NOT EXISTS (
                            SELECT 1 FROM [RoleUser] 
                            WHERE [RolesId] = [Roles].[Id] AND [UsersId] = @SuperAdminId
                        )
                    END
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove SuperAdmin user-role assignments
            migrationBuilder.Sql(@"
                    DECLARE @SuperAdminId INT = (SELECT TOP 1 [Id] FROM [Users] WHERE [Email] = 'super@admin.com');
                    IF @SuperAdminId IS NOT NULL
                    BEGIN
                        DELETE FROM [RoleUser] WHERE [UsersId] = @SuperAdminId
                    END
                ");

            // Remove SuperAdmin user
            migrationBuilder.Sql(@"
                    DELETE FROM [Users] WHERE [Email] = 'super@admin.com'
                ");
        }
    }
}
