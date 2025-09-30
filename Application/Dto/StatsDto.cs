namespace Application.Dto
{
    public record StatsDto
    {
        public int TotalCourt { get; set; }
        public int TotalJudges { get; set; }
        public int TotalClients { get; set; }
        public int TotalRegistrar { get; set; }
        public int TotalLawyers { get; set; }
        public int PendingVerifications { get; set; }
    }
}
