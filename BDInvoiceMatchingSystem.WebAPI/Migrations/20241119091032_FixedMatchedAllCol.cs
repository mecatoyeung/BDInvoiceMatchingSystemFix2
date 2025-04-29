using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixedMatchedAllCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllItemsAreMatched",
                table: "PriceRebates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllItemsAreMatched",
                table: "PriceRebates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
