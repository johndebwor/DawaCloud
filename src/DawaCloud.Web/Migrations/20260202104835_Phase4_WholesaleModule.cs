using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DawaCloud.Web.Migrations
{
    /// <inheritdoc />
    public partial class Phase4_WholesaleModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "City",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultTaxRate",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "ExpiryAlertDays30",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "ExpiryAlertDays60",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "PharmacyLicenseNumber",
                table: "CompanySettings");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "CompanySettings",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "TaxId",
                table: "CompanySettings",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "CompanySettings",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LowStockThresholdDays",
                table: "CompanySettings",
                newName: "RoundingValue");

            migrationBuilder.RenameColumn(
                name: "ExpiryAlertDays90",
                table: "CompanySettings",
                newName: "RoundingMode");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyRoundingToInvoices",
                table: "CompanySettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyRoundingToRetail",
                table: "CompanySettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyRoundingToWholesale",
                table: "CompanySettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CompanySettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CompanySettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CompanySettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoundingEnabled",
                table: "CompanySettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CompanySettings",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyRoundingToInvoices",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "ApplyRoundingToRetail",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "ApplyRoundingToWholesale",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "IsRoundingEnabled",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CompanySettings");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "CompanySettings",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "RoundingValue",
                table: "CompanySettings",
                newName: "LowStockThresholdDays");

            migrationBuilder.RenameColumn(
                name: "RoundingMode",
                table: "CompanySettings",
                newName: "ExpiryAlertDays90");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "CompanySettings",
                newName: "TaxId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "CompanySettings",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultTaxRate",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpiryAlertDays30",
                table: "CompanySettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpiryAlertDays60",
                table: "CompanySettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyLicenseNumber",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
