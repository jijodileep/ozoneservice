using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzoneMobileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeMobileMastersGlobal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mobile_variants_TenantId",
                table: "mobile_variants");

            migrationBuilder.DropIndex(
                name: "IX_mobile_models_TenantId",
                table: "mobile_models");

            migrationBuilder.DropIndex(
                name: "IX_mobile_brands_TenantId",
                table: "mobile_brands");

            migrationBuilder.DropIndex(
                name: "IX_mobile_brands_TenantId_Name",
                table: "mobile_brands");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "mobile_variants");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "mobile_variants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mobile_variants");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "mobile_variants");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "mobile_variants");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "mobile_models");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "mobile_models");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mobile_models");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "mobile_models");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "mobile_models");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "mobile_brands");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "mobile_brands");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mobile_brands");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "mobile_brands");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "mobile_brands");

            migrationBuilder.CreateIndex(
                name: "IX_mobile_brands_Name",
                table: "mobile_brands",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mobile_brands_Name",
                table: "mobile_brands");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "mobile_variants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "mobile_variants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mobile_variants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "mobile_variants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "mobile_variants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "mobile_models",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "mobile_models",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mobile_models",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "mobile_models",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "mobile_models",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "mobile_brands",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "mobile_brands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mobile_brands",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "mobile_brands",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "mobile_brands",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mobile_variants_TenantId",
                table: "mobile_variants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mobile_models_TenantId",
                table: "mobile_models",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mobile_brands_TenantId",
                table: "mobile_brands",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mobile_brands_TenantId_Name",
                table: "mobile_brands",
                columns: new[] { "TenantId", "Name" },
                unique: true);
        }
    }
}
