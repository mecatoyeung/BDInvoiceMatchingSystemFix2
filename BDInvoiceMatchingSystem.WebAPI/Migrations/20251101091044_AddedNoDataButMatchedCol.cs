using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedNoDataButMatchedCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NoDataButMatched",
                table: "PriceRebateItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoDataButMatched",
                table: "PriceRebateItems");
        }
    }
}
