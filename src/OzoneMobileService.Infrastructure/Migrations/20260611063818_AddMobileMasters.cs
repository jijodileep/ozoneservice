using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzoneMobileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMobileMasters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mobile_brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mobile_brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mobile_models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mobile_models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mobile_models_mobile_brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "mobile_brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mobile_variants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mobile_variants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mobile_variants_mobile_models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "mobile_models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mobile_brands_TenantId",
                table: "mobile_brands",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mobile_brands_TenantId_Name",
                table: "mobile_brands",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mobile_models_BrandId_Name",
                table: "mobile_models",
                columns: new[] { "BrandId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mobile_models_TenantId",
                table: "mobile_models",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mobile_variants_ModelId_Name",
                table: "mobile_variants",
                columns: new[] { "ModelId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mobile_variants_TenantId",
                table: "mobile_variants",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mobile_variants");

            migrationBuilder.DropTable(
                name: "mobile_models");

            migrationBuilder.DropTable(
                name: "mobile_brands");
        }
    }
}
