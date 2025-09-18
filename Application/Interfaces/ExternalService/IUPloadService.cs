using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.ExternalService
{
    public interface IUploadService
    {
       string? UploadProfile(IFormFile file);
       string UploadFile(IFormFile file);
    }
}