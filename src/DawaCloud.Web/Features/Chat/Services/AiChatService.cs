using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DawaCloud.Web.Features.Chat.Services;

public interface IAiChatService
{
    Task<string> AskAsync(string userMessage, List<ChatMessage> history);
}

public class AiChatSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "claude-sonnet-4-5-20250929";
    public string Endpoint { get; set; } = "https://api.anthropic.com/v1/messages";
}

public class ChatMessage
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class AiChatService : IAiChatService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AiChatSettings _settings;
    private readonly ILogger<AiChatService> _logger;

    public AiChatService(
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory scopeFactory,
        Microsoft.Extensions.Options.IOptions<AiChatSettings> settings,
        ILogger<AiChatService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> AskAsync(string userMessage, List<ChatMessage> history)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return "AI Chat is not configured yet. Please add your Anthropic API key in Settings > AI Configuration or in appsettings.json under the \"AiChat\" section.";

        try
        {
            var systemContext = await BuildSystemContextAsync();

            var messages = new List<object>();

            // Add recent history (last 10 messages)
            foreach (var msg in history.TakeLast(10))
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }

            messages.Add(new { role = "user", content = userMessage });

            var requestBody = new
            {
                model = _settings.Model,
                system = systemContext,
                messages,
                temperature = 0.3,
                max_tokens = 2000
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_settings.Endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("AI API error: {StatusCode} - {Body}", response.StatusCode, errorBody);
                return $"Sorry, I encountered an error communicating with the AI service. Status: {response.StatusCode}";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var contentBlocks = doc.RootElement.GetProperty("content");
            var textBlock = contentBlocks.EnumerateArray()
                .FirstOrDefault(b => b.GetProperty("type").GetString() == "text");
            var assistantMessage = textBlock.ValueKind != JsonValueKind.Undefined
                ? textBlock.GetProperty("text").GetString()
                : null;

            return assistantMessage ?? "I'm sorry, I couldn't generate a response.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AI Chat");
            return "Sorry, an error occurred while processing your question. Please try again.";
        }
    }

    private async Task<string> BuildSystemContextAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var totalDrugs = await db.Drugs.CountAsync(d => d.IsActive && !d.IsDeleted);
        var totalCategories = await db.DrugCategories.CountAsync(c => !c.IsDeleted);

        var activeBatches = await db.Batches
            .Where(b => b.Status == BatchStatus.Active && b.CurrentQuantity > 0)
            .ToListAsync();
        var totalStock = activeBatches.Sum(b => b.CurrentQuantity);
        var inventoryValue = activeBatches.Sum(b => b.CurrentQuantity * b.CostPrice);

        var expiringIn30 = await db.Batches
            .CountAsync(b => b.Status == BatchStatus.Active && b.CurrentQuantity > 0 && b.ExpiryDate <= DateTime.UtcNow.AddDays(30));

        var lowStockCount = await db.Drugs
            .Where(d => d.IsActive && !d.IsDeleted && d.ReorderLevel > 0)
            .Where(d => d.Batches!.Where(b => b.Status == BatchStatus.Active).Sum(b => b.CurrentQuantity) <= d.ReorderLevel)
            .CountAsync();

        var today = DateTime.UtcNow.Date;
        var thisMonth = new DateTime(today.Year, today.Month, 1);

        var salesToday = await db.RetailSales
            .Where(s => s.SaleDate >= today && !s.IsDeleted)
            .SumAsync(s => s.TotalAmount);

        var salesThisMonth = await db.RetailSales
            .Where(s => s.SaleDate >= thisMonth && !s.IsDeleted)
            .SumAsync(s => s.TotalAmount);

        var ordersToday = await db.RetailSales
            .CountAsync(s => s.SaleDate >= today && !s.IsDeleted);

        var totalSuppliers = await db.Suppliers.CountAsync(s => s.IsActive && !s.IsDeleted);

        var pendingExpenses = await db.Expenses
            .CountAsync(e => e.Status == ExpenseStatus.Pending && !e.IsDeleted);

        var approvedExpensesThisMonth = await db.Expenses
            .Where(e => e.Date >= thisMonth && e.Status == ExpenseStatus.Approved && !e.IsDeleted)
            .SumAsync(e => e.AmountBase);

        // Top 10 drugs by stock value
        var topDrugsByValue = await db.Batches
            .Include(b => b.Drug)
            .Where(b => b.Status == BatchStatus.Active && b.CurrentQuantity > 0)
            .GroupBy(b => b.Drug!.Name)
            .Select(g => new { Drug = g.Key, Value = g.Sum(b => b.CurrentQuantity * b.CostPrice), Qty = g.Sum(b => b.CurrentQuantity) })
            .OrderByDescending(d => d.Value)
            .Take(10)
            .ToListAsync();

        var topDrugsStr = string.Join("\n", topDrugsByValue.Select((d, i) => $"  {i + 1}. {d.Drug}: {d.Qty} units, value ${d.Value:N2}"));

        // Recent low stock drugs
        var lowStockDrugs = await db.Drugs
            .Where(d => d.IsActive && !d.IsDeleted && d.ReorderLevel > 0)
            .Select(d => new { d.Name, Stock = d.Batches!.Where(b => b.Status == BatchStatus.Active).Sum(b => b.CurrentQuantity), d.ReorderLevel })
            .Where(d => d.Stock <= d.ReorderLevel)
            .OrderBy(d => (double)d.Stock / d.ReorderLevel)
            .Take(10)
            .ToListAsync();

        var lowStockStr = lowStockDrugs.Any()
            ? string.Join("\n", lowStockDrugs.Select(d => $"  - {d.Name}: {d.Stock} units (reorder level: {d.ReorderLevel})"))
            : "  None";

        // Expiring soon batches
        var expiringBatches = await db.Batches
            .Include(b => b.Drug)
            .Where(b => b.Status == BatchStatus.Active && b.CurrentQuantity > 0 && b.ExpiryDate <= DateTime.UtcNow.AddDays(60))
            .OrderBy(b => b.ExpiryDate)
            .Take(10)
            .Select(b => new { b.Drug!.Name, b.BatchNumber, b.ExpiryDate, b.CurrentQuantity })
            .ToListAsync();

        var expiringStr = expiringBatches.Any()
            ? string.Join("\n", expiringBatches.Select(b => $"  - {b.Name} (Batch: {b.BatchNumber}): expires {b.ExpiryDate:yyyy-MM-dd}, {b.CurrentQuantity} units"))
            : "  None";

        return $@"You are DawaCloud AI Assistant, a helpful pharmacy management system assistant.
You have access to the following real-time pharmacy data. Use this data to answer the user's questions accurately.
Always provide specific numbers when asked about quantities, values, or counts.
Format currency values with $ symbol. Be concise but thorough.

=== CURRENT PHARMACY DATA (as of {DateTime.UtcNow:yyyy-MM-dd HH:mm}) ===

INVENTORY OVERVIEW:
- Total active drugs: {totalDrugs}
- Total drug categories: {totalCategories}
- Total stock units: {totalStock:N0}
- Total inventory value: ${inventoryValue:N2}
- Batches expiring within 30 days: {expiringIn30}
- Drugs below reorder level: {lowStockCount}

SALES:
- Sales today: ${salesToday:N2} ({ordersToday} orders)
- Sales this month: ${salesThisMonth:N2}

EXPENSES:
- Pending expense approvals: {pendingExpenses}
- Approved expenses this month: ${approvedExpensesThisMonth:N2}

SUPPLIERS:
- Active suppliers: {totalSuppliers}

TOP 10 DRUGS BY INVENTORY VALUE:
{topDrugsStr}

LOW STOCK ALERTS:
{lowStockStr}

EXPIRING SOON (within 60 days):
{expiringStr}

=== INSTRUCTIONS ===
- Answer questions about drugs, inventory, sales, expenses, suppliers, and pharmacy operations.
- When the user asks about specific drugs, refer to the data above.
- If you don't have enough data to answer, say so clearly.
- Suggest actions when appropriate (e.g., reorder stock, check expiring batches).
- Keep responses focused on pharmacy management.
- Do not make up data that isn't provided above.";
    }
}
