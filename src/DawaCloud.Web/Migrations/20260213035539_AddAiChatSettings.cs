using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DawaCloud.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAiChatSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiChatApiKey",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AiChatEnabled",
                table: "CompanySettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AiChatEndpoint",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiChatModel",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiChatApiKey",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "AiChatEnabled",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "AiChatEndpoint",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "AiChatModel",
                table: "CompanySettings");
        }
    }
}
