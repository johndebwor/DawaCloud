using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DawaFlow.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPosDisplayStyleSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosProductDisplayStyle",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosProductDisplayStyle",
                table: "CompanySettings");
        }
    }
}
