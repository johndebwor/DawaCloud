using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DawaCloud.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseRecurrenceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlyDayOfMonth",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyDayOfWeek",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyRecurrenceType",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyWeekPosition",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OccurrenceNumber",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceEndAfterCount",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurrenceEndDate",
                table: "Expenses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceEndType",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceFrequency",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceInterval",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceTemplateId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeeklyDays",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_RecurrenceTemplateId",
                table: "Expenses",
                column: "RecurrenceTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Expenses_RecurrenceTemplateId",
                table: "Expenses",
                column: "RecurrenceTemplateId",
                principalTable: "Expenses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Expenses_RecurrenceTemplateId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_RecurrenceTemplateId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "MonthlyDayOfMonth",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "MonthlyDayOfWeek",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "MonthlyRecurrenceType",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "MonthlyWeekPosition",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "OccurrenceNumber",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndAfterCount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndDate",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndType",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurrenceFrequency",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurrenceInterval",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurrenceTemplateId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "WeeklyDays",
                table: "Expenses");
        }
    }
}
