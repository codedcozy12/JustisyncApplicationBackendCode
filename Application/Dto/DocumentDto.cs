using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string FileName { get; set; } = default!;
        public string FileType { get; set; } = default!;
        public string FileUrl { get; set; } = default!;
    }
}
    public class DocumentCreateRequestDto
        {
            public Guid CaseId { get; set; }
            public string FileName { get; set; } = default!;
            public string FileType { get; set; } = default!;
            public IFormFile FileUrl { get; set; } = default!;
        }