using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;

namespace DawaCloud.Web.Features.Notifications.Services;

public interface INotificationService
{
    Task SendAsync(Notification notification, CancellationToken ct = default);
    Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
    Task SendWhatsAppAsync(string phone, string message, CancellationToken ct = default);
    Task SendInAppAsync(string userId, string title, string message, NotificationType type, CancellationToken ct = default);
    Task<List<Notification>> GetUserNotificationsAsync(string userId, bool unreadOnly = false, CancellationToken ct = default);
    Task MarkAsReadAsync(int notificationId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken ct = default);
}

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification, CancellationToken ct = default)
    {
        try
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(ct);

            switch (notification.Channel)
            {
                case NotificationChannel.Email:
                    if (!string.IsNullOrEmpty(notification.RecipientEmail))
                    {
                        await SendEmailAsync(notification.RecipientEmail, notification.Title, notification.Message, ct);
                    }
                    break;
                case NotificationChannel.WhatsApp:
                    if (!string.IsNullOrEmpty(notification.RecipientPhone))
                    {
                        await SendWhatsAppAsync(notification.RecipientPhone, notification.Message, ct);
                    }
                    break;
                case NotificationChannel.InApp:
                    // Already saved, no additional action needed
                    break;
            }

            notification.SentAt = DateTime.UtcNow;
            notification.Status = NotificationStatus.Sent;
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification {NotificationId}", notification.Id);
            notification.Status = NotificationStatus.Failed;
            notification.ErrorMessage = ex.Message;
            notification.RetryCount++;
            await _context.SaveChangesAsync(ct);
            throw;
        }
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
        var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@revopharma.com";
        var fromName = _configuration["Email:FromName"] ?? "Revo Pharma & Medical Ltd";
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];

        if (string.IsNullOrEmpty(smtpHost))
        {
            _logger.LogWarning("SMTP not configured, email not sent to {To}", to);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls, ct);
        
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            await client.AuthenticateAsync(username, password, ct);
        }
        
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
        
        _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
    }

    public async Task SendWhatsAppAsync(string phone, string message, CancellationToken ct = default)
    {
        var accountSid = _configuration["WhatsApp:AccountSid"];
        var authToken = _configuration["WhatsApp:AuthToken"];
        var fromNumber = _configuration["WhatsApp:FromNumber"];

        if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
        {
            _logger.LogWarning("WhatsApp/Twilio not configured, message not sent to {Phone}", phone);
            return;
        }

        // Using Twilio for WhatsApp
        Twilio.TwilioClient.Init(accountSid, authToken);
        
        var messageResult = await Twilio.Rest.Api.V2010.Account.MessageResource.CreateAsync(
            body: message,
            from: new Twilio.Types.PhoneNumber($"whatsapp:{fromNumber}"),
            to: new Twilio.Types.PhoneNumber($"whatsapp:{phone}")
        );

        _logger.LogInformation("WhatsApp message sent to {Phone}, SID: {Sid}", phone, messageResult.Sid);
    }

    public async Task SendInAppAsync(string userId, string title, string message, NotificationType type, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            Title = title,
            Message = message,
            Type = type,
            Channel = NotificationChannel.InApp,
            RecipientUserId = userId,
            Status = NotificationStatus.Sent,
            SentAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("In-app notification created for user {UserId}: {Title}", userId, title);
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(string userId, bool unreadOnly = false, CancellationToken ct = default)
    {
        var query = _context.Notifications
            .Where(n => n.RecipientUserId == userId && n.Channel == NotificationChannel.InApp)
            .OrderByDescending(n => n.CreatedAt);

        if (unreadOnly)
        {
            query = (IOrderedQueryable<Notification>)query.Where(n => n.ReadAt == null);
        }

        return await query.Take(50).ToListAsync(ct);
    }

    public async Task MarkAsReadAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _context.Notifications.FindAsync(new object[] { notificationId }, ct);
        if (notification != null && notification.ReadAt == null)
        {
            notification.ReadAt = DateTime.UtcNow;
            notification.Status = NotificationStatus.Read;
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task MarkAllAsReadAsync(string userId, CancellationToken ct = default)
    {
        await _context.Notifications
            .Where(n => n.RecipientUserId == userId && n.ReadAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.ReadAt, DateTime.UtcNow)
                .SetProperty(n => n.Status, NotificationStatus.Read), ct);
    }
}
