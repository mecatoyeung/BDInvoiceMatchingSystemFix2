using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedUploadErrorColToPriceRebate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadError",
                table: "PriceRebates",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadError",
                table: "PriceRebates");
        }
    }
}
