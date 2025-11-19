using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationsTablesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductsDonationId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentificacionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityDonations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    DonationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityDonations_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonetaryDonations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonationId = table.Column<int>(type: "int", nullable: false),
                    Ammount = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonetaryDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonetaryDonations_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsDonations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsDonations_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_ProductsDonationId",
                table: "InventoryMovements",
                column: "ProductsDonationId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDonations_DonationId",
                table: "ActivityDonations",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_MonetaryDonations_DonationId",
                table: "MonetaryDonations",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsDonations_DonationId",
                table: "ProductsDonations",
                column: "DonationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_ProductsDonations_ProductsDonationId",
                table: "InventoryMovements",
                column: "ProductsDonationId",
                principalTable: "ProductsDonations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_ProductsDonations_ProductsDonationId",
                table: "InventoryMovements");

            migrationBuilder.DropTable(
                name: "ActivityDonations");

            migrationBuilder.DropTable(
                name: "MonetaryDonations");

            migrationBuilder.DropTable(
                name: "ProductsDonations");

            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_ProductsDonationId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "ProductsDonationId",
                table: "InventoryMovements");
        }
    }
}
