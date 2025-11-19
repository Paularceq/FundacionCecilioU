using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOutgoingDonation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutgoingDonationId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "InventoryMovements",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "OutgoingDonations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    RecipientId = table.Column<int>(type: "int", nullable: false),
                    ApproverId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Ammount = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingDonations_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingDonations_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingDonations_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_OutgoingDonationId",
                table: "InventoryMovements",
                column: "OutgoingDonationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDonations_ApproverId",
                table: "OutgoingDonations",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDonations_RecipientId",
                table: "OutgoingDonations",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDonations_RequesterId",
                table: "OutgoingDonations",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_OutgoingDonations_OutgoingDonationId",
                table: "InventoryMovements",
                column: "OutgoingDonationId",
                principalTable: "OutgoingDonations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_OutgoingDonations_OutgoingDonationId",
                table: "InventoryMovements");

            migrationBuilder.DropTable(
                name: "OutgoingDonations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_OutgoingDonationId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "OutgoingDonationId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InventoryMovements");
        }
    }
}
