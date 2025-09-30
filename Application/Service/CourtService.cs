using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class CourtService : ICourtService
    {
        private readonly ILogger<CourtService> _logger;
        private readonly ICourtRepository _courtRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly IAuditLogService _auditLogService;

        public CourtService(ILogger<CourtService> logger, ICourtRepository courtRepository, IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _logger = logger;
            _courtRepository = courtRepository;
            _unitofwork = unitOfWork;
            _auditLogService = auditLogService;
        }
        public async Task<ResponseDto<CourtCreateRequestDto>> CreateCourtAsync(CourtCreateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogError("Court creation request is null");
                return new ResponseDto<CourtCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "Invalid court creation request",
                    Data = null
                };
            }
            var validEmail = await _courtRepository.CheckIfEmailExists(request.ContactEmail);
            if (validEmail)
            {
                _logger.LogWarning("Court creation failed: Contact email {ContactEmail} already exists", request.ContactEmail);
                return new ResponseDto<CourtCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "Contact email already exists",
                    Data = null
                };
            }
            var validNumber = await _courtRepository.CheckIfNumberExist(request.ContactPhone);
            if(validNumber)
            {
                _logger.LogWarning("Court creation failed: Contact number {ContactPhone} already exists", request.ContactPhone);
                return new ResponseDto<CourtCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "Contact number already exists",
                    Data = null
                };
            }
            _logger.LogInformation("Creating new court with name: {Name}", request.Name);

            var court = new Court
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Type = request.Type,
                Address = request.Address,
                City = request.City,
                State = request.State,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Jurisdiction = request.Jurisdiction,
                Registrars = new List<Registrar>(),
                IsActive = true
            };
            await _courtRepository.AddAsync(court);
            await _unitofwork.SaveChangesAsync();
            _logger.LogInformation("Court created successfully with ID: {Id}", court.Id);
            await _auditLogService.LogActionAsync(court.Id, "Create", "Court", court.Id, $"Court {court.Name} created", Role.admin.ToString());
            return new ResponseDto<CourtCreateRequestDto>
            {
                IsSuccess = true,
                Message = "Court created successfully",
                Data = request
            };
        }
        public async Task<bool> DeleteCourtAsync(Guid courtId)
        {
            var court = await _courtRepository.GetByIdAsync(courtId);
            if (court == null)
            {
                _logger.LogWarning("Court with ID: {Id} not found for deletion", courtId);
                return false;
            }
            court.IsActive = false;
            court.IsDeleted = true;
            await _unitofwork.SaveChangesAsync();

            _logger.LogInformation("Court with ID: {Id} marked as inactive", courtId);
            return true;
        }
        public async Task<ResponseDto<IEnumerable<CourtDto>>> GetActiveCourtsAsync()
        {
            var courts = await _courtRepository.GetActiveCourtsAsync();
            var courtDtos = courts.Select(c => new CourtDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Address = c.Address,
                City = c.City,
                State = c.State,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                Jurisdiction = c.Jurisdiction,
                IsActive = c.IsActive
            });
            return new ResponseDto<IEnumerable<CourtDto>>
            {
                IsSuccess = true,
                Message = "Active courts retrieved successfully",
                Data = courtDtos
            };
        }
        public async Task<ResponseDto<IEnumerable<CourtDto>>> GetAllCourtsAsync()
        {
            var courts = await _courtRepository.GetAllAsync();
            if (courts == null)
            {
                _logger.LogInformation("No Courts found");
                return new ResponseDto<IEnumerable<CourtDto>>
                {
                    Data = Enumerable.Empty<CourtDto>(),
                    IsSuccess = true,
                    Message = "No Court found"
                };
            }
            var courtDtos = courts.Select(c => new CourtDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Address = c.Address,
                City = c.City,
                State = c.State,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                Jurisdiction = c.Jurisdiction,
                IsActive = c.IsActive
            });
            return new ResponseDto<IEnumerable<CourtDto>>
            {
                IsSuccess = true,
                Message = "All courts retrieved successfully",
                Data = courtDtos
            };
        }
        public async Task<ResponseDto<CourtDto>> GetCourtByIdAsync(Guid courtId)
        {
            var court = await _courtRepository.GetByIdAsync(courtId);
            if (court == null)
            {
                _logger.LogWarning("Court not found with Id: {CourtId}", courtId);
                return new ResponseDto<CourtDto> 
                { 
                  IsSuccess = false,
                  Message = "Court not found" };
            }

            var dto = new CourtDto
            {
                Id = court.Id,
                Name = court.Name,
                Type = court.Type,
                Address = court.Address,
                City = court.City,
                State = court.State,
                ContactEmail = court.ContactEmail,
                ContactPhone = court.ContactPhone,
                Jurisdiction = court.Jurisdiction,
                IsActive = court.IsActive
            };

            _logger.LogInformation("Court retrieved successfully with Id: {CourtId}", court.Id);

            return new ResponseDto<CourtDto> 
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<CourtUpdateRequestDto> UpdateCourtAsync(Guid courtId, CourtUpdateRequestDto request)
        {
            var court = await _courtRepository.GetByIdAsync(courtId);
            if (court == null)
            {
                _logger.LogWarning("Court with ID: {Id} not found for update", courtId);
                return null;
            }
            var validEmail = await _courtRepository.CheckIfEmailExists(request.ContactEmail);
            if (validEmail)
            {
                _logger.LogWarning("Court update failed: Contact email {ContactEmail} already exists", request.ContactEmail);
                return null;
            }
            var validNumber = await _courtRepository.CheckIfNumberExist(request.ContactPhone);
            if (validNumber)
            {
                _logger.LogWarning("Court update failed: Contact number {ContactPhone} already exists", request.ContactPhone);
                return null;
            }
            court.ContactEmail = request.ContactEmail;
            court.ContactPhone = request.ContactPhone;
            court.Jurisdiction = request.Jurisdiction;
            court.IsActive = request.IsActive;
            court.IsDeleted = !request.IsActive;
            await _courtRepository.UpdateAsync(court);
            await _unitofwork.SaveChangesAsync();
            _logger.LogInformation("Court with ID: {Id} updated successfully", courtId);
            await _auditLogService.LogActionAsync(court.Id, "Update", "Court", court.Id, $"Court {court.Name} updated.", Role.admin.ToString());
            return request;
        }
    }
}