using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
