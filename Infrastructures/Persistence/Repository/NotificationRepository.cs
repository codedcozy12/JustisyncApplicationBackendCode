using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }
        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }
        }
        public async Task<Notification?> GetNotificationByIdAsync(Guid id)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(Guid userId)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.Id)
                .ToListAsync();
        }
        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!notifications.Any())
                return false;

            foreach (var n in notifications)
                n.IsRead = true;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }
        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
        }
    }
}