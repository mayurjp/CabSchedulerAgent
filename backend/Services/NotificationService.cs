using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Models;

namespace CabScheduler.Api.Services;

public class NotificationService
{
    private readonly CabSchedulerDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(CabSchedulerDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Notification> SendAsync(string recipient, string channel, string message)
    {
        var notification = new Notification
        {
            Recipient = recipient,
            Channel = channel,
            Message = message,
            SentAt = DateTime.UtcNow,
            Status = "Sent"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Notification sent to {Recipient} via {Channel}: {Message}",
            recipient, channel, message);

        return notification;
    }

    public async Task<List<Notification>> GetNotificationsAsync(string? channel = null)
    {
        IQueryable<Notification> query = _context.Notifications;
        if (!string.IsNullOrWhiteSpace(channel))
            query = query.Where(n => n.Channel == channel);
        return await query.OrderByDescending(n => n.SentAt).Take(50).ToListAsync();
    }
}
