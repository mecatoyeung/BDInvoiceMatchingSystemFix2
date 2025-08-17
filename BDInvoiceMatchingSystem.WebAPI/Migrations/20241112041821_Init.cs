using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace BDInvoiceMatchingSystem.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CustomerCode = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CustomerName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CustomerAddress = table.Column<string>(type: "NVARCHAR(1023)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentsFromCashew",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DocumentClass = table.Column<int>(type: "int", nullable: false),
                    DocumentCreatedFrom = table.Column<int>(type: "int", nullable: false),
                    PDFFilename = table.Column<string>(type: "longtext", nullable: false),
                    CSVFilename = table.Column<string>(type: "varchar(255)", nullable: false),
                    DocumentNo = table.Column<string>(type: "varchar(255)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CustomerCode = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    CustomerName = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    CustomerAddress = table.Column<string>(type: "VARCHAR(1023)", nullable: true),
                    PDFFile = table.Column<byte[]>(type: "longblob", nullable: true),
                    CSVFile = table.Column<byte[]>(type: "longblob", nullable: true),
                    UploadedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsFromCashew", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FileSources",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FolderPath = table.Column<string>(type: "NVARCHAR(1023)", nullable: false),
                    DocumentClass = table.Column<int>(type: "int", nullable: false),
                    DocumentNoColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    DocumentDateColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    DeliveryDateColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CustomerCodeColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CustomerNameColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CustomerAddressColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    StockCodeColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    DescriptionColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    LotNoColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    QuantityColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    UnitOfMeasureColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    UnitPriceColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    SubtotalColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    FilenameColName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    FirstRowIsHeader = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IntervalInSeconds = table.Column<int>(type: "int", nullable: false),
                    NextCaptureDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileSources", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Matchings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchings", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PriceRebates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ExcelFilename = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    ExcelFile = table.Column<byte[]>(type: "longblob", nullable: true),
                    AllItemsAreMatched = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UploadedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRebates", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PriceRebateSetting",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DocumentNoHeaderName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    StockCodeHeaderName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    DescriptionHeaderName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    QuantityHeaderName = table.Column<string>(type: "NVARCHAR(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRebateSetting", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomerApproximateMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    ApproximateValue = table.Column<string>(type: "NVARCHAR(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerApproximateMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerApproximateMappings_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentFromCashewItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DocumentFromCashewID = table.Column<long>(type: "bigint", nullable: false),
                    StockCode = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    LotNo = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Matched = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MatchingID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFromCashewItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DocumentFromCashewItems_DocumentsFromCashew_DocumentFromCash~",
                        column: x => x.DocumentFromCashewID,
                        principalTable: "DocumentsFromCashew",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentFromCashewItems_Matchings_MatchingID",
                        column: x => x.MatchingID,
                        principalTable: "Matchings",
                        principalColumn: "ID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PriceRebateItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PriceRebateID = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNo = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    StockCode = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Matched = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MatchingID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRebateItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PriceRebateItems_Matchings_MatchingID",
                        column: x => x.MatchingID,
                        principalTable: "Matchings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PriceRebateItems_PriceRebates_PriceRebateID",
                        column: x => x.PriceRebateID,
                        principalTable: "PriceRebates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerApproximateMappings_CustomerID",
                table: "CustomerApproximateMappings",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CustomerName",
                table: "Customers",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashewItem_LotNo",
                table: "DocumentFromCashewItems",
                column: "LotNo");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashewItem_StockCode",
                table: "DocumentFromCashewItems",
                column: "StockCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashewItems_DocumentFromCashewID",
                table: "DocumentFromCashewItems",
                column: "DocumentFromCashewID");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashewItems_MatchingID",
                table: "DocumentFromCashewItems",
                column: "MatchingID");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_CSVFilename",
                table: "DocumentsFromCashew",
                column: "CSVFilename");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_CustomerCode",
                table: "DocumentsFromCashew",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_CustomerName",
                table: "DocumentsFromCashew",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_DeliveryDate",
                table: "DocumentsFromCashew",
                column: "DeliveryDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_DocumentClass",
                table: "DocumentsFromCashew",
                column: "DocumentClass");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_DocumentCreatedFrom",
                table: "DocumentsFromCashew",
                column: "DocumentCreatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_DocumentDate",
                table: "DocumentsFromCashew",
                column: "DocumentDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_DocumentNo",
                table: "DocumentsFromCashew",
                column: "DocumentNo");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFromCashew_UploadedDateTime",
                table: "DocumentsFromCashew",
                column: "UploadedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRebateItems_MatchingID",
                table: "PriceRebateItems",
                column: "MatchingID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRebateItems_PriceRebateID",
                table: "PriceRebateItems",
                column: "PriceRebateID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRebate_ExcelFilename",
                table: "PriceRebates",
                column: "ExcelFilename");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRebate_UploadedDateTime",
                table: "PriceRebates",
                column: "UploadedDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CustomerApproximateMappings");

            migrationBuilder.DropTable(
                name: "DocumentFromCashewItems");

            migrationBuilder.DropTable(
                name: "FileSources");

            migrationBuilder.DropTable(
                name: "PriceRebateItems");

            migrationBuilder.DropTable(
                name: "PriceRebateSetting");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "DocumentsFromCashew");

            migrationBuilder.DropTable(
                name: "Matchings");

            migrationBuilder.DropTable(
                name: "PriceRebates");
        }
    }
}
