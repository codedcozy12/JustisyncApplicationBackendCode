using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface INotificationService
    {
        Task<ResponseDto<Guid>> CreateNotificationAsync(NotificationCreateRequestDto dto);
        Task<ResponseDto<NotificationDto?>> GetNotificationByIdAsync(Guid id);
        Task<ResponseDto<IEnumerable<NotificationDto>>> GetNotificationsByUserIdAsync(Guid userId);
        Task<ResponseDto<IEnumerable<NotificationDto>>> GetUnreadNotificationsAsync(Guid userId);
        Task<bool> MarkAllAsReadAsync(Guid userId);
        Task<bool> MarkAsReadAsync(Guid notificationId);
        Task<ResponseDto<Guid>> SendManyNotificationsAsync(ManyNotificationRequestDto manyNotificationRequestDto);
        Task<ResponseDto<NotificationDto>> UpdateNotificationAsync(Guid notificationId, NotificationUpdateRequestDto dto);
        Task<bool> DeleteNotificationAsync(Guid notificationId);
    }
}