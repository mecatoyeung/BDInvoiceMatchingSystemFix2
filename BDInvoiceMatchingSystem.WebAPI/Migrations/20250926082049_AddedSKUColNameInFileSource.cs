using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedSKUColNameInFileSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "PriceRebateItems",
                type: "VARCHAR(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(255)");

            migrationBuilder.AddColumn<string>(
                name: "SKUColName",
                table: "FileSources",
                type: "NVARCHAR(255)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SKUColName",
                table: "FileSources");

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "PriceRebateItems",
                type: "VARCHAR(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(255)",
                oldNullable: true);
        }
    }
}
