using Application.Dto;
using Application.Interface;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;
        private readonly IAuditLogService _auditLogService;
        public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<NotificationService> logger, IAuditLogService auditLogService)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditLogService = auditLogService;
        }
        public async Task<ResponseDto<Guid>> CreateNotificationAsync(NotificationCreateRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.ToId);
            if(user == null)
            {
                _logger.LogWarning("Attempted to create notification for non-existent user with ID: {UserId}", dto.ToId);
                return new ResponseDto<Guid>
                {
                    Data = Guid.Empty,
                    IsSuccess = false,
                    Message = "User not found"
                };
            }
            var notification = new Notification
            {
                UserId = user.Id,
                Message = dto.Message,
                Title = dto.Title,
                IsRead = false,
                DateCreated = DateTime.UtcNow
            };
            await _notificationRepository.AddNotificationAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Notification created with ID: {NotificationId}", notification.Id);
            await _auditLogService.LogActionAsync(notification.UserId,"CreateNotification",nameof(Notification),notification.Id, $"Notification created with ID: {notification.Id}", Role.admin.ToString());
            return new ResponseDto<Guid>
            {
                Data = notification.Id,
                IsSuccess = true,
                Message = "Notification created successfully"
            };
        }
        public async Task<bool> DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
            if (notification == null)
            {
                _logger.LogWarning("Attempted to delete non-existent notification with ID: {NotificationId}", notificationId);
                return false;
            }
            notification.IsDeleted = true;
            await _notificationRepository.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Notification with ID: {NotificationId} deleted successfully", notificationId);
            return true;
        }
        public async Task<ResponseDto<NotificationDto?>> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                _logger.LogWarning("Notification with ID: {NotificationId} not found", id);
                return new ResponseDto<NotificationDto?>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Notification not found"
                };
            }
            var notificationDto = new NotificationDto
            {
                Id = notification.Id,
                ToId = notification.UserId,
                Message = notification.Message,
                IsRead = notification.IsRead,
                Title = notification.Title,
                DateCreated = notification.DateCreated
            };
            return new ResponseDto<NotificationDto?>
            {
                Data = notificationDto,
                IsSuccess = true,
                Message = "Notification retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<NotificationDto>>> GetNotificationsByUserIdAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
            var notificationDtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                ToId = n.UserId,
                Message = n.Message,
                IsRead = n.IsRead,
                Title = n.Title,
                DateCreated = n.DateCreated
            });
            return new ResponseDto<IEnumerable<NotificationDto>>
            {
                Data = notificationDtos,
                IsSuccess = true,
                Message = "Notifications retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<NotificationDto>>> GetUnreadNotificationsAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);
            var notificationDtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                ToId = n.UserId,
                Message = n.Message,
                IsRead = n.IsRead,
                Title = n.Title,
                DateCreated = n.DateCreated
            });
            return new ResponseDto<IEnumerable<NotificationDto>>
            {
                Data = notificationDtos,
                IsSuccess = true,
                Message = "Unread notifications retrieved successfully"
            };
        }
        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            var result = await _userRepository.GetByIdAsync(userId);
            if (result == null)
            {
                return false;
            }
            await _notificationRepository.MarkAsReadAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> MarkAsReadAsync(Guid notificationId)
        {
            var result = await _notificationRepository.GetNotificationByIdAsync(notificationId);
            if (result != null)
            {
                await _notificationRepository.MarkAsReadAsync(notificationId);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<ResponseDto<Guid>> SendManyNotificationsAsync(ManyNotificationRequestDto manyNotificationRequestDto)
        {
            var users = await _userRepository.GetAllUsersWithoutPaginationAsync();
            foreach (var user in users)
            {
                var notification = new Notification
                {
                    UserId = user.Id,
                    Message = manyNotificationRequestDto.Message,
                    Title = manyNotificationRequestDto.Title,
                    IsRead = false,
                    DateCreated = DateTime.UtcNow
                };
                await _notificationRepository.AddNotificationAsync(notification);
                _logger.LogInformation("Notification created for User ID: {UserId} with Notification ID: {NotificationId}", user.Id, notification.Id);
                await _auditLogService.LogActionAsync(user.Id, "CreateNotification", nameof(Notification), notification.Id, $"Notification created with ID: {notification.Id}", Role.admin.ToString());
            }
            await _unitOfWork.SaveChangesAsync();
            return new ResponseDto<Guid>
            {
                Data = Guid.NewGuid(),
                IsSuccess = true,
                Message = "Notifications sent successfully"
            };
        }
        public async Task<ResponseDto<NotificationDto>> UpdateNotificationAsync(Guid notificationId, NotificationUpdateRequestDto dto)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
            if (notification == null)
            {
                _logger.LogWarning("Notification with ID: {NotificationId} not found for update", notificationId);
                return new ResponseDto<NotificationDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Notification not found"
                };
            }
            notification.Message = dto.Message ?? notification.Message;
            notification.Title = dto.Title ?? notification.Title;
            notification.IsRead = dto.IsRead;
            await _notificationRepository.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            var updatedDto = new NotificationDto
            {
                Id = notification.Id,
                ToId = notification.UserId,
                Message = notification.Message,
                IsRead = notification.IsRead,
                Title = notification.Title,
                DateCreated = notification.DateCreated
            };
            _logger.LogInformation("Notification with ID: {NotificationId} updated successfully", notificationId);
            await _auditLogService.LogActionAsync(notification.UserId, "UpdateNotification", nameof(Notification), notification.Id, $"Notification updated with ID: {notification.Id}", Role.admin.ToString());
            return new ResponseDto<NotificationDto>
            {
                Data = updatedDto,
                IsSuccess = true,
                Message = "Notification updated successfully"
            };
        }
    }
}