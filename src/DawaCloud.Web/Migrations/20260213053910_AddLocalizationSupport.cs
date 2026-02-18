using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DawaCloud.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalizationSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Locale",
                table: "NotificationTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VariablePlaceholders",
                table: "NotificationTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultLocale",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EnableAutoTranslation",
                table: "CompanySettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SupportedLocales",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LocalizedReportDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Locale = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColumnDefinitionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedReportDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLocalePreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PreferredLocale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Use24HourFormat = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocalePreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLocalePreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedReportDefinitions_IsActive",
                table: "LocalizedReportDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedReportDefinitions_ReportType_Locale",
                table: "LocalizedReportDefinitions",
                columns: new[] { "ReportType", "Locale" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLocalePreferences_UserId",
                table: "UserLocalePreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalizedReportDefinitions");

            migrationBuilder.DropTable(
                name: "UserLocalePreferences");

            migrationBuilder.DropColumn(
                name: "Locale",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "VariablePlaceholders",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "DefaultLocale",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "EnableAutoTranslation",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "SupportedLocales",
                table: "CompanySettings");
        }
    }
}
