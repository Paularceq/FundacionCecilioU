using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReasonAndApprovedVolunteerRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_VolunteerRequestId",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "RegisteredBy",
                table: "VolunteerHours");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "VolunteerRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "VolunteerRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "VolunteerHours",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "VolunteerHours",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_State",
                table: "VolunteerHours",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "UX_VolunteerHours_OnePerDay",
                table: "VolunteerHours",
                columns: new[] { "VolunteerRequestId", "Date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_State",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "UX_VolunteerHours_OnePerDay",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "VolunteerRequests");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "VolunteerRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "VolunteerHours",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "VolunteerHours",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "VolunteerHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VolunteerHours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredBy",
                table: "VolunteerHours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_VolunteerRequestId",
                table: "VolunteerHours",
                column: "VolunteerRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
