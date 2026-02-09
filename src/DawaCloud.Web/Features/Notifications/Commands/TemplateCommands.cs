using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Notifications.Commands;

// Create Template
public record CreateNotificationTemplateCommand(
    string Name,
    string Code,
    NotificationType Type,
    NotificationChannel Channel,
    string Subject,
    string Body,
    bool IsActive
) : IRequest<int>;

public class CreateNotificationTemplateCommandHandler : IRequestHandler<CreateNotificationTemplateCommand, int>
{
    private readonly AppDbContext _context;

    public CreateNotificationTemplateCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateNotificationTemplateCommand request, CancellationToken ct)
    {
        var template = new NotificationTemplate
        {
            Name = request.Name,
            Code = request.Code,
            Type = request.Type,
            Channel = request.Channel,
            Subject = request.Subject,
            Body = request.Body,
            IsActive = request.IsActive
        };

        _context.NotificationTemplates.Add(template);
        await _context.SaveChangesAsync(ct);

        return template.Id;
    }
}

// Update Template
public record UpdateNotificationTemplateCommand(
    int Id,
    string Name,
    string Code,
    NotificationType Type,
    NotificationChannel Channel,
    string Subject,
    string Body,
    bool IsActive
) : IRequest;

public class UpdateNotificationTemplateCommandHandler : IRequestHandler<UpdateNotificationTemplateCommand>
{
    private readonly AppDbContext _context;

    public UpdateNotificationTemplateCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateNotificationTemplateCommand request, CancellationToken ct)
    {
        var template = await _context.NotificationTemplates.FindAsync(new object[] { request.Id }, ct);

        if (template != null)
        {
            template.Name = request.Name;
            template.Code = request.Code;
            template.Type = request.Type;
            template.Channel = request.Channel;
            template.Subject = request.Subject;
            template.Body = request.Body;
            template.IsActive = request.IsActive;

            await _context.SaveChangesAsync(ct);
        }
    }
}

// Delete Template
public record DeleteNotificationTemplateCommand(int Id) : IRequest;

public class DeleteNotificationTemplateCommandHandler : IRequestHandler<DeleteNotificationTemplateCommand>
{
    private readonly AppDbContext _context;

    public DeleteNotificationTemplateCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteNotificationTemplateCommand request, CancellationToken ct)
    {
        var template = await _context.NotificationTemplates.FindAsync(new object[] { request.Id }, ct);

        if (template != null)
        {
            _context.NotificationTemplates.Remove(template);
            await _context.SaveChangesAsync(ct);
        }
    }
}
