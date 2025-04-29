using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedPriceRebateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubtotalInUSDHeaderName",
                table: "PriceRebateSetting",
                type: "NVARCHAR(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnitPriceHeaderName",
                table: "PriceRebateSetting",
                type: "NVARCHAR(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalInHKD",
                table: "PriceRebateItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalInUSD",
                table: "PriceRebateItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "PriceRebateItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubtotalInUSDHeaderName",
                table: "PriceRebateSetting");

            migrationBuilder.DropColumn(
                name: "UnitPriceHeaderName",
                table: "PriceRebateSetting");

            migrationBuilder.DropColumn(
                name: "SubtotalInHKD",
                table: "PriceRebateItems");

            migrationBuilder.DropColumn(
                name: "SubtotalInUSD",
                table: "PriceRebateItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "PriceRebateItems");
        }
    }
}
