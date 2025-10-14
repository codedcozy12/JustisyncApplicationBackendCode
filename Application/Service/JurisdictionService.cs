using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class JurisdictionService : IJurisdictionService
    {
        private readonly IJurisdictionRepository _jurisdictionRepository;
        private readonly ICourtRepository courtRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JurisdictionService> _logger;
        public JurisdictionService(IJurisdictionRepository jurisdictionRepository, IUnitOfWork unitOfWork, ILogger<JurisdictionService> logger, ICourtRepository courtRepository)
        {
            _jurisdictionRepository = jurisdictionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            this.courtRepository = courtRepository;
        }

        public async Task<ResponseDto<Guid>> AddJurisdictionToCourt(List<Guid> Ids,Guid courtId)
        {
            var court = await courtRepository.GetByIdAsync(courtId);
            if (court == null)
            {
                return new ResponseDto<Guid>
                {
                    Data = Guid.Empty,
                    IsSuccess = false,
                    Message = "Court not found."
                };
            }
            if (Ids.Count > 0)
            {
                foreach (var item in Ids)
                {
                   var jurisdiction = await _jurisdictionRepository.GetByIdAsync(item);
                    if (jurisdiction != null)
                    {
                        jurisdiction.Courts.Add(court);
                        await _jurisdictionRepository.UpdateAsync(jurisdiction);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
            return new ResponseDto<Guid>
            {
                Data = court.Id,
                IsSuccess = true,
                Message = "Jurisdictions added to court successfully."
            };
        }

        public async Task<ResponseDto<Guid>> CreateAsync(JurisdictionCreateRequestDto dto)
        {
            try
            {
                var jurisdiction = new Jurisdiction
                {
                    Id = Guid.NewGuid(),
                    SubjectMatter = dto.SubjectMatter,
                    MonetaryLimit = dto.MonetaryLimit
                };
                if (dto.CourtIds != null && dto.CourtIds.Any())
                {
                    List<Court> courts = new List<Court>();
                    foreach (var court in dto.CourtIds)
                    {
                        var existingCourt = await courtRepository.GetByIdAsync(court);
                        if (existingCourt != null)
                        {
                            courts.Add(existingCourt);
                        }
                    }
                    jurisdiction.Courts = courts;
                }
                await _jurisdictionRepository.AddAsync(jurisdiction);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDto<Guid>
                {
                    Data = jurisdiction.Id,
                    IsSuccess = true,
                    Message = "Jurisdiction created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating jurisdiction");
                return new ResponseDto<Guid>
                {
                    Data = Guid.Empty,
                    IsSuccess = false,
                    Message = $"Error creating jurisdiction: {ex.Message}"
                };
            }
        }
        public async Task<IEnumerable<ResponseDto<JurisdictionDto>>> GetAllAsync()
        {
            var jurisdictions = await _jurisdictionRepository.GetAllAsync();
            if (jurisdictions == null)
            {
                _logger.LogWarning("No jurisdictions found.");
                return Enumerable.Empty<ResponseDto<JurisdictionDto>>();
            }
            return jurisdictions.Select(j => new ResponseDto<JurisdictionDto>
            {
                Data = new JurisdictionDto
                {
                    Id = j.Id,
                    SubjectMatter = j.SubjectMatter,
                    MonetaryLimit = j.MonetaryLimit,
                    CourtIds = j.Courts?.Select(c => c.Id).ToList() ?? new List<Guid>()
                },
                IsSuccess = true,
                Message = "Jurisdiction retrieved successfully."
            });
        }

        public async Task<IEnumerable<ResponseDto<JurisdictionDto>>> GetByCourtIdAsync(Guid courtId)
        {
            var jurisdictions = await _jurisdictionRepository.GetByCourtIdAsync(courtId);
            if (jurisdictions == null || !jurisdictions.Any())
            {
                _logger.LogWarning("No jurisdictions found for Court ID {CourtId}.", courtId);
                return Enumerable.Empty<ResponseDto<JurisdictionDto>>();
            }
            return jurisdictions.Select(j => new ResponseDto<JurisdictionDto>
            {
                Data = new JurisdictionDto
                {
                    Id = j.Id,
                    SubjectMatter = j.SubjectMatter,
                    MonetaryLimit = j.MonetaryLimit,
                    CourtIds = j.Courts?.Select(c => c.Id).ToList() ?? new List<Guid>()
                },
                IsSuccess = true,
                Message = "Jurisdiction retrieved successfully."
            });
        }
        public async Task<ResponseDto<JurisdictionDto>> GetByIdAsync(Guid id)
        {
            var jurisdiction = await _jurisdictionRepository.GetByIdAsync(id);
            if (jurisdiction == null)
            {
                _logger.LogWarning("Jurisdiction with ID {Id} not found.", id);
                return new ResponseDto<JurisdictionDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Jurisdiction not found."
                };
            }
            return new ResponseDto<JurisdictionDto>
            {
                Data = new JurisdictionDto
                {
                    Id = jurisdiction.Id,
                    SubjectMatter = jurisdiction.SubjectMatter,
                    MonetaryLimit = jurisdiction.MonetaryLimit,
                    CourtIds = jurisdiction.Courts?.Select(c => c.Id).ToList() ?? new List<Guid>()
                },
                IsSuccess = true,
                Message = "Jurisdiction retrieved successfully."
            };
        }
        public async Task<IEnumerable<ResponseDto<JurisdictionDto>>> GetBySubjectMatterAsync(string subjectMatter)
        {
            var jurisdictions = await _jurisdictionRepository.GetBySubjectMatterAsync(subjectMatter);
            if (jurisdictions == null || !jurisdictions.Any())
            {
                _logger.LogWarning("No jurisdictions found for Subject Matter {SubjectMatter}.", subjectMatter);
                return Enumerable.Empty<ResponseDto<JurisdictionDto>>();
            }
            return jurisdictions.Select(j => new ResponseDto<JurisdictionDto>
            {
                Data = new JurisdictionDto
                {
                    Id = j.Id,
                    SubjectMatter = j.SubjectMatter,
                    MonetaryLimit = j.MonetaryLimit,
                    CourtIds = j.Courts?.Select(c => c.Id).ToList() ?? new List<Guid>()
                },
                IsSuccess = true,
                Message = "Jurisdiction retrieved successfully."
            });
        }
        public async Task<ResponseDto<Guid>> UpdateAsync(JurisdictionUpdateRequestDto dto)
        {
            try
            {
                var jurisdiction = await _jurisdictionRepository.GetByIdAsync(dto.Id);
                if (jurisdiction == null)
                {
                    _logger.LogWarning("Jurisdiction with ID {Id} not found for update.", dto.Id);
                    return new ResponseDto<Guid>
                    {
                        Data = Guid.Empty,
                        IsSuccess = false,
                        Message = "Jurisdiction not found."
                    };
                }
                jurisdiction.SubjectMatter = dto.SubjectMatter ?? jurisdiction.SubjectMatter;
                jurisdiction.MonetaryLimit = dto.MonetaryLimit != 0 ? dto.MonetaryLimit : jurisdiction.MonetaryLimit;
                var updatedJurisdiction = await _jurisdictionRepository.UpdateAsync(jurisdiction);
                if (updatedJurisdiction == null)
                {
                    _logger.LogError("Failed to update jurisdiction with ID {Id}.", dto.Id);
                    return new ResponseDto<Guid>
                    {
                        Data = Guid.Empty,
                        IsSuccess = false,
                        Message = "Failed to update jurisdiction."
                    };
                }
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDto<Guid>
                {
                    Data = updatedJurisdiction.Id,
                    IsSuccess = true,
                    Message = "Jurisdiction updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating jurisdiction with ID {Id}.", dto.Id);
                return new ResponseDto<Guid>
                {
                    Data = Guid.Empty,
                    IsSuccess = false,
                    Message = $"Error updating jurisdiction: {ex.Message}"
                };
            }
        }
    }
}