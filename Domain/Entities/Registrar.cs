using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Domain.Entities
{
    public class Registrar: BaseEntities
    {
        public Guid CourtId { get; set; }
        public Court Court { get; set; } = default!;
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
    }
}