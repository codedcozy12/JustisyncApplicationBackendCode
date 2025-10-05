namespace Domain.Entities
{
    public class UserCourtSession : BaseEntities
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid CourtSessionId { get; set; }
        public CourtSession CourtSession { get; set; }
    }
}
