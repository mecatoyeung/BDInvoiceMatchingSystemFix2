using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixFileUplaodMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentUploadRow",
                table: "PriceRebates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "PriceRebates",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalUploadRow",
                table: "PriceRebates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentUploadRow",
                table: "PriceRebates");

            migrationBuilder.DropColumn(
                name: "Filename",
                table: "PriceRebates");

            migrationBuilder.DropColumn(
                name: "TotalUploadRow",
                table: "PriceRebates");
        }
    }
}
