using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedSKUColInPriceRebateItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentFromCashewItemID",
                table: "Matchings");

            migrationBuilder.DropColumn(
                name: "PriceRebateItemID",
                table: "Matchings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DocumentFromCashewItemID",
                table: "Matchings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PriceRebateItemID",
                table: "Matchings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
