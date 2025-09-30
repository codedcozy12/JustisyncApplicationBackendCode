using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task<Notification?> GetNotificationByIdAsync(Guid id);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(Guid userId);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task<bool> MarkAllAsReadAsync(Guid userId);
        Task UpdateAsync(Notification notification);
        Task DeleteNotificationAsync(Guid notificationId);
    }
}