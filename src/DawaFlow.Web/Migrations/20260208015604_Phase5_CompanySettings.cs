using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DawaFlow.Web.Migrations
{
    /// <inheritdoc />
    public partial class Phase5_CompanySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "DefaultCurrency",
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

            migrationBuilder.AddColumn<int>(
                name: "ExpiryAlertDays90",
                table: "CompanySettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FromEmail",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromName",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LowStockThresholdDays",
                table: "CompanySettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyLicenseNumber",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpHost",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpPassword",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmtpPort",
                table: "CompanySettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SmtpUsername",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwilioAccountSid",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwilioAuthToken",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwilioFromNumber",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "DefaultCurrency",
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
                name: "ExpiryAlertDays90",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "FromEmail",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "FromName",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "LowStockThresholdDays",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "PharmacyLicenseNumber",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "SmtpHost",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "SmtpPassword",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "SmtpPort",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "SmtpUsername",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "TwilioAccountSid",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "TwilioAuthToken",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "TwilioFromNumber",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "CompanySettings");
        }
    }
}
