using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRebateItemIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_PriceRebateItems_PriceRebateID",
                table: "PriceRebateItems",
                newName: "IX_PriceRebateItem_PriceRebateID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_PriceRebateItem_PriceRebateID",
                table: "PriceRebateItems",
                newName: "IX_PriceRebateItems_PriceRebateID");
        }
    }
}
