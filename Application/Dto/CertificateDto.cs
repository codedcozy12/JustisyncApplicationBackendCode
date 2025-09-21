using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class CertificateDto
    {
        public Guid Id { get; set; }
        public string CertificateType { get; set; } = default!;
        public string FilePath { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
        public Guid LawyerId { get; set; }
        public string LawyerName { get; set; } = default!;
    }
    public class CertificateCreateRequestDto
    {
        public string CertificateType { get; set; } = default!;
        public IFormFile File { get; set; } = default!;
        public Guid LawyerId { get; set; }
    }
    public class CertificateUpdateRequestDto
    {
        public bool IsVerified { get; set; }
        public Guid? VerifiedByUserId { get; set; }
    }
    public class CertificateUploadRequestDto
    {
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        public List<string> CertificateType { get; set; } = default!;
        public Guid LawyerId { get; set; }
    }
}