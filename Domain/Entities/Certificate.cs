namespace Domain.Entities
{
    public class Certificate : BaseEntities
    {
        public string CertificateType { get; set; } = default!;
        public string FilePath { get; set; } = default!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = false;
        public Guid? VerifiedByUserId { get; set; }
        public User? VerifiedBy { get; set; }
        public Guid LawyerId { get; set; }
        public Lawyer Lawyer { get; set; } = default!;
    }
}