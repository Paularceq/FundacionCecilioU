using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVolunteerHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "VolunteerHours",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "ApproverId",
                table: "VolunteerHours",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ActivitiesDescription",
                table: "VolunteerHours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "VolunteerHours",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VolunteerHours",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "VolunteerHours",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VolunteerHours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "VolunteerHours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredBy",
                table: "VolunteerHours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "VolunteerHours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "VolunteerHours",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "VolunteerHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalHours",
                table: "VolunteerHours",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "ActivitiesDescription",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "RegisteredBy",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "State",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "TotalHours",
                table: "VolunteerHours");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "VolunteerHours",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "ApproverId",
                table: "VolunteerHours",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Users_ApproverId",
                table: "VolunteerHours",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
