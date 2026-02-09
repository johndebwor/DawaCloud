using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DawaCloud.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFieldsToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "WholesaleSales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsed",
                table: "WholesaleSales",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultCurrencyId",
                table: "WholesaleCustomers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "RetailSales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Quotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsed",
                table: "Quotations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountBase",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsed",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "GoodsReceipts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsed",
                table: "GoodsReceipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountBase",
                table: "GoodsReceipts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCostBase",
                table: "GoodsReceiptItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPriceOriginal",
                table: "Drugs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseCurrencyId",
                table: "Drugs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualAmountBase",
                table: "DrugRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "DrugRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsed",
                table: "DrugRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountBase",
                table: "DrugRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuotedPriceBase",
                table: "DrugRequestItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPriceBase",
                table: "DrugRequestItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPriceBase",
                table: "DrugRequestItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BaseCurrencyId",
                table: "CompanySettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultPurchaseCurrencyId",
                table: "CompanySettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultSalesCurrencyId",
                table: "CompanySettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "CashierShifts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostCurrencyId",
                table: "Batches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPriceOriginal",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsed",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WholesaleSales_CurrencyId",
                table: "WholesaleSales",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_WholesaleCustomers_DefaultCurrencyId",
                table: "WholesaleCustomers",
                column: "DefaultCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailSales_CurrencyId",
                table: "RetailSales",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CurrencyId",
                table: "Quotations",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CurrencyId",
                table: "Payments",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_CurrencyId",
                table: "GoodsReceipts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_PurchaseCurrencyId",
                table: "Drugs",
                column: "PurchaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_DrugRequests_CurrencyId",
                table: "DrugRequests",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettings_BaseCurrencyId",
                table: "CompanySettings",
                column: "BaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettings_DefaultPurchaseCurrencyId",
                table: "CompanySettings",
                column: "DefaultPurchaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettings_DefaultSalesCurrencyId",
                table: "CompanySettings",
                column: "DefaultSalesCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashierShifts_CurrencyId",
                table: "CashierShifts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CostCurrencyId",
                table: "Batches",
                column: "CostCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Currencies_CostCurrencyId",
                table: "Batches",
                column: "CostCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CashierShifts_Currencies_CurrencyId",
                table: "CashierShifts",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanySettings_Currencies_BaseCurrencyId",
                table: "CompanySettings",
                column: "BaseCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanySettings_Currencies_DefaultPurchaseCurrencyId",
                table: "CompanySettings",
                column: "DefaultPurchaseCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanySettings_Currencies_DefaultSalesCurrencyId",
                table: "CompanySettings",
                column: "DefaultSalesCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DrugRequests_Currencies_CurrencyId",
                table: "DrugRequests",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_Currencies_PurchaseCurrencyId",
                table: "Drugs",
                column: "PurchaseCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceipts_Currencies_CurrencyId",
                table: "GoodsReceipts",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Currencies_CurrencyId",
                table: "Payments",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Currencies_CurrencyId",
                table: "Quotations",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RetailSales_Currencies_CurrencyId",
                table: "RetailSales",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WholesaleCustomers_Currencies_DefaultCurrencyId",
                table: "WholesaleCustomers",
                column: "DefaultCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WholesaleSales_Currencies_CurrencyId",
                table: "WholesaleSales",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Currencies_CostCurrencyId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_CashierShifts_Currencies_CurrencyId",
                table: "CashierShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanySettings_Currencies_BaseCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanySettings_Currencies_DefaultPurchaseCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanySettings_Currencies_DefaultSalesCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropForeignKey(
                name: "FK_DrugRequests_Currencies_CurrencyId",
                table: "DrugRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_Currencies_PurchaseCurrencyId",
                table: "Drugs");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceipts_Currencies_CurrencyId",
                table: "GoodsReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Currencies_CurrencyId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Currencies_CurrencyId",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_RetailSales_Currencies_CurrencyId",
                table: "RetailSales");

            migrationBuilder.DropForeignKey(
                name: "FK_WholesaleCustomers_Currencies_DefaultCurrencyId",
                table: "WholesaleCustomers");

            migrationBuilder.DropForeignKey(
                name: "FK_WholesaleSales_Currencies_CurrencyId",
                table: "WholesaleSales");

            migrationBuilder.DropIndex(
                name: "IX_WholesaleSales_CurrencyId",
                table: "WholesaleSales");

            migrationBuilder.DropIndex(
                name: "IX_WholesaleCustomers_DefaultCurrencyId",
                table: "WholesaleCustomers");

            migrationBuilder.DropIndex(
                name: "IX_RetailSales_CurrencyId",
                table: "RetailSales");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_CurrencyId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CurrencyId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipts_CurrencyId",
                table: "GoodsReceipts");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_PurchaseCurrencyId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_DrugRequests_CurrencyId",
                table: "DrugRequests");

            migrationBuilder.DropIndex(
                name: "IX_CompanySettings_BaseCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropIndex(
                name: "IX_CompanySettings_DefaultPurchaseCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropIndex(
                name: "IX_CompanySettings_DefaultSalesCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropIndex(
                name: "IX_CashierShifts_CurrencyId",
                table: "CashierShifts");

            migrationBuilder.DropIndex(
                name: "IX_Batches_CostCurrencyId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "WholesaleSales");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsed",
                table: "WholesaleSales");

            migrationBuilder.DropColumn(
                name: "DefaultCurrencyId",
                table: "WholesaleCustomers");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "RetailSales");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsed",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "AmountBase",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsed",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsed",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "TotalAmountBase",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "UnitCostBase",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "CostPriceOriginal",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "PurchaseCurrencyId",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "ActualAmountBase",
                table: "DrugRequests");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "DrugRequests");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsed",
                table: "DrugRequests");

            migrationBuilder.DropColumn(
                name: "TotalAmountBase",
                table: "DrugRequests");

            migrationBuilder.DropColumn(
                name: "QuotedPriceBase",
                table: "DrugRequestItems");

            migrationBuilder.DropColumn(
                name: "TotalPriceBase",
                table: "DrugRequestItems");

            migrationBuilder.DropColumn(
                name: "UnitPriceBase",
                table: "DrugRequestItems");

            migrationBuilder.DropColumn(
                name: "BaseCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultPurchaseCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultSalesCurrencyId",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "CashierShifts");

            migrationBuilder.DropColumn(
                name: "CostCurrencyId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "CostPriceOriginal",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsed",
                table: "Batches");
        }
    }
}
