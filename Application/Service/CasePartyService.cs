using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class CasePartyService : ICasePartyService
    {
        private readonly ICasePartyRepository _casePartyRepository;
        private readonly ICaseRepository _caseRepository;
        private readonly ILogger<CasePartyService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CasePartyService(ICasePartyRepository casePartyRepository, ICaseRepository caseRepository, ILogger<CasePartyService> logger, IUnitOfWork unitOfWork)
        {
            _casePartyRepository = casePartyRepository;
            _caseRepository = caseRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<CasePartyCreateDto>> AddCasePartyAsync(CasePartyCreateDto newParty)
        {
            var caseEntity = await _caseRepository.GetCaseByIdAsync(newParty.CaseId!.Value);
            if (caseEntity == null)
                return ResponseDto<CasePartyCreateDto>.Fail("Case not found.");

            var party = new CaseParty
            {
                Id = Guid.NewGuid(),
                FullName = newParty.FullName,
                ContactEmail = newParty.Email,
                Role = newParty.Role,
                ContactPhone = newParty.Contact,
            };

            await _casePartyRepository.AddCasePartyAsync(party);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseDto<CasePartyCreateDto>
            {
                IsSuccess = true,
                Message = "Court created successfully",
                Data = newParty
            };
        }
        public async Task<ResponseDto<IEnumerable<CasePartyDto>>> GetCasePartiesByCaseIdAsync(Guid caseId)
        {
            var parties = await _casePartyRepository.GetCasePartiesByCaseAsync(caseId);
            if (!parties.Any())
                return ResponseDto<IEnumerable<CasePartyDto>>.Fail("No parties found for this case.");

            var dtoList = parties.Select(p => new CasePartyDto
            {
                Id = p.Id,
                CaseId = p.CaseId,
                FullName = p.FullName,
                Contact = p.ContactPhone,
                Role = p.Role,
                Email = p.ContactEmail
            }).ToList();

            return ResponseDto<IEnumerable<CasePartyDto>>.Success(dtoList, "Case parties retrieved successfully.");
        }
        public async Task<ResponseDto<CasePartyDto?>> GetCasePartyByIdAsync(Guid casePartyId)
        {
            var party = await _casePartyRepository.GetCasePartyByIdAsync(casePartyId);
            if (party == null)
                return ResponseDto<CasePartyDto?>.Fail("Case party not found.");

            var dto = new CasePartyDto
            {
                Id = party.Id,
                CaseId = party.CaseId,
                FullName = party.FullName,
                Contact = party.ContactPhone,
                Role = party.Role,
                Email = party.ContactEmail
            };

            return ResponseDto<CasePartyDto?>.Success(dto, "Case party retrieved successfully.");
        }
        public async Task<ResponseDto<CasePartyUpdateDto>> UpdateCasePartyAsync(CasePartyUpdateDto updatedParty, Guid modifiedBy)
        {
            var party = await _casePartyRepository.GetCasePartyByIdAsync(updatedParty.Id);
            if (party == null)
                return ResponseDto<CasePartyUpdateDto>.Fail("Case party not found.");

            party.FullName = updatedParty.FullName ?? party.FullName;
            party.ContactPhone = updatedParty.Contact ?? party.ContactPhone;
            party.ContactEmail = updatedParty.Email ?? party.ContactEmail;
            party.Role = updatedParty.Role ?? party.Role;

            await _casePartyRepository.UpdateCasePartyAsync(party);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Case party {PartyId} updated by user {ModifiedBy}", updatedParty.Id, modifiedBy);

            return ResponseDto<CasePartyUpdateDto>.Success(updatedParty, "Case party updated successfully.");
        }
    }
}