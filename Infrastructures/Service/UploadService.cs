using Application.Interfaces.ExternalService;
using Microsoft.AspNetCore.Http;

namespace Infrastructures.Service
{
    public class UploadService : IUploadService
    {
        public string? UploadProfile(IFormFile file)
        {
            var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserImages");
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
            var contentType = file.ContentType;

            string subFolder = contentType switch
            {
                var type when type.StartsWith("image/") => "Images",
                var type when type.StartsWith("video/") => "Videos",
                var type when type.StartsWith("application/") => "Documents",
                _ => "Others"
            };

            var folder = Path.Combine(rootFolder, subFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }
        public string UploadFile(IFormFile file)
        {
            var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Certificate");
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
            var contentType = file.ContentType;

            string subFolder = contentType switch
            {
                var type when type.StartsWith("image/") => "Images",
                var type when type.StartsWith("video/") => "Videos",
                var type when type.StartsWith("application/") => "Documents",
                _ => "Others"
            };

            var folder = Path.Combine(rootFolder, subFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }
        public string UploadDocument(IFormFile file)
        {
            var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Documents");
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
            var contentType = file.ContentType;

            string subFolder = contentType switch
            {
                var type when type.StartsWith("image/") => "Images",
                var type when type.StartsWith("video/") => "Videos",
                var type when type.StartsWith("application/") => "Documents",
                _ => "Others"
            };

            var folder = Path.Combine(rootFolder, subFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }
    }
}