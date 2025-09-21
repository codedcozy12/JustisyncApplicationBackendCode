using Application.Dto;

namespace Application.IService
{
    public interface IRegistrarService
    {
       Task<ResponseDto<Guid>> CreateRegistrarAsync(RegistrarCreateRequestDto request);
        Task<ResponseDto<RegistrarDto>> GetRegistrarByIdAsync(Guid registrarId);
        Task<ResponseDto<IEnumerable<RegistrarDto>>> GetAllRegistrarsAsync();
        Task<RegistrarUpdateRequestDto> UpdateRegistrarAsync(Guid registrarId, RegistrarUpdateRequestDto request);
        Task<bool> DeleteRegistrarAsync(Guid registrarId);
        Task<ResponseDto<IEnumerable<RegistrarDto>>> GetRegistrarsByCourtAsync(Guid courtId);
        Task<RegistrarUpdateRequestDto> AssignRegistrarToCourtAsync(Guid registrarId, Guid courtId, DateTime? appointmentDate);
    }
}