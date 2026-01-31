using DawaFlow.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace DawaFlow.Web.Infrastructure.Middleware;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditFields(eventData.Context);
            await CreateAuditLogs(eventData.Context);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContext context)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var entries = context.ChangeTracker.Entries<BaseAuditableEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = userId;
                    break;
            }
        }
    }

    private async Task CreateAuditLogs(DbContext context)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity is not AuditLog)
            .ToList();

        foreach (var entry in entries)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                UserName = userId,
                Action = entry.State.ToString(),
                EntityType = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKeyValue(entry),
                OldValues = entry.State == EntityState.Modified ? GetOldValues(entry) : null,
                NewValues = entry.State is EntityState.Added or EntityState.Modified ? GetNewValues(entry) : null,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            context.Set<AuditLog>().Add(auditLog);
        }

        await Task.CompletedTask;
    }

    private static string? GetPrimaryKeyValue(EntityEntry entry)
    {
        var keyName = entry.Metadata.FindPrimaryKey()?.Properties
            .Select(x => x.Name)
            .FirstOrDefault();

        if (keyName != null)
        {
            var value = entry.Property(keyName).CurrentValue;
            return value?.ToString();
        }

        return null;
    }

    private static string? GetOldValues(EntityEntry entry)
    {
        var result = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.IsModified && property.OriginalValue != null)
            {
                result[property.Metadata.Name] = property.OriginalValue;
            }
        }

        return result.Count > 0 ? JsonSerializer.Serialize(result) : null;
    }

    private static string? GetNewValues(EntityEntry entry)
    {
        var result = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (entry.State == EntityState.Added || property.IsModified)
            {
                result[property.Metadata.Name] = property.CurrentValue;
            }
        }

        return result.Count > 0 ? JsonSerializer.Serialize(result) : null;
    }
}
