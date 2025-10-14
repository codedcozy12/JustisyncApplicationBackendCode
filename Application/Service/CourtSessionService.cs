using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class CourtSessionService : ICourtSessionService
    {
        private readonly ILogger<CourtSessionService> _logger;
        private readonly ICourtSessionRepository _courtSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CourtSessionService(ILogger<CourtSessionService> logger, ICourtSessionRepository courtSessionRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _courtSessionRepository = courtSessionRepository;
        }
        public async Task<ResponseDto<CourtSessionDto>> EndCourtSessionAsync(Guid sessionId)
        {
            var session = await _courtSessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                _logger.LogWarning("Court session {SessionId} not found", sessionId);
                return ResponseDto<CourtSessionDto>.Fail("Court session not found.");
            }
            if (session.Status != CourtSessionStatus.IN_PROGRESS)
            {
                _logger.LogWarning("Court session {SessionId} is not in a state to end", sessionId);
                return ResponseDto<CourtSessionDto>.Fail("Court session cannot be ended in its current state.");
            }
            session.Status = CourtSessionStatus.COMPLETED;
            await _courtSessionRepository.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Court session {SessionId} ended successfully", sessionId);
            var dto = new CourtSessionDto
            {
                Id = session.Id,
                CaseId = session.CaseId,
                CourtId = session.CourtId,
                ScheduledDate = session.ScheduledDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Status = session.Status
            };
            return ResponseDto<CourtSessionDto>.Success(dto, "Court session ended successfully.");
        }
        public async Task<ResponseDto<IEnumerable<CourtSessionDto>>> GetByCaseIdAsync(Guid caseId)
        {
            var sessions = await _courtSessionRepository.GetByCaseAsync(caseId);
            var filteredSessions = sessions.Where(s => !s.IsDeleted);
            var dtoList = sessions.Select(s => new CourtSessionDto
            {
                Id = s.Id,
                CaseId = s.CaseId,
                CourtId = s.CourtId,
                CaseNumber = s.Case.CaseNumber,
                ScheduledDate = s.ScheduledDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status
            }).ToList();
            return ResponseDto<IEnumerable<CourtSessionDto>>.Success(dtoList);
        }
       public async Task<ResponseDto<IEnumerable<CourtSessionDto>>> GetByCourtIdAsync(Guid courtId)
        {
            var sessions = await _courtSessionRepository.GetByCourtAsync(courtId);
            var filteredSessions = sessions.Where(s => !s.IsDeleted);
            var dtoList = sessions.Select(s => new CourtSessionDto
            {
                Id = s.Id,
                CaseId = s.CaseId,
                CourtId = s.CourtId,
                CaseNumber = s.Case.CaseNumber,
                ScheduledDate = s.ScheduledDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status
            }).ToList();

            return ResponseDto<IEnumerable<CourtSessionDto>>.Success(dtoList);
        }
        public async Task<ResponseDto<CourtSessionDto>> GetByIdAsync(Guid id)
        {
            var session = await _courtSessionRepository.GetByIdAsync(id);
            if (session == null || session.IsDeleted)
            {
                _logger.LogWarning("Court session {SessionId} not found", id);
                return ResponseDto<CourtSessionDto>.Fail("Court session not found.");
            }

            var dto = new CourtSessionDto
            {
                Id = session.Id,
                CaseId = session.CaseId,
                CourtId = session.CourtId,
                CaseNumber = session.Case.CaseNumber,
                ScheduledDate = session.ScheduledDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Status = session.Status
            };
            return ResponseDto<CourtSessionDto>.Success(dto);
        }
        public async Task<ResponseDto<IEnumerable<CourtSessionDto>>> GetUpcomingSessionsAsync(DateTime from, DateTime to)
        {
            if (from > to)
            {
                _logger.LogWarning("Invalid date range for upcoming sessions: {From} - {To}", from, to);
                return ResponseDto<IEnumerable<CourtSessionDto>>.Fail("Invalid date range.");
            }

            var sessions = await _courtSessionRepository.GetUpcomingSessionsAsync(from, to);
            var dtoList = sessions.Select(s => new CourtSessionDto
            {
                Id = s.Id,
                CaseId = s.CaseId,
                CourtId = s.CourtId,
                ScheduledDate = s.ScheduledDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status
            }).ToList();

            return ResponseDto<IEnumerable<CourtSessionDto>>.Success(dtoList);
        }
        public async Task<ResponseDto<CourtSessionDto>> StartVirtualSessionAsync(Guid sessionId)
        {
            var session = await _courtSessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                _logger.LogWarning("Court session {SessionId} not found", sessionId);
                return ResponseDto<CourtSessionDto>.Fail("Court session not found.");
            }

            if (session.Status != CourtSessionStatus.SCHEDULED)
            {
                _logger.LogWarning("Court session {SessionId} is not in a state to start", sessionId);
                return ResponseDto<CourtSessionDto>.Fail("Court session cannot be started in its current state.");
            }

            var roomName = $"JustiSync-{session.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var jitsiBaseUrl = "https://meet.jit.si/";
            var meetingLink = $"{jitsiBaseUrl}{roomName}";

            session.Status = CourtSessionStatus.IN_PROGRESS;
            session.StartTime = DateTime.UtcNow;
            session.VirtualMeetingUrl = meetingLink;

            await _courtSessionRepository.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Court session {SessionId} started successfully with meeting link {MeetingLink}", sessionId, meetingLink);

            var dto = new CourtSessionDto
            {
                Id = session.Id,
                CaseId = session.CaseId,
                CourtId = session.CourtId,
                ScheduledDate = session.ScheduledDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Status = session.Status,
                VirtualMeetingRoom = roomName,
                VirtualMeetingUrl = meetingLink
            };

            return ResponseDto<CourtSessionDto>.Success(dto, "Court session started successfully.");
        }
        public async Task<ResponseDto<CourtSessionUpdateRequestDto>> UpdateAsync(Guid id, CourtSessionUpdateRequestDto dto)
        {
            _logger.LogInformation("Updating court session with ID {SessionId}", id);

            var session = await _courtSessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                _logger.LogWarning("Court session {SessionId} not found", id);
                return ResponseDto<CourtSessionUpdateRequestDto>.Fail("Court session not found.");
            }

            session.ScheduledDate = dto.ScheduledDate;
            session.StartTime = dto.StartTime;
            session.EndTime = dto.EndTime;
            session.Status = dto.Status;

            await _courtSessionRepository.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Court session {SessionId} updated successfully", id);

            var response = new CourtSessionUpdateRequestDto
            {
                ScheduledDate = session.ScheduledDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = session.Status
            };
            return ResponseDto<CourtSessionUpdateRequestDto>.Success(response, "Court session updated successfully.");
        }
    }
}