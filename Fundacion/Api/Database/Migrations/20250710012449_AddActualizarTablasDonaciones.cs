using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddActualizarTablasDonaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationProducts");

            migrationBuilder.AddColumn<int>(
                name: "ProductsDonationId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_ProductsDonationId",
                table: "InventoryMovements",
                column: "ProductsDonationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityDonations_Donations_Id",
                table: "ActivityDonations",
                column: "Id",
                principalTable: "Donations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_ProductsDonations_ProductsDonationId",
                table: "InventoryMovements",
                column: "ProductsDonationId",
                principalTable: "ProductsDonations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_Products_ProductId",
                table: "InventoryMovements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonetaryDonations_Donations_Id",
                table: "MonetaryDonations",
                column: "Id",
                principalTable: "Donations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsDonations_Donations_Id",
                table: "ProductsDonations",
                column: "Id",
                principalTable: "Donations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityDonations_Donations_Id",
                table: "ActivityDonations");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_ProductsDonations_ProductsDonationId",
                table: "InventoryMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_Products_ProductId",
                table: "InventoryMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_MonetaryDonations_Donations_Id",
                table: "MonetaryDonations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductsDonations_Donations_Id",
                table: "ProductsDonations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_ProductsDonationId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "ProductsDonationId",
                table: "InventoryMovements");

            migrationBuilder.CreateTable(
                name: "DonationProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductsDonationId = table.Column<int>(type: "int", nullable: true),
                    Unit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationProducts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonationProducts_ProductsDonationId",
                table: "DonationProducts",
                column: "ProductsDonationId");
        }
    }
}
