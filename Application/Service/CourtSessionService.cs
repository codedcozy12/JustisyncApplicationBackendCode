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
        public async Task<ResponseDto<IEnumerable<CourtSessionDto>>> GetByCaseIdAsync(Guid caseId)
        {
            var sessions = await _courtSessionRepository.GetByCaseAsync(caseId);
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
            if (session == null)
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