using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzoneMobileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpgradeRequestsAndTaxMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CgstAmount",
                table: "invoices",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceType",
                table: "invoices",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SgstAmount",
                table: "invoices",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "subscription_upgrade_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_upgrade_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_upgrade_requests_invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_subscription_upgrade_requests_subscription_plans_CurrentPla~",
                        column: x => x.CurrentPlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscription_upgrade_requests_subscription_plans_RequestedP~",
                        column: x => x.RequestedPlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscription_upgrade_requests_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tax_configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CgstRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    SgstRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_configurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_upgrade_requests_CurrentPlanId",
                table: "subscription_upgrade_requests",
                column: "CurrentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_upgrade_requests_InvoiceId",
                table: "subscription_upgrade_requests",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_upgrade_requests_RequestedPlanId",
                table: "subscription_upgrade_requests",
                column: "RequestedPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_upgrade_requests_TenantId",
                table: "subscription_upgrade_requests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_upgrade_requests_TenantId_Status",
                table: "subscription_upgrade_requests",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_upgrade_requests");

            migrationBuilder.DropTable(
                name: "tax_configurations");

            migrationBuilder.DropColumn(
                name: "CgstAmount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceType",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "SgstAmount",
                table: "invoices");
        }
    }
}
